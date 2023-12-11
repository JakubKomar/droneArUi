using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System;
using System.Text;

public class WebSocketManager2 : Singleton<WebSocketManager>
{
    /// <summary>
    /// Drone Server URI
    /// </summary>
    private string APIDomainWS = "";
    /// <summary>
    /// Websocket context
    /// </summary>
    /// 

    private WebSocket myWebsocket;



    private string ClientID;
    private bool handshake_done = false;

    private bool droneRequestSent = false;

    [Serializable]
    private class Response<T>
    {
        public string type;
        public T data;
    }

    [Serializable]
    private class HandshakeResponseData
    {
        public string ClientID;
        public int rtmp_port;
    }

    private void Update()
    {
        if (myWebsocket != null && myWebsocket.State == WebSocketState.Open)
            myWebsocket.DispatchMessageQueue();
    }

    public async void ConnectToServer(string domain, int port)
    {
        ClosePreviousConnection();

        try
        {
            APIDomainWS = GetWSURI(domain, port);
            myWebsocket = new WebSocket(APIDomainWS);

            myWebsocket.OnOpen += OnConnected;
            myWebsocket.OnError += OnError;
            myWebsocket.OnClose += OnClose;
            myWebsocket.OnMessage += HandleReceivedData;

            await myWebsocket.Connect();
        }
        catch (UriFormatException ex)
        {
            Debug.LogError(ex);
        }
    }

    private void ClosePreviousConnection()
    {
        if (myWebsocket != null && myWebsocket.State == WebSocketState.Open)
        {
            myWebsocket.CancelConnection();
        }
    }

    public async void SendToServer(string msg)
    {
        if (myWebsocket != null)
        {
            try
            {
                await myWebsocket.SendText(msg);
            }
            catch (WebSocketException ex)
            {
                Debug.LogError(ex);
            }
        }
    }

    public void SendDroneListRequest()
    {
        if (!droneRequestSent)
        {
            Debug.Log("Sending drone list request.");
            SendToServer("{\"type\":\"drone_list\"}");
            droneRequestSent = true;
            StartCoroutine(RequestTimeout());
        }
    }

    private IEnumerator RequestTimeout()
    {
        yield return new WaitForSeconds(10f);
        droneRequestSent = false;
    }

    public void SendCarDetectionRequest(string clientID, bool run = true)
    {
        Debug.Log("{\"type\":\"vehicle_detection_set\", \"data\":{\"drone_stream_id\":\"" + clientID + "\", \"state\":" + run.ToString().ToLower() + "}}");
        SendToServer("{\"type\":\"vehicle_detection_set\", \"data\":{\"drone_stream_id\":\"" + clientID + "\", \"state\":" + run.ToString().ToLower() + "}}");
    }

    private void HandleReceivedData(byte[] message)
    {
        string msgstr = Encoding.Default.GetString(message);
        Response<string> msg = JsonUtility.FromJson<Response<string>>(msgstr);

        if (!handshake_done && msg.type == "hello_resp")
        {
            Response<HandshakeResponseData> hr = JsonUtility.FromJson<Response<HandshakeResponseData>>(msgstr);
            if (hr != null)
            {
                ClientID = hr.data.ClientID;
                handshake_done = true;
                Debug.Log("Handshake successful.");
            }
        }
        else if (handshake_done && msg.type == "drone_list_resp")
        {
            Response<DroneFlightData2[]> dsdr = JsonUtility.FromJson<Response<DroneFlightData2[]>>(msgstr);
            Debug.Log(msgstr);
            //DroneManager.Instance.HandleReceivedDroneList(dsdr.data);
            droneRequestSent = false;
        }
        else if (handshake_done && msg.type == "data_broadcast")
        {
            Response<DroneFlightData> dsfdr = JsonUtility.FromJson<Response<DroneFlightData>>(msgstr);
            //DroneManager.Instance.HandleReceivedDroneData(dsfdr.data);
        }
        else if (handshake_done && msg.type == "vehicle_detection_rects")
        {
            Response<DroneFlightData2> dsvdr = JsonUtility.FromJson<Response<DroneFlightData2>>(msgstr);
            //Debug.Log(msgstr);
            //DroneManager.Instance.HandleReceivedVehicleData(dsvdr.data);
        }
        else if (!handshake_done && msg.type == "data_broadcast")
        {
        }
        else
        {
            Debug.LogError("Unknown data received! " + msgstr);
        }
    }

    private void OnClose(WebSocketCloseCode closeCode)
    {
        Debug.Log("Connection closed!");
        handshake_done = false;
        //GameManager.Instance.HandleConnectionFailed();
    }

    private void OnError(string errorMsg)
    {
        Debug.LogError(errorMsg);
        handshake_done = false;
        //GameManager.Instance.HandleConnectionFailed();
    }

    private void OnConnected()
    {
        Debug.Log("Connected - sending handshake");
        SendToServer("{\"type\":\"hello\",\"data\":{\"ctype\":1}}");
    }

    /// <summary>
    /// Create websocket URI from domain name and port
    /// </summary>
    /// <param name="domain">Domain name or IP address</param>
    /// <param name="port">Server port</param>
    /// <returns></returns>
    public string GetWSURI(string domain, int port)
    {
        return "ws://" + domain + ":" + port.ToString();
    }

    private async void OnApplicationQuit()
    {
        await myWebsocket.Close();
    }

}