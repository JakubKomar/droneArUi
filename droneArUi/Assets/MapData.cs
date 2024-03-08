// autor jakub komárek

using JetBrains.Annotations;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;

using static SpawnOnMap;

public class MapData : Singleton <MapData>
{
    // Start is called before the first frame update
    // uložená trasa
    [HideInInspector]
    public List<Waypoint> _planedRoute = new List<Waypoint>();

    // objekty zájmu - pokud jsou nìjaké
    [HideInInspector]
    private List<ObjOfInterest> _objOfInterest = new List<ObjOfInterest>();
    [HideInInspector]
    private List<MapObject> _otherObjects = new List<MapObject>();

    [SerializeField]
    public List<MapObjectSer> mapObjectSers = new List<MapObjectSer>();

    [HideInInspector]
    private DroneManager droneManger = null;

    [HideInInspector]
    private DroneObject droneObj = new DroneObject();

    [HideInInspector]
    public List<MapObject> allObjects = new List<MapObject>();

    [HideInInspector]
    public List<SpawnOnMap> spawnOnMapScripts = new List<SpawnOnMap>();

    public string misionName="test";
    public void saveMision()
    {
        JsonFileTdo jsonFileTdo = new JsonFileTdo();
        jsonFileTdo.misionName = misionName;
        jsonFileTdo._planedRoute = _planedRoute;
        jsonFileTdo._objOfInterest = _objOfInterest;

        jsonFileTdo._otherObjects= _otherObjects;
        jsonFileTdo._otherObjects.AddRange(mapObjectSers);
        jsonFileTdo.save();
    }

    public void loadMision(string name) {
        string path="misions/"+name+".json";

        try
        {
            if (File.Exists(path))
            {
                string jsonContent = File.ReadAllText(path);

                JsonFileTdo jsonFileTdo = JsonConvert.DeserializeObject<JsonFileTdo>(jsonContent);

                _planedRoute= jsonFileTdo._planedRoute;
                _objOfInterest= jsonFileTdo._objOfInterest;
                _otherObjects =jsonFileTdo._otherObjects;
              
                misionName = jsonFileTdo.misionName;
                onObjectChanged();
            }
            else
            {
                Debug.LogWarning("File not found: " + path);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error reading or deserializing the file: " + ex.Message);
        }
    }


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

[Serializable]
public class JsonFileTdo : System.Object
{
    [JsonProperty("mision_name")]
    public string misionName="";

    [JsonProperty("dronePath")]
    public List<Waypoint> _planedRoute = new List<Waypoint>();

    [JsonProperty("objOfInterest")]
    public List<ObjOfInterest> _objOfInterest = new List<ObjOfInterest>();

    [JsonProperty("mashObjects")]
    public List<MapObject> _otherObjects = new List<MapObject>();

    public void save()
    {
        if (!Directory.Exists("misions/"))
        {
            // If not, create the directory
            Directory.CreateDirectory("misions/");
        }

        string json = JsonConvert.SerializeObject(this);
        File.WriteAllText("misions/"+ misionName+".json", json);
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
    public Quaternion rotation= Quaternion.identity;

    public DroneObject()
    {
        type = ObjType.Drone;
    }
}


// data pro uložení 
[Serializable]
public class MapObjectSer: MapObject { }


[Serializable]
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

