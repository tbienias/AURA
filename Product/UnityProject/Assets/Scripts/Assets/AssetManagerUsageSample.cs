using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManagerUsageSample : MonoBehaviour
{
    private AssetManager _mgr;

    // Use this for initialization
    void Start()
    {
        // Get the manager. Must exist in the scene
        _mgr = GameObject.Find("Asset Manager").GetComponent<AssetManager>();

        // Request the list of assets, result is delivered asynchronously to the callback.
        _mgr.RequestAssetList(AssetListCallBack);
    }

    void AssetListCallBack(List<string> assetList)
    {
        foreach (var asset in assetList)
        {
            _mgr.RequestAsset(asset, AssetCallBack);
        }
    }

    void AssetCallBack(GameObject loadedAsset)
    {
        Vector3 pos = new Vector3(0, 0, 2);
        Quaternion rot = new Quaternion(45.0f, 45.0f, 45.0f, 0.0f);
        Instantiate(loadedAsset, pos, rot);
    }
}
