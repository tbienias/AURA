using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using HoloToolkit.Unity;

public class DebugLogManager : Singleton<DebugLogManager> {

    private Text textComponent;
    private Queue<KeyValuePair<int,string>> entries = new Queue<KeyValuePair<int, string>> ();
    private int lastIndex = 0;

    [Tooltip("Number of messages visible")]
    public int Entries = 8;
    [Tooltip("Unity UI Canvas with Text child on which the messages are to be displayed")]
    public GameObject Canvas;

    // Use this for initialization
    void Start()
    {
        Debug.Assert(Canvas != null, "Canvas property must be set!"); 
        textComponent = Canvas.GetComponentInChildren<Text>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
    {
        entries.Enqueue(new KeyValuePair<int, string>(lastIndex++, message));
        if (entries.Count > Entries)
            entries.Dequeue();

        var lines = entries.Select((e => string.Format("[{0,4:D}]  {1}", e.Key, e.Value)));
        textComponent.text = "[Debug Log]\n" + string.Join("\n", lines.ToArray());        
    }

}
