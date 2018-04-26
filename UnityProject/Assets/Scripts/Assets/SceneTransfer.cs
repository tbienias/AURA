using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO.Compression;

public class SceneTransfer : MonoBehaviour
{
    public delegate void SceneSaveListLoaded(SceneSaveItem[] saveList);

    public event AssetManager.AssetLoadedFunc OnAssetLoaded;

    public string LastOccuredError
    {
        get { return _lastError; }
    }

    string  _lastError;
    bool    _isSaving = false;
    int     _entitiesToLoad = 0;

    Dictionary<string, SaveDataList>    _sceneCache;
    Dictionary<string, List<SaveData>> _entityCache;

    protected SceneTransfer() { }

    private void Awake()
    {
        _sceneCache = new Dictionary<string, SaveDataList>();
    }

    /// <summary>
    /// Adds - if not present - a SaveLoadBehavior-Component to the specified GameObject
    /// and sets the Savable-State accordingly.
    /// </summary>
    /// <param name="gameobject">The GameObject you want to set to be savable</param>
    /// <param name="isSavable">Setting the savable-State to true will enable saving of the GameObjects' data</param>
    /// <returns>The SaveLoad-Component of the GameObject. It is not advisable to manually change any data though!</returns>
    static public SaveLoadBehavior SetSavableStatus(GameObject gameobject, bool isSavable)
    {
        SaveLoadBehavior saveload = gameobject.GetComponent<SaveLoadBehavior>() as SaveLoadBehavior;
        if (saveload == null)
        {
            saveload = gameobject.AddComponent<SaveLoadBehavior>();
        }

        saveload.IsSavable = isSavable;

        return saveload;
    }

    /// <summary>
    /// Cycles through all GameObjects in the Scene which has the SaveLoadBehavior and
    /// serializes them if they are set to be savable.
    /// The serialized file is then sent to the AssetServer using the specified save name.
    /// </summary>
    /// <param name="saveName">Name under which the save file should be put under</param>
    /// <param name="callBack">Optional Callback which gets called after sending the data (only way to check for errors)</param>
    public void SaveCurrentScene(string saveName, AssetManager.DataSentToServerFunc callBack = null)
    {
        if (_isSaving)
        {
            // TODO: 'Real' error handling, so the caller knows that there's another save-process running
            Debug.Log("Can't Save while another Save-Process is ongoing");
            return;
        }

        _isSaving = true;

        SaveLoadBehavior[] all = GameObject.FindObjectsOfType<SaveLoadBehavior>();

        if (all.Length == 0)
        {
            Debug.Log("There are no GameObjects designated to be saved in the scene!");
            return;
        }

        SaveData data = null;
        SaveDataList dataList = new SaveDataList() { DataList = new List<SaveData>() };

        GameObject obj = null;
        foreach (SaveLoadBehavior component in all)
        {
            if (!component.IsSavable)
                continue;

            obj = component.gameObject;

            data = new SaveData()
            {
                InstanceID = obj.GetInstanceID(),
                Name = obj.name,
                AssetBundle = component.AssetBundleName
            };
            data.Serialize(obj);

            dataList.DataList.Add(data);
        }

        if (dataList.DataList.Count == 0)
        {
            Debug.Log("There are no GameObjects designated to be saved in the scene!");
        }
        else
        {
            string strToSave = JsonUtility.ToJson(dataList, false);
            /*
            // COMPRESSION MUCH?
            using (var output = new System.IO.MemoryStream())
            {
                using (DeflateStream gzip = new DeflateStream(output, CompressionMode.Compress))
                {
                    using (var writer = new System.IO.StreamWriter(gzip, System.Text.Encoding.ASCII))
                    {
                        writer.Write(strToSave);
                    }
                }
                byte[] yay = output.ToArray();
                string bay = System.Text.Encoding.ASCII.GetString(yay);
                AssetManagement.Instance.AssetManager.SendDataToServer(output.ToString(), string.Format("Scene/{0}", saveName), callBack);
            }
            */
            AssetManagement.Instance.AssetManager.SendDataToServer(strToSave, string.Format("Scene/{0}", saveName), callBack);
        }
        _isSaving = false;
    }

    /// <summary>
    /// Queries the List of all previously saved Scenes and passes the list to the
    /// specified callback-method.
    /// </summary>
    /// <param name="callBack">
    /// Method which is called upon once the list has been fetched
    /// Callback will get a null-argument if an error occured while fetchting the list.
    /// </param>
    public void GetListOfSavedScenes(SceneSaveListLoaded callBack)
    {
        Debug.Log("Scene Manager: Requesting scene list from server...");
        StartCoroutine(FetchSceneList(callBack));
    }

    /// <summary>
    /// Fetches the required Assets from the Server to load the Scene with the provided name.
    /// </summary>
    /// <param name="saveName">Name of the Savefile to load</param>
    /// <param name="cacheScene">Whether the loaded Scene should be put/fetched into/from a cache for subsequent calls (set to false to force-reload a scene)</param>
    public void LoadScene(string saveName, bool cacheScene = false)
    {
        if (_entitiesToLoad > 0)
        {
            Debug.Log(string.Format("Scene Manager: Can't Load Scene [{0}] bevor a previous Load-Operation is finished!", saveName));
            return;
        }
        
        _entityCache = new Dictionary<string, List<SaveData>>();

        Debug.Log(string.Format("Scene Manager: Loading Scene [{0}] ...", saveName));
        UnityWebRequest sceneRequest = UnityWebRequest.Get((string.Format("{0}/Scene/{1}", AssetManagement.Instance.AssetManager.ServerAddress, saveName)));
        StartCoroutine(FetchScene(sceneRequest, saveName, cacheScene));
    }

