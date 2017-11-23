using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeInputHandler : MonoBehaviour
{

    private GazeInput gazeInput;
    private Canvas uiCanvas;

	// Use this for initialization
	void Start () {
        Camera camera = GameObject.Find("MixedRealityCamera").GetComponent<Camera>();
	    uiCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
	    gazeInput = GazeInput.Instance;
        gazeInput.setCamera(camera);
	}
	
	// Update is called once per frame
	void Update () {
		gazeInput.moveObject();
        gazeInput.canvasRotation(uiCanvas);
	}
}
