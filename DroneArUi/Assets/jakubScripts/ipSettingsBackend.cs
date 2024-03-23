// author: jakub komárek

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ipSettingsBackend : MonoBehaviour
{
    // Start is called before the first frame update

    public WebSocketManager2 WebSocketManager=null;
    public RTMPstreamPlayer rTMPstreamPlayer=null;

    public MRTKTMPInputField ipTelemetry=null;
    public MRTKTMPInputField portTelemetry = null;

    public MRTKTMPInputField ipVideoServer = null;
    public MRTKTMPInputField portVideoServer = null;
    public PressableButtonHoloLens2 sameIpCheckBox = null;
    void Start()
    {
        if (PlayerPrefs.HasKey("ipTelemetry"))
        {
            ipTelemetry.text = PlayerPrefs.GetString("ipTelemetry");
        }
        if (PlayerPrefs.HasKey("ipTelemetryPort"))
        {
            portTelemetry.text = PlayerPrefs.GetString("ipTelemetryPort");
        }
        if (PlayerPrefs.HasKey("ipVideoServer"))
        {
            ipVideoServer.text = PlayerPrefs.GetString("ipVideoServer");
        }
        if (PlayerPrefs.HasKey("portVideoServer"))
        {
            portVideoServer.text = PlayerPrefs.GetString("portVideoServer");
        }
        onConnectetToServerPressed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool checkBoxState=false;
    public void onSameIpCheckBoxClicked()
    {
        checkBoxState=!checkBoxState;

        if (checkBoxState)
        {
            ipVideoServer.text = ipTelemetry.text;
        }
    }

    public void onIpTelemetryTextChanged()
    {
        if (checkBoxState)
        {
            ipVideoServer.text = ipTelemetry.text;
        }
    }

    public void onIpVideServerChanged()
    {

        if (checkBoxState)
        {
            ipTelemetry.text = ipVideoServer.text;
        }
    }

    public void onConnectetToServerPressed()
    {
        if (int.TryParse(portTelemetry.text, out int result))
        {
            WebSocketManager.ConnectToServer(ipTelemetry.text, result);
        }
        else
        {
            Console.WriteLine("Invalid format for conversion using int.TryParse()");
            return;
        }

        rTMPstreamPlayer.ip=ipVideoServer.text;
        rTMPstreamPlayer.port=portVideoServer.text;

        PlayerPrefs.SetString("ipTelemetry", ipTelemetry.text);
        PlayerPrefs.SetString("ipTelemetryPort", portTelemetry.text);
        PlayerPrefs.SetString("ipVideoServer", ipVideoServer.text);
        PlayerPrefs.SetString("portVideoServer", portVideoServer.text);
        PlayerPrefs.Save();
    }
}

