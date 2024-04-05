/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// stará se o zadání ip adres serverù, adresy persistentnì ukládá a nastavuje je pøi zapnutí programu
/// </summary>

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using UnityEngine;

public class ipSettingsBackend : Singleton<ipSettingsBackend>
{
    [SerializeField]
    WebSocketManager2 WebSocketManager = null;

    [SerializeField]
    public MRTKTMPInputField ipTelemetry = null;
    [SerializeField]
    public MRTKTMPInputField portTelemetry = null;

    [SerializeField]
    public MRTKTMPInputField ipVideoServer = null;
    [SerializeField]
    public MRTKTMPInputField portVideoServer = null;
    [SerializeField]
    public PressableButtonHoloLens2 sameIpCheckBox = null;

    public List<RTMPstreamPlayer> streamPlayerList = new List<RTMPstreamPlayer>();
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


    bool checkBoxState = false;
    public void onSameIpCheckBoxClicked()
    {
        checkBoxState = !checkBoxState;

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

