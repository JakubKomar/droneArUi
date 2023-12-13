/*
 * WebsocketManager - class to connect to websocket server and send recieved messages to another managers
 * 
 * Author : Martin Kyjac (xkyjac00)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading;
#if WINDOWS_UWP
using WebSocketSharpUwp;
#else
using WebSocketSharp;
#endif

public class WebSocketManager : Singleton<WebSocketManager> {

    public string ServerHostname = "loaclhost";
    public int Port = 5555;
    private WebSocket websocket;

    public delegate void DroneFlightDataEventHandler(object sender, DroneFlightDataEventArgs args);
    public class DroneFlightDataEventArgs {
        public string Data {
            get; set;
        }

        public DroneFlightDataEventArgs(string data) {
            Data = data;
        }
    }
    public DroneFlightDataEventHandler OnDroneDataReceived;

    public EventHandler OnConnectedToServer;

    /// <summary>
    /// ARServer domain or IP address
    /// </summary>
    private string serverDomain;

    /// <summary>
    /// Requset id pool
    /// </summary>
    private int requestID = 1;

    /// <summary>
    /// Dictionary of unprocessed responses
    /// </summary>
    private Dictionary<int, string> responses = new Dictionary<int, string>();


    private void Start() {
        ConnectToServer(ServerHostname, Port);
    }


    /// <summary>
    /// Create websocket URI from domain name and port
    /// </summary>
    /// <param name="domain">Domain name or IP address</param>
    /// <param name="port">Server port</param>
    /// <returns></returns>
    public string GetWSURI(string domain, int port) {
        return "ws://" + domain + ":" + port.ToString();
    }

    public void ConnectToServer(string domain = "pcbambusek.fit.vutbr.cz", int port = 5555) {
        try {
            string APIDomainWS = GetWSURI(domain, port);
            websocket = new WebSocket(APIDomainWS);
            serverDomain = domain;

            websocket.OnOpen += OnConnectedWS;
            websocket.OnError += OnErrorWS;
            websocket.OnClose += OnCloseWS;
            websocket.OnMessage += HandleReceivedDataWS;

#if WINDOWS_UWP
            websocket.ConnectAsync();
#else
            websocket.Connect();
#endif
        } catch (UriFormatException ex) {
            Debug.LogError(ex);
        }
    }

    public void ReconnectToServer(string domain = "localhost", int port = 5555) {
        if (websocket != null) {
            websocket.Close();
        }
        ConnectToServer(domain, port);
    }

    private void HandleReceivedDataWS(object sender, MessageEventArgs e) {
        UnityMainThreadDispatcher.Instance().Enqueue(UpdateDroneData(e.Data));
    }

    public IEnumerator UpdateDroneData(string data) {
        DroneManager.Instance.HandleReceivedDroneData(data);
        yield return null;
    }

    private void OnCloseWS(object sender, CloseEventArgs e) {
        Debug.Log("Connection closed!");
    }

    private void OnErrorWS(object sender, ErrorEventArgs e) {
#if WINDOWS_UWP
        Debug.LogError(e.Message);
#else
        Debug.LogError(e.Message + " : " + e.Exception);
#endif
    }

    private void OnConnectedWS(object sender, EventArgs e) {
        Debug.Log("On connected");
        OnConnectedToServer?.Invoke(this, e);
    }
    /// <summary>
    /// Universal method for sending data to server
    /// </summary>
    /// <param name="data">String to send</param>
    /// <param name="key">ID of request (used to obtain result)</param>
    /// <param name="storeResult">Flag whether or not store result</param>
    /// <param name="logInfo">Flag whether or not log sended message</param>
    public void SendDataToServer(string data, int key = -1, bool storeResult = false, bool logInfo = false) {
        if (key < 0) {
            key = Interlocked.Increment(ref requestID);
        }
        if (logInfo)
            Debug.Log("Sending data to server: " + data);

        if (storeResult) {
            responses[key] = null;
        }
        SendWebSocketMessage(data);
    }

    /// <summary>
    /// Sends data to server
    /// </summary>
    /// <param name="data"></param>
    private async void SendWebSocketMessage(string data) {
        //if (websocket.State == WebSocketState.Open) {
        if (websocket.IsAlive == true) {
            websocket.Send(data);
        }
    }

    public bool IsConnected() {
        return websocket?.IsAlive ?? false;
    }

    private void OnDestroy() {
        websocket.Close();
    }

}
