using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetManager : MonoBehaviour
{
    [Tooltip("Specifiy the server where to look for assets.")]
    public string ServerAddress = "127.0.0.1";

    public delegate void AssetLoadedFunc(GameObject loadedAsset);
    public delegate void AssetListLoadedFunc(List<string> assetList);
    public delegate void DataSentToServerFunc(string file, bool sent);

    public event AssetLoadedFunc OnAssetLoaded;
    public event AssetListLoadedFunc OnAssetListLoaded;

    public string LastOccuredError
    {
        get { return _lastError; }
    }

    // Map equivalent to hold gameobject references that are already loaded for caching purposes
    private Dictionary<string, GameObject> _gameObjects;
    private Dictionary<string, byte[]> _thumbnails;

    private string _lastError;

    protected AssetManager() { }

    private void Start()
    {
        _gameObjects = new Dictionary<string, GameObject>();
        _thumbnails = new Dictionary<string, byte[]>();
    }

    /// <summary>
    /// Sends a request to the Asset Server and fetches the list of all
    /// stored assets waiting to be queried asynchroniously.
    /// Due to the returning list containing the Thumbnails of the AssetBundles,
    /// this Request is a network-heavy process and should only be called once.
    /// </summary>
    /// <param name="callBack">
    /// Method to be called when returning the list of Assets.
    /// Callback will get a null-argument if an error occured while fetchting the list.
    /// </param>
    public void RequestAssetList(AssetListLoadedFunc callBack)
    {
        Debug.Log("Asset Manager: Requesting asset list from server...");
        StartCoroutine(LoadAssetList(callBack));
    }

    /// <summary>
    /// Load an AssetBundle from the dedicated AssetServer and serve it to the
    /// specified Callback-Method.
    /// Callback will get a null-argument if an error occured while fetchting the Asset.
    /// </summary>
    /// <param name="assetURI">AssetBundle to fetch from server (preserves folder hirarchy)</param>
    /// <param name="callBack">Method to call once the request is finished</param>
    /// <notes>
    /// The Asset - or rather GameObject - which gets created and served to the callback is a clone
    /// of the asset. You may do whatever you want with it 
    /// </notes>
    public void RequestAsset(string assetURI, AssetLoadedFunc callBack)
    {
        // Start the download process
        Debug.Log(string.Format("Asset Manager: Requesting asset from server: {0}...", assetURI));
        UnityWebRequest assetRequest = UnityWebRequest.GetAssetBundle((string.Format("{0}/Bundle/{1}", this.ServerAddress, assetURI)));
        StartCoroutine(LoadAsset(assetRequest, callBack, assetURI));
    }

    /// <summary>
    /// Gets the thumbnail-Texture of the specified AssetBundle.
    /// For this to work, a call to RequestAssetList must have been made before.
    /// </summary>
    /// <param name="assetName">Name of the AssetBundle for which to get the thumbnail to</param>
    /// <returns>A byte array containing the Thumbnails' (raw) Image-Data or null, if no Thumbnail was loaded for specified Asset</returns>
    byte[] GetAssetThumbnail(string assetName)
    {
        if (_thumbnails.ContainsKey(assetName))
            return _thumbnails[assetName];
        else
            return null;
    }

    /// <summary>
    /// Sends the given string to the AssetServer and puts it into a file
    /// </summary>
    /// <param name="textToSend">A string representing the content of the file</param>
    /// <param name="filename">The file in which to put the contents. Can be prefixed by a folder-structure.</param>
    /// <param name="callback">Optional callback which will be called upon completing the upload</param>
    public void SendDataToServer(string textToSend, string filename, DataSentToServerFunc callback = null)
    {
        Debug.Log(string.Format("Asset Manager: Sending data to server: {0}...", filename));
        UnityWebRequest req = UnityWebRequest.Put((string.Format("{0}/{1}", this.ServerAddress, filename)), textToSend);
        StartCoroutine(SaveData(req, callback));
    }

    IEnumerator SaveData(UnityWebRequest request, DataSentToServerFunc callback = null)
    {
        yield return request.SendWebRequest();

        // Get the actual filename
        string[] uri = request.url.Split('/');
        string filename = uri[uri.Length-1];
        bool couldBeSent = false;

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(string.Format("Couldn't upload file {0} due to: {1}", request.url, request.error));
        }
        else
        {
            couldBeSent = true;
        }

        if (callback != null)
            callback(filename, couldBeSent);
    }

    IEnumerator LoadAsset(UnityWebRequest assetRequest, AssetLoadedFunc callBack, string assetURI)
    {
        GameObject gameObject = null;

        // Check if asset is already loaded, if it is, just call the callback immediately
        if (_gameObjects.ContainsKey(assetURI))
        {
            gameObject = _gameObjects[assetURI];
        }
        else
        {
            // Ensure server download is finished, before continuing
            yield return assetRequest.SendWebRequest();

            if (assetRequest.isHttpError || assetRequest.isNetworkError)
            {
                _lastError = assetRequest.error;

                if (callBack != null)
                    callBack(null);

                yield break;
            }
        }

        if (gameObject == null)
        {
            Debug.Log(string.Format("Asset Manager: Asset download completed."));

            AssetBundle bundle = ((DownloadHandlerAssetBundle)assetRequest.downloadHandler).assetBundle;

            // Build game object from request bundle
            // Per definition (or more likely due to technical limitations at this time) 
            // it has to be the first asset in the bundle
            gameObject = bundle.LoadAsset(bundle.GetAllAssetNames()[0]) as GameObject;

            // Per definition, every Asset can be saved.
            SceneTransfer.SetSavableStatus(gameObject, true).AssetBundleName = bundle.name;

            // Add gameobject to caching map
            _gameObjects.Add(assetURI, gameObject);
        }

        gameObject = GameObject.Instantiate(gameObject);

        // Dispatch event
        if (callBack != null)
            callBack(gameObject);

        if (OnAssetLoaded != null)
            OnAssetLoaded(gameObject);

    }

    IEnumerator LoadAssetList(AssetListLoadedFunc callBack)
    {
        // Defer this function to the next frame until server replied and response has been downloaded
        UnityWebRequest req = UnityWebRequest.Get(string.Format("{0}/Bundle", this.ServerAddress));
        yield return req.SendWebRequest();

        if (req.isNetworkError || req.isHttpError)
        {
            
            _lastError = req.error;

            if (callBack != null)
                callBack(null);
        }
        else
        {
            Debug.Log("Asset Manager: Asset list finished downloading.");

            AssetBundleListing listing = JsonUtility.FromJson<AssetBundleListing>(req.downloadHandler.text);

            List<string> assetList = new List<string>();
            foreach (var bundle in listing.AssetBundleList)
            {
                assetList.Add(bundle.BundleName);

                if (bundle.Thumbnail != null)
                {
                    _thumbnails[bundle.BundleName] = System.Convert.FromBase64String(bundle.Thumbnail);
                    bundle.Thumbnail = null;
                }
            }

            if (callBack != null)
            {
                // Dispatch events
                callBack(assetList);
                if (OnAssetListLoaded != null)
                {
                    OnAssetListLoaded(assetList);
                }
            }
        }
    }


    // For server messaging
    [System.Serializable]
    public class AssetBundleListing
    {
        public AssetBundleItem[] AssetBundleList;
    }

    [System.Serializable]
    public class AssetBundleItem
    {
        public string BundleName;
        //public string Asset;
        public string Thumbnail;
    }
}
