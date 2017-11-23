using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class VoiceInputHandler : MonoBehaviour, ISpeechHandler
{

    private GazeInput gazeInput;
    private Canvas uiCanvas;

	// Use this for initialization
	void Start () {
		gazeInput = GazeInput.Instance;	    
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        switch (eventData.RecognizedText)
        {
            case "cube":
                ObjectSpawner.spawnCube();
                break;
        
            case "grab":
                gazeInput.grabObject();
                break;
        
            case "drop":
                gazeInput.dropObject();
                break;
        
            case "position":
                uiCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                gazeInput.repositionCanvas(uiCanvas);
                break;
        
            default:
        
                break;
        }
    }
}
