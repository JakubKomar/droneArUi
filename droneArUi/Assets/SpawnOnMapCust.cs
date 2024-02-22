// author jakub komárek

using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using Mapbox.Directions;
using Newtonsoft.Json;
using System;
using Unity.VisualScripting;
using Mapbox.Examples;
using System.Linq;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using System.Globalization;

public class SpawnOnMap : MonoBehaviour
{
    [SerializeField]
    AbstractMap _map;

    [SerializeField]
    private List<MapObjectData> _wayPointsFromUnity;

    public MapObjectData droneObj = null;


    [SerializeField]
    float _spawnScale = 100f;

    [SerializeField]
    GameObject _markerPrefab;


    public BoxCollider boxCollider = null;

    public DroneManager droneManger = null;
    ~SpawnOnMap()
    {
        foreach (var mapGameObject in _wayPointsFromUnity)
        {
            mapGameObject.spawnetGameObject=null;
        }

    }
    void Start()
    {

        foreach (var mapGameObject in _wayPointsFromUnity)
        {
            renderObject(mapGameObject);
        }

        droneObj = new MapObjectData();
        droneObj.locationString = "49.22743926623377, 16.596966877183366";
        droneObj.relativeAltitude = 10;
        droneObj.name = "dron";
        droneObj.type = MapObjectData.ObjType.Drone;
        _wayPointsFromUnity.Add(droneObj);
        


    }
    private void renderObject(MapObjectData mapCustumeObject)
    {
        mapCustumeObject.vector2D = Conversions.StringToLatLon(mapCustumeObject.locationString);

        Vector2d vector2D = mapCustumeObject.vector2D;
        

        if(mapCustumeObject.spawnetGameObject == null)
            mapCustumeObject.spawnetGameObject = Instantiate(_markerPrefab);

        GameObject instance = mapCustumeObject.spawnetGameObject;


        instance.transform.localPosition = _map.GeoToWorldPosition(vector2D, true);

        float calcHeight = calcScenePosition(instance.transform.localPosition.y, mapCustumeObject.relativeAltitude);
        instance.transform.localPosition = new Vector3(instance.transform.localPosition.x, calcHeight, instance.transform.localPosition.z);
        instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

        LabelTextSetter labelTextSetter = instance.GetComponent<LabelTextSetter>();
        labelTextSetter.Set(new Dictionary<String, object> { { "name", mapCustumeObject.name }, });




        if (boxCollider.bounds.Contains(instance.transform.localPosition))
        { // if obejct is in boundig box, show it
            instance.SetActive(true);
        }
        else
            instance.SetActive(false);
    }

    private void Update()
    {
        if (droneManger.ControlledDrone!=null)
        {
            droneObj.locationString = string.Format("{0}, {1}",  droneManger.ControlledDrone.FlightData.Latitude.ToString(CultureInfo.InvariantCulture), 
                droneManger.ControlledDrone.FlightData.Longitude.ToString(CultureInfo.InvariantCulture));

            droneObj.name = droneManger.ControlledDrone.FlightData.DroneId;
            droneObj.relativeAltitude = (float)droneManger.ControlledDrone.FlightData.Altitude;
        }

        foreach (var mapGameObject in _wayPointsFromUnity)
        {
            renderObject(mapGameObject);
        }


    }


    private float calcScenePosition(float groundYpos, float relativeAlt)
    {
        const float tiltScaleUnity = 0.115f;
        const float defalutZoomLevel = 19;
        const float aproximateConstant = 0.25f;
        return groundYpos + (relativeAlt / defalutZoomLevel) * tiltScaleUnity * _map.transform.lossyScale.y * aproximateConstant;

    }

    [Serializable]
    public class MapObjectData
    {
        public enum ObjType
        {
            Waypoint,
            LandingPad,
            Player,
            Drone,
            PowerLine,
            Barier
        }

        [SerializeField]
        [JsonProperty("locationStr")]
        [Geocode]
        public string locationString;

        [JsonProperty("alt")]
        [SerializeField]
        public float relativeAltitude = 10f;

        [JsonProperty("name")]
        public string name = "test";

        [JsonProperty("type")]
        [SerializeField]
        public ObjType type = ObjType.Waypoint;


        [JsonIgnore]
        [HideInInspector]
        public GameObject spawnetGameObject = null;

        [JsonIgnore]
        [HideInInspector]
        public Vector2d vector2D;

    }
}


