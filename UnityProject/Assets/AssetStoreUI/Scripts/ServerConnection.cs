using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerConnection : MonoBehaviour
{
    private TextMesh statusText;
    private AssetManager assetManager;
    
    // Use this for initialization
	void Start ()
	{
	    statusText = GameObject.Find("ServerConnectionStatus").GetComponent<TextMesh>();
        assetManager = GameObject.Find("Asset Manager").GetComponent<AssetManager>();
	    InvokeRepeating("updateStatus", 1.0f, 5f);
    }

    public void updateStatus()
    {
        //        if (assetManager.isConnected())
        //        {
        //        	statusText.color = Color.green;
        //        	statusText.text = assetManager.ServerAddress + ": connected";
        //        }
        //        else
        //        {
        //        	statusText.color = Color.red;
        //        	statusText.text = assetManager.ServerAddress + ": disconnected";
        //        }
    }

}
