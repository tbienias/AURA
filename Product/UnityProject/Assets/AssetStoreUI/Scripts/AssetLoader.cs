using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetLoader : MonoBehaviour
{
    
    public GameObject buttonPrefab;
    public GameObject buttonContainer;
    public uint openPage = 0;


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
        _mgr = GameObject.Find("Asset Manager").GetComponent<AssetManager>();

	    listLoadprogress = false;
	    _mgr.RequestAssetList(getAssetList);

        
        List<string> assets = new List<string>{"Button1", "Button2"};
	    foreach (string asset in assets)
	    {
	        createNewButton(asset);
	    }
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
        uint lastIndexOfList = openPage * 10 + 9;
        if (openPage * 10 + 9 > assetList.Count)
        {
            lastIndexOfList = (uint)assetList.Count ;
        }
        List<string> partAssetList = assetList.GetRange((int)openPage * 10, (int)lastIndexOfList);
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
        }
    }

    

    public void prevPage()
    {
        clearAssetContainer();
        //showAssets(new List<string>() { "a", "b", "c", "d" });
        if (openPage == 0)
        {
            Debug.Log("First Page already open!");
        }
        else
        {
            openPage--;
        }
           
    }

    public void getAssetList(List<string> list)
    {
        assetList = list;
        listLoadprogress = true;
        showAssets();
    }

    public void assetloadfunc(GameObject go)
    {
        if (go != null)
        {
            GameObject cam = GameObject.Find("MixedRealityCamera");
            Instantiate(go, (cam.transform.position + Camera.main.transform.forward * 2), cam.transform.rotation);
            Debug.Log("Spawned ");
        }
        else
        {
            Debug.Log("Game Object Null!");
        }
    }
}
