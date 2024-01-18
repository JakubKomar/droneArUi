/*
 * Drone - class to store all drone data necessary to rendering
 * 
 * Author : Martin Kyjac (xkyjac00)
 */

using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Drone {

    public GameObject DroneGameObject {
        get; set;
    }

    public DroneFlightData FlightData {
        get; set;
    }

    public bool IsControlled {
        get; set;
    }

    private Vector3 CorrectionOffset {
        get; set;
    }

    public double RotationOffset { 
        get; set;
    }

    private readonly IEnumerable<string> DronesWithZeroAltitude = new List<string> { "DJI-Mavic2", "DJI-MAVIC_PRO", "DJI-MAVIC_MINI" };

    public void ClearPositionOffset() => CorrectionOffset = Vector3.zero;

    public float RelativeAltitude {
        get
        {
            return (float)FlightData.Altitude;
            if (this.DronesWithZeroAltitude.Contains(FlightData.DroneId))
            {
                return (float)FlightData.Altitude;
            }

            var latlong = new Vector2d(FlightData.Latitude, FlightData.Longitude);
            var groundAltitude = DroneManager.Instance.Map.QueryElevationInUnityUnitsAt(latlong);
            return (float)FlightData.Altitude - groundAltitude;
        }
    }

    public Drone(GameObject droneGameObject, DroneFlightData flightData, bool isControlled=false, float rotationOffset=0) {
        DroneGameObject = droneGameObject;
        FlightData = flightData;
        IsControlled = isControlled;
        RotationOffset = rotationOffset;
    }

    /// <summary>
    /// Update drone position, rotation and Head up display data according
    /// to data recieved from server.
    /// </summary>
    /// <param name="flightData"></param>
    public void UpdateDroneFlightData(DroneFlightData flightData) {
        var trackingType = UserProfileManager.Instance.TrackingType;

        if (trackingType == TrackingTypeEnum.GPS)
        {
            UpdatePositionByGPS(flightData);
        }
        else
        {
            UpdatePositionByIMU(flightData);
        }
    }

    /// <summary>
    /// Update drone position by x,y,z velocities provided in
    /// North-East-Down coordinate system. 
    /// </summary>
    /// <param name="flightData"></param>
    private void UpdatePositionByIMU(DroneFlightData flightData)
    {
        FlightData = flightData;
        var height = GetRelativeAltitude();
        var correction = UserProfileManager.Instance.DroneThreshold;
        var newVector = Rotate2DVector(new Vector2((float)flightData.VelocityX, (float)flightData.VelocityY), RotationOffset) * GPSManager.droneUpdateInterval;
        newVector *= (float)(1 + correction);
        var dronePosition = DroneGameObject.transform.position;
        var newPosition = dronePosition + new Vector3(newVector.y, 0, newVector.x);

        DroneGameObject.transform.position = new Vector3(newPosition.x, height, newPosition.z);
        SetRotation();
    }

    /// <summary>
    /// Update position by GPS data recieved from Drone.
    /// Using mapbox, GPS are transformed into Unity coordinates and then
    /// rotated by RotationOffset to align coordinates with camera
    /// </summary>
    /// <param name="flightData"></param>
    private void UpdatePositionByGPS(DroneFlightData flightData)
    {
        FlightData = flightData;
        if (double.IsNaN(FlightData.Latitude) || double.IsNaN(FlightData.Longitude))
        {
            return;
        }

        var newRealPos = DroneManager.Instance.Map.GeoToWorldPosition(new Vector2d(flightData.Latitude, flightData.Longitude), false);
        var position3d = newRealPos;

        position3d.y = GetRelativeAltitude();

        if (CheckThreshold(flightData, position3d.y))
        {
            DroneGameObject.transform.position = position3d + CorrectionOffset;
        }
        else
        {
            CorrectionOffset = DroneGameObject.transform.position - position3d;
        }

        var dronePosition = DroneGameObject.transform.position;
        DroneGameObject.transform.position = new Vector3(dronePosition.x, position3d.y, dronePosition.z);
        SetRotation();
    }

    /// <summary>
    /// Rotate Drone game object by FlightData values (included RotationOffset)
    /// </summary>
    private void SetRotation()
    {
        DroneGameObject.transform.localEulerAngles = new Vector3((float)-FlightData.Pitch, (float)(FlightData.Yaw - RotationOffset), (float)-FlightData.Roll);
    }

    /// <summary>
    /// Check if altitude is higher than 0.01m and 
    /// Roll + Pitch value is higher than threshold value in settings
    /// </summary>
    /// <param name="flightData"></param>
    /// <param name="altitude"></param>
    /// <returns></returns>
    private bool CheckThreshold(DroneFlightData flightData, float altitude)
    {
        return (UserProfileManager.Instance.DroneThreshold == 0 || Math.Abs(flightData.Pitch) + Math.Abs(flightData.Roll) > (double)UserProfileManager.Instance.DroneThreshold) && 
            altitude > 0.01 - UserProfileManager.Instance.Height;
    }

    /// <summary>
    /// Get new position rotated by RotationOffset angle that was set when Set GPS btn was pressed
    /// </summary>
    /// <param name="newPos">New position from GPS without rotation offset</param>
    /// <returns></returns>
    private Vector3 GetRotatedPosition(Vector3 newPos)
    {
        var rotated = Rotate2DVector(new Vector2(newPos.x, newPos.z), -RotationOffset);
        var rotated3D = new Vector3(rotated.x, 0, rotated.y);
        return rotated3D;
    }

    /// <summary>
    /// Rotate 2D vector by degrees given around 0,0 point in the counter clockwise direction
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="degrees"></param>
    /// <returns></returns>
    private Vector2 Rotate2DVector(Vector2 vector, double degrees)
    {
        Vector2 result = new Vector2();
        var radians = (Math.PI / 180) * degrees;
        result.x = (float)Math.Round(vector.x * Math.Cos(radians) + vector.y * Math.Sin(radians), 8);
        result.y = (float)Math.Round(vector.y * Math.Cos(radians) - vector.x * Math.Sin(radians), 8);
        return result;
    }

    /// <summary>
    /// Get relative altitude by FlightData property.
    /// If drone ID is not in DronesWithZeroAltitude, relative altitude
    /// is calculated via MapBox. Then is the value substracted by user height because
    /// when app starts, camera coordinates are 0,0,0 but user is in height cca 1.75m, so
    /// this value must be substracted from relative altitude to display drone correctly.
    /// </summary>
    /// <returns></returns>
    private float GetRelativeAltitude()
    {
        //float alt = 0f;
        return (float)FlightData.Altitude;

        /*
        if (DronesWithZeroAltitude.Contains(FlightData.DroneId))
        {
            alt = (float)FlightData.Altitude;
        }
        else
        {
            // ground altitude has to be calculated from camera location
            float groundAltitude = DroneManager.Instance.Map.QueryElevationInUnityUnitsAt(DroneManager.Instance.Map.WorldToGeoPosition(GPSManager.Instance.Camera.position));
            alt = (float)FlightData.Altitude - groundAltitude;
        }

        return alt - UserProfileManager.Instance.Height;*/
    }
}
