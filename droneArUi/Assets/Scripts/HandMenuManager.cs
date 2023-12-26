/*
 * Handmenu Manager - process handmenu button events
 * 
 * Author : Martin Kyjac (xkyjac00)
 */

public class HandMenuManager : Singleton<HandMenuManager> {

    public void ConnectToServer() {
        WebSocketManager2.Instance.ConnectToServer(WebSocketManager2.Instance.ServerHostname, WebSocketManager2.Instance.Port);
    }

    public void SetGPS() {
        if (WebSocketManager2.Instance.IsConnected()) {
            GPSManager.Instance.SetGPS();
        }
    }

    public void ReconnectToServer() {
        WebSocketManager2.Instance.ConnectToServer(WebSocketManager2.Instance.ServerHostname, WebSocketManager2.Instance.Port);
    }

}
