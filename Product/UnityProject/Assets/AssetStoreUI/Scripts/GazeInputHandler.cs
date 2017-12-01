using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GazeInputHandler : MonoBehaviour
{
    private GazeInput gazeInput;
    private Canvas uiCanvas;
    private GameObject debugWindow;

	// Use this for initialization
	void Start () {
        Camera camera = GameObject.Find("MixedRealityCamera").GetComponent<Camera>();
	    uiCanvas = GameObject.Find("UiCanvas").GetComponent<Canvas>();
        debugWindow = GameObject.Find("DebugEventLog");
	    gazeInput = GazeInput.Instance;
        gazeInput.setCamera(camera);
	}
	
	// Update is called once per frame
	void Update () {
		gazeInput.moveObject();
        gazeInput.canvasRotation(uiCanvas);
        gazeInput.debugWindowRotation(debugWindow);
	}

    public bool dragthispls(GameObject objToDrag)
    {
        gazeInput.dragPlease(objToDrag);
            
        return true;
    }
    public bool toggleGrab()
    {
        gazeInput.toggleGrab();

        return true;
    }
}