    IEnumerator FetchSceneList(SceneSaveListLoaded callBack)
    {
        // Defer this function to the next frame until server replied and response has been downloaded
        UnityWebRequest req = UnityWebRequest.Get(string.Format("{0}/Scene", AssetManagement.Instance.AssetManager.ServerAddress));
        yield return req.SendWebRequest();

        if (req.isHttpError || req.isNetworkError)
        {
            _lastError = req.error;

            Debug.Log(string.Format("Couldn't fetch SceneSave-List due to: {0}", req.error));

            if (callBack != null)
                callBack(null);
        }
        else
        {
            Debug.Log("Scene Manager: Scene list finished downloading.");

            SceneSaveListing listing = JsonUtility.FromJson<SceneSaveListing>(req.downloadHandler.text);

            if (callBack != null)
                callBack(listing.SceneList);
        }
    }

    IEnumerator FetchScene(UnityWebRequest request, string saveName, bool cacheScene)
    {
        SaveDataList lst = null;
        _entitiesToLoad = 0;

        if (cacheScene && _sceneCache.ContainsKey(saveName))
        {
            lst = _sceneCache[saveName];
        }
        else
        {
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                _lastError = request.error;

                Debug.Log(string.Format("Couldn't fetch the Scene [{0}] due to: {1}", saveName, request.error));

                yield break;
            }
            else
            {
                lst = JsonUtility.FromJson<SaveDataList>(request.downloadHandler.text);
            }
        }

        _entitiesToLoad = lst.DataList.Count;
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        if (_entitiesToLoad > 0)
        {
            foreach (var entitiyData in lst.DataList)
            {
                if (entitiyData.AssetBundle == "")
                {
                    // Object was placed in the Editor. Search for the GameObject in current scene
                    foreach (var go in allObjects)
                    {
                        if (go.GetInstanceID() == entitiyData.InstanceID)
                        {
                            // Hurray! We found the GameObject matching our saved data!
                            entitiyData.Deserialize(go);
                            _entitiesToLoad--;

                            // TODO: Find out if it is of interest to signal the AssetLoaded-Event here
                            break;
                        }
                    }
                }
                else
                {
                    // Asset is not in the current scene (not placed in the editor),  so we have to manually load it 
                    // from the server - therefore, put it into a list for further use

                    if (!_entityCache.ContainsKey(entitiyData.AssetBundle))
                        _entityCache[entitiyData.AssetBundle] = new List<SaveData>();

                    _entityCache[entitiyData.AssetBundle].Add(entitiyData);
                }
            }

            if (_entitiesToLoad > 0)
            {
                foreach (var dispatchJob in _entityCache)
                {
                    AssetManagement.Instance.AssetManager.RequestAsset(dispatchJob.Key, EntityFinishedLoading);
                }
            }
        }
    }

    void EntityFinishedLoading(GameObject loadedAsset)
    {
        if (loadedAsset == null)
        {
            Debug.Log("Scene Manager: Something very, very wrong happened!");
            _entitiesToLoad = 0;

            return;
        }

        SaveLoadBehavior slb = loadedAsset.GetComponent<SaveLoadBehavior>();

        if (slb == null)
        {
            Debug.Log("Scene Manager: Something very, very, very wrong happened!");
        }

        // Grab one of the SaveData from the Cache and apply the changes to the gameobject
        if (_entityCache.ContainsKey(slb.AssetBundleName))
        {
            SaveData data;

            while( _entityCache[slb.AssetBundleName].Count > 0 )
            {
                data = _entityCache[slb.AssetBundleName][0];
                _entityCache[slb.AssetBundleName].RemoveAt(0);

                GameObject assetCopy = GameObject.Instantiate(loadedAsset);
                data.Deserialize(assetCopy);

                if (OnAssetLoaded != null)
                    OnAssetLoaded(assetCopy);

                _entitiesToLoad--;
            }

            _entityCache[slb.AssetBundleName] = null;
            Destroy(loadedAsset);
        }
    }


    [System.Serializable]
    class SaveData
    {
        public int InstanceID;
        public string Name;
        public string AssetBundle;
        public Vector3 Pos;
        public Vector3 Scale;
        public Quaternion Rot;

        public void Serialize(GameObject go)
        {
            Pos = go.transform.position;
            Scale = go.transform.localScale;
            Rot = go.transform.rotation;
        }

        public void Deserialize(GameObject go)
        {
            go.transform.position = Pos;
            go.transform.rotation = Rot;
            go.transform.localScale = Scale;
        }
    }

    [System.Serializable]
    class SaveDataList
    {
        public List<SaveData> DataList;
    }

    [System.Serializable]
    public class SceneSaveListing
    {
        public SceneSaveItem[] SceneList;
    }

    [System.Serializable]
    public class SceneSaveItem
    {
        public float LastModified;
        public string Name;
    }
}
