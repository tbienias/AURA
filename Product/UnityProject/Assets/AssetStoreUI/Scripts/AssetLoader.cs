using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetLoader : MonoBehaviour
{
    
    public GameObject buttonPrefab;
    public GameObject buttonContainer;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

	// Use this for initialization
	void Start ()
	{
	    List<string> assets = new List<string>{"Button1", "Button2"};
	    foreach (string asset in assets)
	    {
	        createNewButton(asset);
	    }
	}

    private void loadAssetFromServer(string assetName)
    {
        Debug.Log(assetName);
    }

    public void showAssets(List<string> assets)
    {
        foreach (string asset in assets)
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
        clearAssetContainer();
        showAssets(new List<string>(){"a", "b"});
    }

    private void clearAssetContainer()
    {
        Button[] buttons = buttonContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
            Destroy(buttons[i].gameObject);
        }
    }

    public void prevPage()
    {
        clearAssetContainer();
        showAssets(new List<string>() { "a", "b", "c", "d" });
    }

}
