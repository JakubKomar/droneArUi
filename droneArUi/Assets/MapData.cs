// autor jakub komárek

using JetBrains.Annotations;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static SpawnOnMap;

public class MapData : Singleton <MapData>
{
    // Start is called before the first frame update
    // uložená trasa

    public List<Waypoint> _planedRoute = new List<Waypoint>();

    // objekty zájmu - pokud jsou nìjaké

    public List<ObjOfInterest> _objOfInterest = new List<ObjOfInterest>();

    public List<ObjOfInterest> _otherObjects = new List<ObjOfInterest>();

    public List<MapObjectSer> mapObjectSers = new List<MapObjectSer>();

    public DroneManager droneManger = null;

    public DroneObject droneObj = new DroneObject();

    //[HideInInspector]
    public List<MapObject> allObjects = new List<MapObject>();

    public List<SpawnOnMap> spawnOnMapScripts = new List<SpawnOnMap>();

     

    void Start()
    {
        droneManger = FindObjectOfType<DroneManager>();

        droneObj.locationString = "49.22743926623377, 16.596966877183366"; //default vals
        droneObj.relativeAltitude = 10;
        droneObj.name = "dron";
        droneObj.type = MapObject.ObjType.Drone;
        onObjectChanged();
    }

    public void onObjectChanged()
    {
        allObjects.Clear();
        allObjects.AddRange(_objOfInterest);
        allObjects.AddRange(_otherObjects);
        allObjects.Add(droneObj);
        allObjects.AddRange(mapObjectSers);

        foreach (var spawnOnMapScript in spawnOnMapScripts)
        {
            spawnOnMapScript.reCreateGameObjects();
        }
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

            droneObj.rotation = new Quaternion(
                (float)droneManger.ControlledDrone.FlightData.Roll,
                (float)droneManger.ControlledDrone.FlightData.Yaw,

                (float)droneManger.ControlledDrone.FlightData.Pitch,
                1f
            );
        }
    }



}


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

public class ObjOfInterest : MapObject
{
    public ObjOfInterest()
    {
        type = ObjType.ObjOfInterest;
    }
}

public class DroneObject : MapObject
{
    public DroneFlightData droneFlightData = null;
    public Quaternion rotation= Quaternion.identity;

    public DroneObject()
    {
        type = ObjType.Drone;
    }
}


// data pro uložení 
[Serializable]
public class MapObjectSer: MapObject { }

public class MapObject: System.Object
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

