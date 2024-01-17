using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugTextConsole : MonoBehaviour
{
    public TMP_Text textMesh;
    private int lineCount= 0;
    // Use this for initialization
    void Start()
    {
        //textMesh = gameObject.GetComponentInChildren<TextMesh>();
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
        if(type== LogType.Error||type==LogType.Log) {
            textMesh.text += message + "\n";
            lineCount++;
        }
            
        
    }
}
