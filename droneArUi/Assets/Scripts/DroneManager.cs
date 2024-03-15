/*
 * DroneManager - parse message recieved from server and set drone params according its content
 * 
 * Author : Martin Kyjac (xkyjac00)
 *          Jakub Kom�rek
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

public class DroneManager : Singleton<DroneManager>
{

    public List<Drone> Drones = new List<Drone>();
    public Drone ControlledDrone=null;
    public AbstractMap Map;
    public GameObject DroneBoundingBox;
    public GameObject ControlledDroneGameObject;
    public static bool RunningInUnityEditor = Application.isEditor;

    private void Update()
    {
        // pokud nejsou přijata data o dronu do 15s maže se
        List<Drone> dronesToRemove = new List<Drone>();
        foreach (Drone drone in Drones)
        {
            if((DateTime.Now - drone.lastUpdate).TotalSeconds > 15)
            {
                dronesToRemove.Add(drone);
            }
        }
        foreach (Drone drone in dronesToRemove)
        {
            Drones.Remove(drone);
            if (ControlledDrone != null)
            {
                if (drone==ControlledDrone)
                {
                    ControlledDrone = null;
                    TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                    textToSpeechSyntetizer.say("Drone timeout.");
                }
            }
        }

        // pro jistotu
        if (ControlledDrone != null)
        {
            if ((DateTime.Now - ControlledDrone.lastUpdate).TotalSeconds > 15)
            {
                ControlledDrone = null;
                TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("Drone timeout.");
            }
        }
        if (ControlledDrone == null)
        {
            SetControlledDrone();
        }
    }

    public void AddDrone(DroneFlightData flightData)
    {
        Mapbox.Utils.Vector2d mapboxPosition = new Mapbox.Utils.Vector2d(flightData.Latitude, flightData.Longitude);
        Vector3 position3d = Map.GeoToWorldPosition(mapboxPosition, false);
        //float groundAltitude = MapController.Instance.Map.QueryElevationInUnityUnitsAt(MapController.Instance.Map.WorldToGeoPosition(position3d));
        position3d.y = (float)flightData.Altitude;

        GameObject BBox = Instantiate(DroneBoundingBox, position3d, Quaternion.identity);
        BBox.name = flightData.DroneId;
        Drone newDrone = new Drone(BBox, flightData);
        newDrone.FlightData = flightData;
        Drones.Add(newDrone);
        BBox.transform.SetParent(transform);
    }

    public void HandleReceivedDroneData(string data)
    {
        DroneFlightData flightData = null;
        try
        {
            flightData = JsonUtility.FromJson<DroneFlightData>(data);
        }
        catch(ArgumentException)
        {
            return;
        }
        
        // If drone sends its location as NaN set Lat and Log to coordinates set in mapbox as center
        if (double.IsNaN(flightData.Latitude) || double.IsNaN(flightData.Longitude))
        {
            var mapboxLatLong = Map.CenterLatitudeLongitude;
            flightData.Longitude = mapboxLatLong.y;
            flightData.Latitude = mapboxLatLong.x;
        }


        foreach (Drone drone in Drones)
        {
            // Drone is already present and instaciated, we found it, just update position
            if (drone.FlightData.DroneId == flightData.DroneId)
            {
                drone.UpdateDroneFlightData(flightData);
                return;
            }
        }
        // Drone is new one in the system, we need to instanciate it
        AddDrone(flightData);
    }

    public void HandleReceivedDroneData(DroneFlightDataNew flyData)
    {
        DroneFlightData flightData = new DroneFlightData(flyData);

        if (double.IsNaN(flightData.Latitude) || double.IsNaN(flightData.Longitude))
        {
            var mapboxLatLong = Map.CenterLatitudeLongitude;
            flightData.Longitude = mapboxLatLong.y;
            flightData.Latitude = mapboxLatLong.x;
        }

        //flightData.Latitude = 49.19003762516931;
        //flightData.Longitude=14.699571367309487;


        foreach (Drone drone in Drones)
        {
            // Drone is already present and instaciated, we found it, just update position
            if (drone.FlightData.DroneId == flightData.DroneId)
            {
                drone.UpdateDroneFlightData(flightData);
                return;
            }
        }
        // Drone is new one in the system, we need to instanciate it
        AddDrone(flightData);
    }

    /*public Drone GetDroneByID(string droneID)
    {
        foreach (Drone drone in Drones)
        {
            if (drone.FlightData.DroneId.StartsWith(droneID))
            {
                return drone;
            }
        }
        return null;
    }*/

    /// <summary>
    /// Checks for drone by droneID and sets it as controlled drone
    /// </summary>
    /// <param name="droneID">Drone ID</param>
    public void SetControlledDrone()
    {
        foreach (var drone in Drones)
        {
            if ( drone.FlightData.DroneId != "HoloLens2_Pilot")
            {
                drone.IsControlled = true;
                ControlledDrone = drone;
                drone.DroneGameObject.Destroy();
                drone.DroneGameObject = ControlledDroneGameObject;
                Debug.Log("Controled drone selected:" + drone.FlightData.DroneId);

                RTMPstreamPlayer.Instance.OnDroneConnected(drone.FlightData.DroneId);

                TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("Drone connected.");
            }
        }
    }
}

[Serializable]
public class DroneFlightData
{
    public string DroneId;
    public double Altitude;
    public double Latitude;
    public double Longitude;
    public double Pitch;
    public double Roll;
    public double Yaw;
    public double Compass;
    public double VelocityX;
    public double VelocityY;
    public double VelocityZ;

    public GimbalOrientation gimbalOrientation;
    public DroneFlightData()
    {
        DroneId = "unset";
        Altitude = 0;
        Latitude = 0;
        Longitude = 0;
        Pitch = 0;
        Roll = 0;
        Yaw = 0;
        Compass = 0;
    }

    public DroneFlightData(DroneFlightDataNew newFormat):this()
    {
        DroneId = newFormat.client_id;
        Altitude = newFormat.altitude;
        Latitude = newFormat.gps.latitude;
        Longitude = newFormat.gps.longitude;
        Pitch = newFormat.aircraft_orientation.pitch;
        Roll = newFormat.aircraft_orientation.roll;
        Yaw = newFormat.aircraft_orientation.yaw;
        Compass = newFormat.aircraft_orientation.compass;
        VelocityX = newFormat.aircraft_velocity.velocity_x;
        VelocityY = newFormat.aircraft_velocity.velocity_y;
        VelocityZ = newFormat.aircraft_velocity.velocity_z;
        gimbalOrientation = newFormat.gimbal_orientation;
    }


    public void SetData(double height, double latitude, double longitute, double pitch, double roll, double yaw, double compass)
    {
        Altitude = height;
        Latitude = latitude;
        Longitude = longitute;
        Pitch = pitch;
        Roll = roll;
        Yaw = yaw;
        Compass = compass;
    }
}

