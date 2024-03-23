// autor jakub kom�rek

using Mapbox.Unity.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using System.Text;

using Mapbox.Utils;
using UnityEngine.Events;
using TMPro.EditorUtilities;
using Unity.VisualScripting;

public class MapData : Singleton <MapData>
{
    // Start is called before the first frame update
    // ulo�en� trasa
    [HideInInspector]
    public List<Waypoint> _planedRoute = new List<Waypoint>();
    private int _planedRouteCurrentWaypoint = 0;

    // objekty z�jmu - pokud jsou n�jak�
    [HideInInspector]
    private List<ObjOfInterest> _objOfInterest = new List<ObjOfInterest>();
    [HideInInspector]
    private List<MapObject> _otherObjects = new List<MapObject>();

    [HideInInspector]
    private DroneManager droneManger = null;

    [HideInInspector]
    private DroneObject droneObj = null;


    public MapObject homeLocation = null;

    [HideInInspector]
    private Player player = null;

    [HideInInspector]
    public List<MapObject> allObjects = new List<MapObject>();

    [HideInInspector]
    public List<SpawnOnMap> spawnOnMapScripts = new List<SpawnOnMap>();

    public string misionName="";

    private calibrationScript calibrationScript = null;

    public string pathToDir;
    public string pathToDirCsvExport = "";
    

    public UnityEvent sevedFile =new UnityEvent();

    private bool droneInBarier=false;
    private bool droneInWarningBarier=false;

    private void Awake()
    {
    }
    void Start()
    {
        droneObj = new DroneObject(this);
        homeLocation = new MapObject(this);
        player = new Player(this);

        pathToDir = Path.Combine(Application.persistentDataPath, "misions/");
        pathToDirCsvExport = Path.Combine(Application.persistentDataPath, "misions/export_csv/");

        droneManger = FindObjectOfType<DroneManager>();
        calibrationScript = FindObjectOfType<calibrationScript>();

        droneObj.locationString = "49.22743926623377, 16.596966877183366"; //default vals
        droneObj.relativeAltitude = 10;
        droneObj.name = "dron";
        droneObj.type = MapObject.ObjType.Drone;

        homeLocation.name = "home";
        homeLocation.relativeAltitude = 0;
        homeLocation.type = MapObject.ObjType.LandingPad;

        _planedRouteCurrentWaypoint = 0;


        onObjectChanged();
    }

    // Update is called once per frame
    void Update()
    {
        // update um�st�n� drona 
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
            
            //nastaven� rotace drona
            droneObj.rotation = new Quaternion(
                (float)droneManger.ControlledDrone.FlightData.Roll,
                (float)droneManger.ControlledDrone.FlightData.Yaw,

                (float)droneManger.ControlledDrone.FlightData.Pitch,
                1f
            );
        }

        // update domovsk� pozice
        homeLocation.locationString= string.Format(NumberFormatInfo.InvariantInfo, "{0}, {1}", calibrationScript.playerPosition.x, calibrationScript.playerPosition.y);

