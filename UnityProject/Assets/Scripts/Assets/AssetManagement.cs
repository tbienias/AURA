using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

[RequireComponent(typeof(SceneTransfer),typeof(AssetManager))]
public class AssetManagement : Singleton<AssetManagement> {

    protected AssetManagement() { }

    SceneTransfer _sceneM = null;
    AssetManager _assetM = null;
    
    public SceneTransfer SceneManager
    {
        get
        {
            if (_sceneM)
            { 
                return _sceneM;
            }
            else
            {
                return (_sceneM = GameObject.Find("Asset Manager").GetComponent<SceneTransfer>());
            }
        }
    }

    public AssetManager AssetManager
    {
        get
        {
            if (_assetM)
            {
                return _assetM;
            }
            else
            {
                return (_assetM = GameObject.Find("Asset Manager").GetComponent<AssetManager>());
            }
        }
    }

}
