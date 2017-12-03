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
        debugWindow = GameObject.Find("Console");
	    gazeInput = GazeInput.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		gazeInput.moveObject();
        gazeInput.canvasRotation(uiCanvas);
        gazeInput.debugWindowRotation(debugWindow);
	}

}
