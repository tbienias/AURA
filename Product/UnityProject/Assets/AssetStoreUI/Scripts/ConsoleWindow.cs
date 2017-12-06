using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleWindow : MonoBehaviour {

    struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    private Text text;
    List<Log> logs = new List<Log>();
    GameObject consoleTextWindow;
    GameObject consoleCanvas;
    GameObject debugError;

    bool show;
    public int clearCount = 19;

    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        {LogType.Assert, Color.white },
        {LogType.Error, Color.red },
        {LogType.Exception, Color.red },
        {LogType.Log, Color.white },
        {LogType.Warning, Color.yellow }
    };

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;

        consoleTextWindow = GameObject.Find("ConsoleText");
        consoleCanvas = GameObject.Find("ConsoleCanvas");
        text = consoleTextWindow.GetComponent<Text>();
        show = true;

        Debug.Log("OnEnable");
        
    }

    void Update()
    {
        if(logs.Count >= clearCount)
        {
            ClearConsole();
        }
        
    }

    public void Show()
    {
        Debug.Log("Show");
        consoleCanvas.SetActive(true);
        show = true;
    }

    public void Hide()
    {
        Debug.Log("Hide");
        consoleCanvas.SetActive(false);
        show = false;
    }

    public void ClearConsole()
    {
        logs.Clear();
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        string nullstring = "";
        text.text = nullstring;
        string temp = text.text;

        logs.Add(new Log()
        {
            message = message,
            stackTrace = stackTrace,
            type = type
        });

        for(int i = 0; i < logs.Count; i++)
        {
            var log = logs[i];
            text.color = logTypeColors[log.type];
            text.text += temp + "\n" + log.message;
            text.text += "\n " + log.stackTrace;
        }
    }

    public void RandomMessage()
    {
        HandleLog("LogMessage", "StackTrace", LogType.Log);
    }

}
