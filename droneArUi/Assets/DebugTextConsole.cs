using System;
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
        string typ="O";
        if (type == LogType.Error) { typ = "E"; }
        else if (type == LogType.Warning) { typ = "W"; }
        else if (type == LogType.Log) { typ = "L"; }

        message=ZkratitText(message, 110);

        textMesh.text += String.Format("{0,4}", lineCount) +" |"+typ+": "+ message + "\n";
        lineCount++;
        
    }

    string ZkratitText(string text, int maxDelka)
    {
        if (text.Length <= maxDelka)
        {
            return text; // Pokud je text krat�� ne� maxim�ln� d�lka, vr�t�me ho beze zm�ny
        }
        else
        {
            return text.Substring(0, maxDelka-3) + "..."; // Jinak zkr�t�me text a p�id�me "..."
        }
    }
}
