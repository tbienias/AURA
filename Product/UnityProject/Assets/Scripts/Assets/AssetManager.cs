using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    [Tooltip("Specifiy the server where to look for assets.")]
    public string ServerAddress = "127.0.0.1";

    public delegate void AssetLoadedFunc(GameObject loadedAsset);
    public delegate void AssetListLoadedFunc(List<string> assetList);

    public event AssetLoadedFunc OnAssetLoaded;
    public event AssetListLoadedFunc OnAssetListLoaded;

    private WWW _server;

    // Map equivalent to hold gameobject references that are already loaded for caching purposes
    private Dictionary<string, GameObject> _gameObjects;


    private void Start()
    {
        _gameObjects = new Dictionary<string, GameObject>();
    }

    void CheckIfServerIsConnected()
    {
        if (_server == null)
        {
            _server = new WWW(ServerAddress);
        }
    }

    public void RequestAssetList(AssetListLoadedFunc callBack)
    {
        CheckIfServerIsConnected();
        Debug.Log("Asset Manager: Requesting asset list from server...");
        StartCoroutine(RequestAssetList(_server, callBack));
    }

    public void RequestAsset(string assetURI, AssetLoadedFunc callBack)
    {
        CheckIfServerIsConnected();

        // Check if asset is already loaded, if it is, just call the callback immediately
        if(_gameObjects.ContainsKey(assetURI))
        {
            callBack(_gameObjects[assetURI]);
            if(OnAssetLoaded != null)
            {
                OnAssetLoaded(_gameObjects[assetURI]);
            }
            return;
        }

        // Start the download process
        Debug.Log(string.Format("Asset Manager: Requesting asset from server: {0}...", assetURI));
        WWW assetRequest = new WWW(string.Format("{0}/{1}", _server.url, assetURI));
        StartCoroutine(RequestAsset(assetRequest, callBack, assetURI));
    }

    IEnumerator RequestAsset(WWW request, AssetLoadedFunc callBack, string assetURI)
    {
        // Ensure server download is finished, before continuing
        yield return request;
        Debug.Log(string.Format("Asset Manager: Asset download completed."));

        // Get asset file name
        string assetName = request.assetBundle.GetAllAssetNames()[0];

        // Build game object from request bundle
        AssetBundle bundle = request.assetBundle;
        GameObject gameObject = bundle.LoadAsset(assetName) as GameObject;

        // Add gameobject to caching map
        _gameObjects.Add(assetURI, gameObject);

        // Dispatch event
        callBack(gameObject);
        if (OnAssetLoaded != null)
        {
            OnAssetLoaded(gameObject);
        }
    }

    IEnumerator RequestAssetList(WWW server, AssetListLoadedFunc callBack)
    {
        // Defer this function to the next frame until server replied and response has been downloaded
        yield return server;
        Debug.Log("Asset Manager: Asset list finished downloading.");

        AssetBundleListing listing = JsonUtility.FromJson<AssetBundleListing>(server.text);

        List<string> assetList = new List<string>();
        foreach (var bundle in listing.AssetBundleList)
        {
            assetList.Add(bundle.BundleName);
        }

        // Dispatch events
        callBack(assetList);
        if (OnAssetListLoaded != null)
        {
            OnAssetListLoaded(assetList);
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
    public string[] Assets;
}