        // update pozice hr��e
        player.locationString= string.Format(NumberFormatInfo.InvariantInfo, "{0}, {1}", calibrationScript.playerGps.x, calibrationScript.playerGps.y);
        player.heading = calibrationScript.playerHading;

    }

    public void onWaypointCrossed(int index,bool skip=false)
    {
        if (index == _planedRouteCurrentWaypoint)
        {
            if(_planedRoute.Count >index) {
                _planedRoute[index].setAsTarget = false;
                _planedRoute[index].hasBeenVisited = true;
            }
            _planedRouteCurrentWaypoint++;
            index++;
            if (_planedRoute.Count > index)
            {
                _planedRoute[index].setAsTarget = true;
                if (skip)
                {
                    TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                    textToSpeechSyntetizer.say("Waypoint skiped.");
                }
                else {
                    TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                    textToSpeechSyntetizer.say("Waypoint reached.");
                }

            }
            else
            {
                trackComplete();
            }
        }
    }

    public void onSkipWaypoint()
    {
        onWaypointCrossed(_planedRouteCurrentWaypoint,true);
    }

    public void onResetRoute()
    {
        _planedRouteCurrentWaypoint = 0;
        foreach (var route in _planedRoute)
        {
            route.onReset();
        }
        if (_planedRoute.Count > 0)
        {
            _planedRoute[0].setAsTarget=true;
        }
    }

    public void trackComplete()
    {
        TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
        textToSpeechSyntetizer.say("Flyplan completed.");

        onResetRoute();
    }

    public void droneEnterBarier(bool isWarning)
    {
        if(isWarning) {
            if (droneInWarningBarier == false)
            {
                TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("Drone enter warning zone.");
                droneInWarningBarier = true;
            }
        }
        else
        {
            if(droneInBarier==false)
            {
                TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("Drone enter danger zone.");
                droneInBarier = true;
            }
        }
    }
    public void droneLeaveBarier(bool isWarning)
    {
        if (isWarning)
        {
            if (droneInWarningBarier == true)
            {
                TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("Drone leave warning zone.");
                droneInWarningBarier = false;
            }
        }
        else
        {
            if (droneInBarier == true)
            {
                TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("Drone leave danger zone.");
                droneInBarier = false;
            }
        }
    }

    public void saveMisionAsNew()
    {
        misionName = "";
        saveMision();
    }
    public void saveMision()
    {
        if (!Directory.Exists(pathToDir))
        {
            Directory.CreateDirectory(pathToDir);
            Debug.Log("Created directory: " + pathToDir);
        }
        if (!Directory.Exists(pathToDirCsvExport))
        {
            Directory.CreateDirectory(pathToDirCsvExport);
            Debug.Log("Created directory: " + pathToDirCsvExport);
        }


        JsonFileTdo jsonFileTdo = new JsonFileTdo();
        if (misionName == "") {
            misionName= string.Format("mise-{0:yyMMdd-HHmmss}", DateTime.Now);
        }
        jsonFileTdo.misionName = misionName;


        jsonFileTdo._planedRoute = _planedRoute;
        jsonFileTdo._objOfInterest = _objOfInterest;

        jsonFileTdo._otherObjects= _otherObjects;
        jsonFileTdo.homeLocation= homeLocation;

        jsonFileTdo.saveJson(pathToDir);
        jsonFileTdo.saveCsv(pathToDirCsvExport);

        sevedFile.Invoke();
    }

    public void loadMision(string path) {
        try
        {
            if (File.Exists(path))
            {
                string jsonContent = File.ReadAllText(path);

                JsonFileTdo jsonFileTdo = JsonConvert.DeserializeObject<JsonFileTdo>(jsonContent);

                _planedRoute= jsonFileTdo._planedRoute;
                _objOfInterest= jsonFileTdo._objOfInterest;
                _otherObjects =jsonFileTdo._otherObjects;
                _planedRouteCurrentWaypoint = 0;

                if (jsonFileTdo.homeLocation != null)
                    calibrationScript.setHomeLocation(jsonFileTdo.homeLocation.locationString);

                misionName = jsonFileTdo.misionName;

                TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("Mission loaded successfully.");
            }
            else
            {
                TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("File not found.");
            }
        }
        catch (Exception ex)
        {
            TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
            textToSpeechSyntetizer.say("File corupted:"+ex);
        }
        onObjectChanged();
    }


    public void newMission()
    {
        misionName = string.Format("mise-{0:yyMMdd-HHmmss}", DateTime.Now);
        _planedRoute.Clear();
        _objOfInterest.Clear();
        _otherObjects.Clear();
        onObjectChanged();

        TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
        textToSpeechSyntetizer.say("New mission created.");
    }

    public void loadCsvMision(string path,string name)
    {
        JsonFileTdo jsonFileTdo =new JsonFileTdo();
        jsonFileTdo.loadCsv(path,name,this);

        _planedRoute = jsonFileTdo._planedRoute;
        _objOfInterest = jsonFileTdo._objOfInterest;
        _otherObjects = jsonFileTdo._otherObjects;

        misionName = jsonFileTdo.misionName;

        if(_planedRoute.Count > 0) // domov je nastaven pod prvn� waypoint - informace v csv form�tu chyb�
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

        allObjects.Add(droneObj);
        allObjects.Add(player);
        allObjects.Add(homeLocation);

        int index = 0;
        if(_planedRouteCurrentWaypoint>= _planedRoute.Count)
        {
            onResetRoute();
        }

        foreach (var obj in _planedRoute)
        {
            obj.onReset();
            obj.setAsTarget = index == _planedRouteCurrentWaypoint;
            obj.pos= index;
            index++;
        }

        index = 0;
        foreach (var obj in _objOfInterest)
        {
            obj.name = index.ToString();
            index++;
        }


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

    public void spawnedObjectDeletion(List<MapObject> list)
    {
        foreach (var item in list) { 
            if (item is Waypoint)
                _planedRoute.Remove((Waypoint)item);
            if (item is ObjOfInterest)
                _objOfInterest.Remove((ObjOfInterest)item);
            _otherObjects.Remove(item);
        }
        if(list.Count > 0) {
            onObjectChanged();
        }
       
    }

    public void test()
    {
        _planedRoute.Clear();
        _objOfInterest.Clear();
        _otherObjects.Clear();
        Waypoint waypoint0 = new Waypoint(this);
        waypoint0.locationString = "49.22732,16.59683";
        waypoint0.relativeAltitude = 0;
        _planedRoute.Add(waypoint0);
        Waypoint waypoint1=new Waypoint(this);
        waypoint1.locationString = "49.22732,16.59683";
        waypoint1.relativeAltitude = 1;
        _planedRoute.Add(waypoint1);

        Waypoint waypoint2=new Waypoint(this);
        waypoint2.locationString = "49.22732,16.59683";
        waypoint2.relativeAltitude = 2;
        _planedRoute.Add(waypoint2);

        Waypoint waypoint5 = new Waypoint(this);
        waypoint5.locationString = "49.22732,16.59683";
        waypoint5.relativeAltitude = 2;
        _planedRoute.Add(waypoint5);

        Waypoint waypoint6 = new Waypoint(this);
        waypoint6.locationString = "49.22732,16.59683";
        waypoint6.relativeAltitude = 3;
        _planedRoute.Add(waypoint6);

        Waypoint waypoint7 = new Waypoint(this);
        waypoint7.locationString = "49.22732,16.59683";
        waypoint7.relativeAltitude = 4;
        _planedRoute.Add(waypoint7);

        Waypoint waypoint3=new Waypoint(this);
        waypoint3.locationString = "49.22714707969114, 16.596684655372727";
        waypoint3.relativeAltitude = 3;
        _planedRoute.Add(waypoint3);

        Waypoint waypoint4=new Waypoint(this);
        waypoint4.locationString = "49.2272752000998, 16.597385833024425";
        waypoint4.relativeAltitude = 4;
        _planedRoute.Add(waypoint4);
        onObjectChanged();
    }
    public void addObject(MapObject NewObject) {
        switch(NewObject.type){
            case MapObject.ObjType.Waypoint:
                Waypoint newWaypoint = new Waypoint(this,NewObject);
                _planedRoute.Add(newWaypoint);
                onObjectChanged();
                break;
            case MapObject.ObjType.ObjOfInterest:
                ObjOfInterest newObjOfInterest = new ObjOfInterest(this,NewObject);
                _objOfInterest.Add(newObjOfInterest);
                onObjectChanged();
                break;          
            case MapObject.ObjType.Barier:
                Barier barier = new Barier(this, NewObject);
                _otherObjects.Add(barier);
                onObjectChanged();
                break;
            case MapObject.ObjType.Warning:
                Warning warning = new Warning(this, NewObject);
                _otherObjects.Add(warning);
                onObjectChanged();
                break;
            case MapObject.ObjType.LandingPad:
            default:
                Debug.Log("NewObject creation of:" + NewObject.type.ToString()+"not Suported");
                break;
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

    [JsonProperty("homeLocation")]
    public MapObject homeLocation =null;

    public void saveJson(string dirPath) { // internal format
        try
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(dirPath + "/"+this.misionName+".json", json);
            TextToSpeechSyntetizer textToSpeechSyntetizer = GameObject.FindObjectOfType<TextToSpeechSyntetizer>();
            textToSpeechSyntetizer.say("Saving successfull.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error saving CSV file: " + ex.Message);
            TextToSpeechSyntetizer textToSpeechSyntetizer = GameObject.FindObjectOfType<TextToSpeechSyntetizer>();
            textToSpeechSyntetizer.say("Saving json failed.");
        }
    }

    double calcDistance(Vector2d firstPos,Vector2d secondPos)
    {
        return Math.Abs(firstPos.x - secondPos.x) + Math.Abs(firstPos.y - secondPos.y);
    }


    public void saveCsv(string dirPath) //lichi kompatible format
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(dirPath + "/" + this.misionName + ".csv", false, Encoding.UTF8))
            {
                // header
                CultureInfo culture = new CultureInfo("en-US");
                sw.WriteLine("latitude,longitude,altitude(m),heading(deg),curvesize(m),rotationdir,gimbalmode,gimbalpitchangle,actiontype1,actionparam1,actiontype2,actionparam2,actiontype3,actionparam3,actiontype4,actionparam4,actiontype5,actionparam5,actiontype6,actionparam6,actiontype7,actionparam7,actiontype8,actionparam8,actiontype9,actionparam9,actiontype10,actionparam10,actiontype11,actionparam11,actiontype12,actionparam12,actiontype13,actionparam13,actiontype14,actionparam14,actiontype15,actionparam15,altitudemode,speed(m/s),poi_latitude,poi_longitude,poi_altitude(m),poi_altitudemode,photo_timeinterval,photo_distinterval");

                // jednotliv� waypointy
                foreach (var waypoint in _planedRoute)
                {
                    Vector2d waypointVec = Conversions.StringToLatLon(waypoint.locationString);

                    ObjOfInterest poi = null;

                    // najdu nejbli��� poi k waypountu
                    foreach (var itrPoi in _objOfInterest)
                    {
                        if (poi == null) { poi = itrPoi; continue; }

                        Vector2d poiVector = Conversions.StringToLatLon(itrPoi.locationString);

                        if (calcDistance(waypointVec, poiVector) < calcDistance(waypointVec, Conversions.StringToLatLon(poi.locationString))) {
                            poi = itrPoi;
                        }
                    }

                    float poiX= poi==null?0: (float)Conversions.StringToLatLon(poi.locationString).x;
                    float poiY = poi == null ? 0 : (float)Conversions.StringToLatLon(poi.locationString).y;
                    float poiAlt = poi == null ? 0 : poi.relativeAltitude;
                    sw.WriteLine($"{waypointVec.x.ToString(culture)},{waypointVec.y.ToString(culture)},{waypoint.relativeAltitude.ToString(culture)},0,3,0,0,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,-1,0,0,0,{poiX.ToString(culture)},{poiY.ToString(culture)},{poiAlt.ToString(culture)},0,-1,-1");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error saving CSV file: " + ex.Message);
            TextToSpeechSyntetizer textToSpeechSyntetizer = GameObject.FindObjectOfType<TextToSpeechSyntetizer>();
            textToSpeechSyntetizer.say("Saving csv failed.");
        }
    }

    public void  loadCsv(string path,string name,MapData mapData)
    {
        // clean up
        _planedRoute.Clear();
        _objOfInterest.Clear();
        _otherObjects.Clear();

        try
        {
            if (File.Exists(path))
            {
                this.misionName = name;
                string[] lines = File.ReadAllLines(path);

                // Skip the header row (assuming the first row contains column names)
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(',');

                    // vytvo�en� waypointu
                    Waypoint waypoint = new Waypoint(mapData);
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

                    // pokud nen� poi nastaveno - skip
                    if (values[40]=="0"|| values[41]=="0")
                    {
                        continue;
                    }

                    string poiLocationString = string.Format("{0}, {1}", values[40], values[41]);
                    float poiRelAlt = 0;

                    if (float.TryParse(values[42], out float result2))                   
                        poiRelAlt = result;                    
                    else
                        throw new Exception("converzion error");
                    
                    //zkus�me dan� poi naj�t - pokud existuje skip
                    bool founded = false;
                    foreach (var poi in _objOfInterest)
                    {
                        if (poi.locationString == poiLocationString)
                        {
                            founded=true;
                            break;
                        }
                    }
                    if (!founded)
                    {
                        ObjOfInterest objOfInterest = new ObjOfInterest(mapData);
                        objOfInterest.locationString = poiLocationString;
                        objOfInterest.relativeAltitude = poiRelAlt;
                        _objOfInterest.Add(objOfInterest);
                    }
                }
                TextToSpeechSyntetizer textToSpeechSyntetizer = GameObject.FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("Mission loaded successfully.");
            }
            else
            {
                Debug.LogWarning("File not found: " + path);
                TextToSpeechSyntetizer textToSpeechSyntetizer = GameObject.FindObjectOfType<TextToSpeechSyntetizer>();
                textToSpeechSyntetizer.say("File not found.");

            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error reading CSV file: " + ex.Message);
            TextToSpeechSyntetizer textToSpeechSyntetizer = GameObject.FindObjectOfType<TextToSpeechSyntetizer>();
            textToSpeechSyntetizer.say("File corupted.");
        }
    }

}


