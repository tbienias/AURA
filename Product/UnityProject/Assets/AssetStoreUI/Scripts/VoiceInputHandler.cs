using System.Collections;
using System.Collections.Generic;
using Assets.Hello.Scripts;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class VoiceInputHandler : MonoBehaviour, ISpeechHandler
{
    private GazeInput gazeInput;
    private Canvas uiCanvas;
    private GameObject debugWindow;

	// Use this for initialization
	void Start () {
		gazeInput = GazeInput.Instance;
	    uiCanvas = GameObject.Find("UiCanvas").GetComponent<Canvas>();
	    debugWindow = GameObject.Find("Console");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        switch (eventData.RecognizedText)
        {
            case "cube":
                ObjectSpawner.Instance.spawnCube();
                break;
        
            case "grab":
                gazeInput.grabObject();
                break;
        
            case "drop":
                gazeInput.dropObject();
                break;
        
            case "position":                
                gazeInput.repositionCanvas(uiCanvas);
                break;

            case "debug":
                gazeInput.repositionDebugWindow(debugWindow);
                break;
        
            default:
        
                break;
        }
    }
}
