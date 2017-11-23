using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;

public class ResetBallbounce : MonoBehaviour, ISpeechHandler
{
    [SerializeField]
    float forwardDistance = 6.0f;

    [SerializeField]
    float upwardDistance = 2.0f;

    public Text text;

    private void Start()
    {
        Debug.Log("Attached to " + this.name);
    }

    public void Update()
    {
        
    }

    public void ResetBall()
    {
        gameObject.transform.position =
            Camera.main.transform.position +
            (Camera.main.transform.up * upwardDistance) +
            (Camera.main.transform.forward * forwardDistance);
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData.RecognizedText == "reset")
        {
            ResetBall();
        }
    }
}
