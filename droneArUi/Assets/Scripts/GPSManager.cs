/*
 * GPS manager - centering map and rotation calibration
 * 
 * Author : Martin Kyjac (xkyjac00)
 */

using System;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class GPSManager : Singleton<GPSManager>
{

    public AbstractMap Map;
    public Transform Camera;

    public static float droneUpdateInterval = 0.1f;

    private Drone drone;
    private float nextUpdate = 0f;

    private bool connectedToServer = false;

    private void Start()
    {
        //WebSocketManager2.Instance.OnConnectedToServer += OnConnectedToServer;

        // Generate Unique ID for our drone
        drone = new Drone(Camera.gameObject, new DroneFlightData());
        drone.FlightData.DroneId = "HoloLens2_Pilot";
        DroneManager.Instance.Drones.Add(drone);
    }

    private void OnConnectedToServer(object sender, EventArgs e)
    {
        connectedToServer = true;
    }

    private void Update()
    {
        if (connectedToServer)
        {
            if (nextUpdate > droneUpdateInterval)
            {
                nextUpdate = 0f;
                Vector2d latitudelongitude = Map.WorldToGeoPosition(Camera.position);
                float groundAltitude = Map.QueryElevationInUnityUnitsAt(latitudelongitude);
                //float groundAltitude = Camera.localPosition.y;
                drone.FlightData.SetData(groundAltitude + UserProfileManager.Instance.Height - 0.15, latitudelongitude.x, latitudelongitude.y, pitch: Camera.eulerAngles.x, roll: Camera.eulerAngles.z, yaw: Camera.eulerAngles.y, 0);
                //WebSocketManager.Instance.SendDataToServer(JsonUtility.ToJson(drone.FlightData));
            }
            nextUpdate += Time.deltaTime;
        }
    }


    public void SetGPS()
    {
        Drone djiDrone = DroneManager.Instance.ControlledDrone;
        if (djiDrone != null)
        {
            SetCameraPositionToDrone(djiDrone);
        }
        else if (DroneManager.Instance.Drones.Count > 0)
        {
            foreach (Drone drone in DroneManager.Instance.Drones)
            {
                if (!drone.FlightData.DroneId.StartsWith("HoloLens2"))
                {
                    djiDrone = drone;
                    break;
                }
            }

            SetCameraPositionToDrone(djiDrone);
        }
        
        //MissionManager.Instance.GenerateWaypoints();
        //MissionManager.Instance.StartMission();
    }

    private void SetCameraPositionToDrone(Drone drone)
    {
        // update map center position
        Map.UpdateMap(new Vector2d(drone.FlightData.Latitude, drone.FlightData.Longitude));
        Map.transform.eulerAngles = new Vector3(0, (float)-drone.FlightData.Yaw, 0);
        drone.DroneGameObject.transform.position = Camera.transform.position;
        drone.RotationOffset = drone.FlightData.Yaw;
        drone.ClearPositionOffset();
    }
}
