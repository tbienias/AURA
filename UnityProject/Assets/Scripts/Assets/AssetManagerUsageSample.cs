using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetManagerUsageSample : MonoBehaviour
{
    private AssetManager _mgr;
    private Vector3 _lastPosition = new Vector3(-2, 0, 0);

    // Use this for initialization
    void Start()
    {
        // Get the manager. Must exist in the scene
        _mgr = AssetManagement.Instance.AssetManager;

        // Request the list of assets, result is delivered asynchronously.
        _mgr.RequestAssetList(AssetListCallBack);

        AssetManagement.Instance.SceneManager.GetListOfSavedScenes(SceneSaveListCallback);

        //StartCoroutine(TakeSnapshot());
    }

    void SceneSaveListCallback(SceneTransfer.SceneSaveItem[] scenes)
    {
        if (scenes == null)
            return;

        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        foreach (var scene in scenes)
            Debug.Log(string.Format("Scene [{0}] last saved: {1}", scene.Name, dtDateTime.AddSeconds(scene.LastModified).ToLocalTime()));

        AssetManagement.Instance.SceneManager.LoadScene(scenes[0].Name + "ASD");
    }

    IEnumerator TakeSnapshot()
    {
        yield return new WaitForSeconds(5);

        AssetManagement.Instance.SceneManager.SaveCurrentScene("TestSave");
    }

    void AssetListCallBack(List<string> assetList)
    {
        if (assetList == null)
        {
            Debug.Log(string.Format("Couldn't fetch the Assets-List: {0}", _mgr.LastOccuredError));
            return;
        }

        foreach (var assetBundle in assetList)
        {
            //_mgr.RequestAsset(assetBundle, AssetCallBack);
        }
    }

    void AssetCallBack(GameObject loadedAsset)
    {
        if (loadedAsset == null)
        {
            Debug.Log("Coulnd't load Asset from Server");
            return;
        }

        Vector3 pos = new Vector3(_lastPosition.x, -1, 2);
        Quaternion rot = new Quaternion(0.0f, 90.0f, 0.0f, 0.0f);

        loadedAsset.transform.position = pos;
        loadedAsset.transform.rotation = rot;

        if (pos.x != 0)
        {
            pos.x *= -1;

            GameObject newObj = GameObject.Instantiate(loadedAsset, pos, rot);
        }

        _lastPosition.x += 1;
    }
}
