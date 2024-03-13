// autor jakub komárek

using Mapbox.Unity.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using System.Text;

using Mapbox.Utils;

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
    public List<MapObject> mapObjectSers = new List<MapObject>();

    [HideInInspector]
    private DroneManager droneManger = null;

    [HideInInspector]
    private DroneObject droneObj = new DroneObject();


    public MapObject homeLocation = new MapObject();

    [HideInInspector]
    private Player player = new Player();

    [HideInInspector]
    public List<MapObject> allObjects = new List<MapObject>();

    [HideInInspector]
    public List<SpawnOnMap> spawnOnMapScripts = new List<SpawnOnMap>();

    public string misionName="test";

    private calibrationScript calibrationScript = null;

    void Start()
    {
        droneManger = FindObjectOfType<DroneManager>();

        droneObj.locationString = "49.22743926623377, 16.596966877183366"; //default vals
        droneObj.relativeAltitude = 10;
        droneObj.name = "dron";
        droneObj.type = MapObject.ObjType.Drone;

        calibrationScript = FindObjectOfType<calibrationScript>();
        homeLocation.name = "home";
        homeLocation.relativeAltitude = 0;
        homeLocation.type = MapObject.ObjType.LandingPad;

        onObjectChanged();
    }

    // Update is called once per frame
    void Update()
    {
        // update umístìní drona 
        if (droneManger.ControlledDrone != null)
        {
            if (droneObj.droneFlightData == null && droneManger.ControlledDrone != null)
            {
                droneObj.droneFlightData = droneManger.ControlledDrone.FlightData;
            }

            droneObj.locationString = string.Format("{0}, {1}", droneManger.ControlledDrone.FlightData.Latitude.ToString(CultureInfo.InvariantCulture),
                droneManger.ControlledDrone.FlightData.Longitude.ToString(CultureInfo.InvariantCulture));

            droneObj.name = droneManger.ControlledDrone.FlightData.DroneId;
            droneObj.relativeAltitude = (float)droneManger.ControlledDrone.FlightData.Altitude;
            
            //nastavení rotace drona
            droneObj.rotation = new Quaternion(
                (float)droneManger.ControlledDrone.FlightData.Roll,
                (float)droneManger.ControlledDrone.FlightData.Yaw,

                (float)droneManger.ControlledDrone.FlightData.Pitch,
                1f
            );
        }

        // update domovské pozice
        homeLocation.locationString= string.Format(NumberFormatInfo.InvariantInfo, "{0}, {1}", calibrationScript.playerPosition.x, calibrationScript.playerPosition.y);

        // update pozice hráèe
        player.locationString= string.Format(NumberFormatInfo.InvariantInfo, "{0}, {1}", calibrationScript.playerGps.x, calibrationScript.playerGps.y);
        player.heading = calibrationScript.playerHading;

    }


    public void saveMision()
    {
        JsonFileTdo jsonFileTdo = new JsonFileTdo();
        jsonFileTdo.misionName = misionName;
        jsonFileTdo._planedRoute = _planedRoute;
        jsonFileTdo._objOfInterest = _objOfInterest;

        jsonFileTdo._otherObjects= _otherObjects;
        jsonFileTdo._otherObjects.AddRange(mapObjectSers);
        jsonFileTdo.homeLocation= homeLocation;
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

                if (jsonFileTdo.homeLocation != null)
                    calibrationScript.setHomeLocation(jsonFileTdo.homeLocation.locationString);

                misionName = jsonFileTdo.misionName;
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
        onObjectChanged();
    }

    public void loadCsvMision(string name)
    {
        JsonFileTdo jsonFileTdo =new JsonFileTdo();
        jsonFileTdo.loadCsv(name);
        _planedRoute = jsonFileTdo._planedRoute;
        _objOfInterest = jsonFileTdo._objOfInterest;
        _otherObjects = jsonFileTdo._otherObjects;

        misionName = jsonFileTdo.misionName;

        if(_planedRoute == null && _planedRoute.Count < 0) // domov je nastaven pod první waypoint - informace v csv formátu chybí
        {
            calibrationScript.setHomeLocation(_planedRoute[0].locationString);
        }

        onObjectChanged();
    }


    public void onObjectChanged()
    {
        allObjects.Clear();
        allObjects.AddRange(_objOfInterest);
        allObjects.AddRange(_otherObjects);
        allObjects.AddRange(mapObjectSers); // test purpeses - from unity editor

        allObjects.Add(droneObj);
        allObjects.Add(player);
        allObjects.Add(homeLocation);


        foreach (var spawnOnMapScript in spawnOnMapScripts)
        {
            try
            {
                spawnOnMapScript.reCreateGameObjects();
            }catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public void test()
    {
        _planedRoute.Clear();
        _objOfInterest.Clear();
        _otherObjects.Clear();

        Waypoint waypoint1=new Waypoint();
        waypoint1.locationString = "49.227654814174905, 16.597106088572705";
        waypoint1.relativeAltitude = 10;
        _planedRoute.Add(waypoint1);

        Waypoint waypoint2=new Waypoint();
        waypoint2.locationString = "49.22753381250514, 16.596691921462376";
        waypoint2.relativeAltitude = 20;
        _planedRoute.Add(waypoint2);

        Waypoint waypoint3=new Waypoint();
        waypoint3.locationString = "49.22714707969114, 16.596684655372727";
        waypoint3.relativeAltitude = 30;
        _planedRoute.Add(waypoint3);

        Waypoint waypoint4=new Waypoint();
        waypoint4.locationString = "49.2272752000998, 16.597385833024425";
        waypoint4.relativeAltitude = 20;
        _planedRoute.Add(waypoint4);
        onObjectChanged();

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

    [JsonProperty("homeLocation")]
    public MapObject homeLocation =null;

    public void save()
    {
        saveJson();
        saveCsv();
    }

    private void checkDir()
    {
        if (!Directory.Exists("misions/"))
        {
            // If not, create the directory
            Directory.CreateDirectory("misions/");
        }
    }

    public void saveJson() { // internal format
        string path = "misions/" + misionName + ".json";
        try
        {
            checkDir();
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error saving CSV file: " + ex.Message);
        }
    }

    public void saveCsv() //lichi kompatible format
    {
        checkDir();
        string path = "misions/" + misionName + ".csv";
        try
        {
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                // header
                CultureInfo culture = new CultureInfo("en-US");
                sw.WriteLine("latitude,longitude,altitude(m),heading(deg),curvesize(m),rotationdir,gimbalmode,gimbalpitchangle,actiontype1,actionparam1,actiontype2,actionparam2,actiontype3,actionparam3,actiontype4,actionparam4,actiontype5,actionparam5,actiontype6,actionparam6,actiontype7,actionparam7,actiontype8,actionparam8,actiontype9,actionparam9,actiontype10,actionparam10,actiontype11,actionparam11,actiontype12,actionparam12,actiontype13,actionparam13,actiontype14,actionparam14,actiontype15,actionparam15,altitudemode,speed(m/s),poi_latitude,poi_longitude,poi_altitude(m),poi_altitudemode,photo_timeinterval,photo_distinterval");

                // jednotlivé waypointy
                foreach (var waypoint in _planedRoute)
                {
                    Vector2d vector2D = Conversions.StringToLatLon(waypoint.locationString);

                    sw.WriteLine($"{vector2D.x.ToString(culture)},{vector2D.y.ToString(culture)},{waypoint.relativeAltitude.ToString(culture)},0,3,0,0,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,0,0,0,0,0,0,-1,-1");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error saving CSV file: " + ex.Message);
        }
    }

    public void loadCsv(string misionName)
    {
        string path = "misions/" + misionName + ".csv";

        // clean up
        _planedRoute.Clear();
        _objOfInterest.Clear();
        _otherObjects.Clear();

        try
        {
            if (File.Exists(path))
            {
                this.misionName = misionName;
                string[] lines = File.ReadAllLines(path);

                // Skip the header row (assuming the first row contains column names)
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(',');

                    Waypoint waypoint = new Waypoint();
                    waypoint.locationString = string.Format("{0}, {1}", values[0], values[1]);

                    if (float.TryParse(values[2], out float result))
                    {
                        waypoint.relativeAltitude = result;
                    }
                    else
                    {
                        throw new Exception("converzion error");
                    }
                    this._planedRoute.Add(waypoint);
                }
            }
            else
            {
                Debug.LogWarning("File not found: " + path);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error reading CSV file: " + ex.Message);
        }
    }
}


[Serializable]
public class Player : MapObject
{
    public Player()
    {
        type = ObjType.Player;
    }

    public float heading=0;
}

[Serializable]
public class Waypoint : MapObject
{
    public Waypoint()
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


[Serializable]
public class MapObject: System.Object
{

    [Geocode]
    public string locationString; // gps pozice

    public string name; // zobrazovaný název

    public float relativeAltitude = 0f; // výška nad zemí 

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
    public ObjType type = ObjType.Unspecified;

}