[Serializable]
public class Player : MapObject
{
    public Player(MapData mapData):base(mapData)
    {
        type = ObjType.Player;
    }

    public float heading=0;
}

[Serializable]
public class Waypoint : MapObject
{
    public Waypoint(MapData mapData, MapObject mapObject = null) : base(mapData,mapObject)
    {
        type = ObjType.Waypoint;
    }

    public double heading;
    public double radius;
    public int pos = -1;

    [JsonIgnore]
    public bool hasBeenVisited=false;
    [JsonIgnore]
    public bool setAsTarget = false;

    public void onDroneEnterColider() {
        mapData.onWaypointCrossed(pos);
    }

    public void onReset()
    {
        hasBeenVisited = false;
        setAsTarget = false;
    }

    public ObjOfInterest objOfInterest = null;

}

[Serializable]
public class ObjOfInterest : MapObject
{
    public ObjOfInterest(MapData mapData,MapObject mapObject = null):base(mapData,mapObject)
    {
        type = ObjType.ObjOfInterest;
    }
}

[Serializable]
public class DroneObject : MapObject
{
    public DroneFlightData droneFlightData = null;
    public Quaternion rotation= Quaternion.identity;

    public DroneObject(MapData mapData):base(mapData)
    {
        type = ObjType.Drone;
    }
}
[Serializable]
public class Barier : MapObject
{
    public bool isWarning=false;
    public Barier(MapData mapData, MapObject mapObject = null) : base(mapData, mapObject)
    {
        type = ObjType.Barier;
    }

    virtual public void onDroneEnterColider()
    {
        mapData.droneEnterBarier(isWarning);
    }
    virtual public void onDroneLeaveColider()
    {
        mapData.droneLeaveBarier(isWarning);
    }
}

[Serializable]
public class Warning : Barier
{
    public Warning(MapData mapData, MapObject mapObject = null) : base(mapData, mapObject)
    {
        type = ObjType.Warning;
        isWarning = true;   
    }
}

    [Serializable]
public class MapObject: System.Object
{

    [Geocode]
    public string locationString=""; // gps pozice

    public string name=""; // zobrazovan� n�zev

    public float relativeAltitude = 0f; // v��ka nad zem� 

    [JsonIgnore]
    public MapData mapData = null;

    public enum ObjType
    {
        Waypoint,
        LandingPad,
        Player,
        Drone,
        Barier,
        Warning,
        Unspecified,
        ObjOfInterest
    }
    public ObjType type = ObjType.Unspecified;

    public MapObject(MapData mapData, MapObject mapObject = null ) 
    {
        if (mapObject != null) { 
            locationString = mapObject.locationString;
            name = mapObject.name;
            relativeAltitude = mapObject.relativeAltitude;
            type = mapObject.type;
        }
        this.mapData = mapData;
    }

}
