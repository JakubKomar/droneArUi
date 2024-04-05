// author: jakub komárek

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ipSettingsBackend :Singleton<ipSettingsBackend>
{
    // Start is called before the first frame update
    [SerializeField]
    WebSocketManager2 WebSocketManager=null;
    //public RTMPstreamPlayer rTMPstreamPlayer=null;

    [SerializeField]
    public MRTKTMPInputField ipTelemetry=null;
    [SerializeField]
    public MRTKTMPInputField portTelemetry = null;

    [SerializeField]
    public MRTKTMPInputField ipVideoServer = null;
    [SerializeField]
    public MRTKTMPInputField portVideoServer = null;
    [SerializeField]
    public PressableButtonHoloLens2 sameIpCheckBox = null;

    public List<RTMPstreamPlayer> streamPlayerList =new List<RTMPstreamPlayer>();
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
            Debug.LogError("Invalid format for conversion using int.TryParse()");
            return;
        }

        PlayerPrefs.SetString("ipTelemetry", ipTelemetry.text);
        PlayerPrefs.SetString("ipTelemetryPort", portTelemetry.text);
        PlayerPrefs.SetString("ipVideoServer", ipVideoServer.text);
        PlayerPrefs.SetString("portVideoServer", portVideoServer.text);
        PlayerPrefs.Save();
    }
}

