/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// logika debug konsole
/// </summary>

using System;
using TMPro;
using UnityEngine;

public class DebugTextConsole : MonoBehaviour
{
    public TMP_Text textMesh;
    private int lineCount= 0;

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

        string [] split=textMesh.text.Split('\n');
        if(split.Length > 28) {
            textMesh.text = string.Join('\n', split[(split.Length-28)..(split.Length - 1)]);
            textMesh.text += "\n";
        }
       
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

    public void clear()
    {
        textMesh.text= "";
    }
}
