// autor jakub komárek

using JetBrains.Annotations;
using Mapbox.Unity.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering;
using static SpawnOnMap;

public class MapData : Singleton <MapData>
{
    // Start is called before the first frame update
    // uložená trasa
    [SerializeField]
    public List<Waypoint> _planedRoute = new List<Waypoint>();

    // objekty zájmu - pokud jsou nìjaké
    [SerializeField]
    public List<ObjOfInterest> _objOfInterest = new List<ObjOfInterest>();

    [SerializeField]
    public List<ObjOfInterest> _otherObjects = new List<ObjOfInterest>();

    private DroneManager droneManger = null;

    private DroneObject droneObj = null;

    void Start()
    {
        droneManger = FindObjectOfType<DroneManager>();

        droneObj = new DroneObject();
        droneObj.locationString = "49.22743926623377, 16.596966877183366";
        droneObj.relativeAltitude = 10;
        droneObj.name = "dron";
        droneObj.type = MapObject.ObjType.Drone;
    }

    // Update is called once per frame
    void Update()
    {
        if (droneManger.ControlledDrone != null)
        {
            if(droneObj.droneFlightData==null&&droneManger.ControlledDrone!=null)
            {
                droneObj.droneFlightData=droneManger.ControlledDrone.FlightData;
            }

            droneObj.locationString = string.Format("{0}, {1}", droneManger.ControlledDrone.FlightData.Latitude.ToString(CultureInfo.InvariantCulture),
                droneManger.ControlledDrone.FlightData.Longitude.ToString(CultureInfo.InvariantCulture));

            droneObj.name = droneManger.ControlledDrone.FlightData.DroneId;
            droneObj.relativeAltitude = (float)droneManger.ControlledDrone.FlightData.Altitude;
            /*droneObj.spawnetGameObject.transform.rotation = new Quaternion(
                (float)droneManger.ControlledDrone.FlightData.Roll,
                (float)droneManger.ControlledDrone.FlightData.Yaw,

                (float)droneManger.ControlledDrone.FlightData.Pitch,
                1f
            );*/
        }
    }



}

[Serializable]
public class Waypoint : MapObject
{
    Waypoint()
    {
        type = ObjType.Waypoint;
    }

    double heading;
    double radius;

    public ObjOfInterest objOfInterest = null;

}

[Serializable]
public class ObjOfInterest : MapObject
{
    public ObjOfInterest()
    {
        type = ObjType.ObjOfInterest;
    }
}

[Serializable]
public class DroneObject : MapObject
{
    public DroneFlightData droneFlightData = null;
    public DroneObject()
    {
        type = ObjType.Drone;
    }
}


// data pro uložení 
[Serializable]
public class MapObject
{
    public enum ObjType
    {
        Waypoint,
        LandingPad,
        Player,
        Drone,
        PowerLine,
        Barier,
        Unspecified,
        ObjOfInterest
    }

    [Geocode]
    public string locationString;

    public string name;

    public float relativeAltitude = 10f;

    public ObjType type = ObjType.Unspecified;


}

