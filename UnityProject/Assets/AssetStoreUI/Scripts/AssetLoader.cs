using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AssetStoreUI.Scripts;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;


public class AssetLoader : MonoBehaviour
{
    
    public GameObject buttonPrefab;
    public GameObject buttonContainer;
    public uint openPage = 0;

    private Camera _camera;
    

    public List<string> assetList;
    public AssetManager _mgr;

    public Boolean listLoadprogress;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

	// Use this for initialization
	void Start ()
	{
        _mgr = AssetManagement.Instance.AssetManager;

	    listLoadprogress = false;
        Debug.Log("request assetlist");
	    _mgr.RequestAssetList(getAssetList);
	    _camera = Camera.main;
        
        List<string> assets = new List<string>{"Button1", "Button2"};
	    foreach (string asset in assets)
	    {
	        createNewButton(asset);
	    }

        AssetManagement.Instance.SceneManager.OnAssetLoaded += SceneManager_OnAssetLoaded;

    }
    
    private void SceneManager_OnAssetLoaded(GameObject loadedAsset)
    {
        // add mesh collider/renderer
//        Utility.meshColliderRecursive(loadedAsset.transform, true);
//        Utility.meshRendererRecursive(loadedAsset.transform, true);
//        Utility.outlineRecursive(loadedAsset.transform, true);

        //loadedAsset.AddComponent<TapToPlace>();        
    }

    private void loadAssetFromServer(string assetName)
    {
        _mgr.RequestAsset(assetName, assetloadfunc);
        Debug.Log(assetName);
    }

    public void clearAssetContainer()
    {
        Button[] buttons = buttonContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
            Destroy(buttons[i].gameObject);
        }
    }

    public void showAssets()
    {
        clearAssetContainer();
        int itemsCount = 10;
        Debug.Log(openPage);
        if (openPage * 10 + 9 > assetList.Count)
        {
            itemsCount = assetList.Count % 10;
        }

        List<string> partAssetList = assetList.GetRange((int)openPage * 10, itemsCount);

        foreach (string asset in partAssetList)
        {
            createNewButton(asset);
        }
    }

    private void createNewButton(string asset)
    {
        GameObject newButton = Instantiate(buttonPrefab, buttonContainer.transform);
        newButton.GetComponentInChildren<Text>().text = asset;
    
        Button.ButtonClickedEvent clickEvent = newButton.GetComponent<Button>().onClick;
        clickEvent.AddListener( () => loadAssetFromServer(asset) );
    }

    public void nextPage()
    {
        //clearAssetContainer();
        //showAssets(new List<string>(){"a", "b"});
        if (assetList.Count / 10 == openPage)
        {
            Debug.Log("Already highest page!");
        }
        else
        {
            openPage++;
            clearAssetContainer();
            showAssets();
        }
    }

    

    public void prevPage()
    {
        clearAssetContainer();
        
        if (openPage == 0)
        {
            Debug.Log("First Page already open!");
        }
        else
        {
            openPage--;
            clearAssetContainer();
            showAssets();
        }
    }

    public void getAssetList(List<string> list)
    {
        Debug.Log("IP: " + _mgr.ServerAddress);
        if (list == null)
        {
            Debug.Log("Error: " + _mgr.LastOccuredError);
            return;
        }
        Debug.Log("Size: " + list.Count);
        assetList = list;
        
        listLoadprogress = true;
        showAssets();
    }

    public void assetloadfunc(GameObject go)
    {
        if (go != null)
        {
            go.transform.position = _camera.transform.position + Camera.main.transform.forward * 2;
            go.transform.rotation = _camera.transform.rotation;

            //            if (go.GetComponent<BoxCollider>() == null)
            //            {
            //                go.AddComponent<BoxCollider>();
            //            }

            // add mesh collider/renderer
            Utility.meshColliderRecursive(go.transform, true);
            Utility.meshRendererRecursive(go.transform, true);
            Utility.outlineRecursive(go.transform, true);

            //go.AddComponent<TapToPlace>();

            // Scale object
            go.transform.localScale = go.transform.localScale * 0.5f;

            go.transform.position = _camera.transform.position + _camera.transform.forward * 4;

            Debug.Log("Spawned ");

            GazeInputHandler.Instance.grabObject(go);
            
        }   
        else
        {
            Debug.Log("Game Object Null!");
        }
    }

    public void onSaveScene()
    {
        AssetManagement.Instance.SceneManager.SaveCurrentScene("Test");
    }
    public void onLoadScene()
    {
        AssetManagement.Instance.SceneManager.LoadScene("Test");
    }
}
