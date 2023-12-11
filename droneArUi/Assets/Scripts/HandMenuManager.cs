/*
 * Handmenu Manager - process handmenu button events
 * 
 * Author : Martin Kyjac (xkyjac00)
 */

public class HandMenuManager : Singleton<HandMenuManager> {

    public void ConnectToServer() {
        WebSocketManager.Instance.ConnectToServer(WebSocketManager.Instance.ServerHostname, WebSocketManager.Instance.Port);
    }

    public void SetGPS() {
        if (WebSocketManager.Instance.IsConnected()) {
            GPSManager.Instance.SetGPS();
        }
    }

    public void ReconnectToServer() {
        WebSocketManager.Instance.ReconnectToServer(WebSocketManager.Instance.ServerHostname, WebSocketManager.Instance.Port);
    }

}
