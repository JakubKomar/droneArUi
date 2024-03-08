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
using System.Globalization;

public class SpawnOnMap : MonoBehaviour
{
    [SerializeField]
    AbstractMap _map;


    private List<MapObjectData> allMapObjects = new List<MapObjectData>();
    private List<MapObjectData> planedRoute = new List<MapObjectData>();

    private MapData mapData = null;


    [SerializeField]
    float _spawnScale = 100f;

    [SerializeField]
    GameObject _markerPrefab;


    [SerializeField]
    BoxCollider boxCollider = null;


    [SerializeField]
    public GameObject prefabPointOfInterest = null;

    [SerializeField]
    public bool isMinimap = false;

    ~SpawnOnMap()
    {
    }
    void Start()
    {
        mapData= FindObjectOfType<MapData>();
        mapData.spawnOnMapScripts.Add(this);
        reCreateGameObjects();

    }

    public void reCreateGameObjects() {
        allMapObjects.Clear();
        foreach (var obj in mapData.allObjects){
            allMapObjects.Add(new MapObjectData(obj));
        }

        planedRoute.Clear();
        foreach (var obj in mapData._planedRoute)
        {
            planedRoute.Add(new MapObjectData(obj));
        }
    }

    private void renderObject(MapObjectData mapCustumeObject)
    {
        Vector2d vector2D = Conversions.StringToLatLon(mapCustumeObject.mapObject.locationString);

        GameObject gameObject = mapCustumeObject.spawnetGameObject;

        if (gameObject == null)
        {
            gameObject = Instantiate(_markerPrefab);
            mapCustumeObject.spawnetGameObject = gameObject;
            
        }


        gameObject.transform.localPosition = _map.GeoToWorldPosition(vector2D, true);

        float calcHeight;
        if (isMinimap)
        {   // aproximaèní rovnice pro minimapu
            calcHeight = calcScenePosition(gameObject.transform.localPosition.y, mapCustumeObject.mapObject.relativeAltitude);
        }
        else
        {   // výška nepotøebuje pøepoèet
            calcHeight = mapCustumeObject.mapObject.relativeAltitude;
        }

        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, calcHeight, gameObject.transform.localPosition.z);
        gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

        LabelTextSetter labelTextSetter = gameObject.GetComponent<LabelTextSetter>();
        if(labelTextSetter != null) {
            labelTextSetter.Set(new Dictionary<String, object> { { "name", mapCustumeObject.mapObject.name }, });
        }
       
        if (boxCollider==null|| boxCollider.bounds.Contains(gameObject.transform.localPosition))
        { // if obeject is in boundig box, show it
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }

    private void renderRoute(MapObjectData mapCustumeObject)
    {
        Vector2d vector2D = Conversions.StringToLatLon(mapCustumeObject.mapObject.locationString);

        GameObject gameObject = mapCustumeObject.spawnetGameObject;

        if (gameObject == null)
            gameObject = Instantiate(_markerPrefab);


        gameObject.transform.localPosition = _map.GeoToWorldPosition(vector2D, true);

        float calcHeight;
        if (isMinimap)
        {   // aproximaèní rovnice pro minimapu
            calcHeight = calcScenePosition(gameObject.transform.localPosition.y, mapCustumeObject.mapObject.relativeAltitude);
        }
        else
        {   // výška nepotøebuje pøepoèet
            calcHeight = mapCustumeObject.mapObject.relativeAltitude;
        }

        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, calcHeight, gameObject.transform.localPosition.z);
        gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

        LabelTextSetter labelTextSetter = gameObject.GetComponent<LabelTextSetter>();
        if (labelTextSetter != null)
        {
            labelTextSetter.Set(new Dictionary<String, object> { { "name", mapCustumeObject.mapObject.name }, });
        }

        if (boxCollider == null || boxCollider.bounds.Contains(gameObject.transform.localPosition))
        { // if obeject is in boundig box, show it
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }

    private void Update()
    {
        foreach (var mapGameObject in allMapObjects)
        {
            try
            {
                renderObject(mapGameObject);
            }
            catch (Exception e)
            {
            }
        }

        foreach (var mapGameObject in planedRoute)
        {
            try
            {
                renderRoute(mapGameObject);
            }
            catch (Exception e)
            {
            }
        }
    }

    private float calcScenePosition(float groundYpos, float relativeAlt)
    {
        const float tiltScaleUnity = 0.115f;
        const float defalutZoomLevel = 19;
        const float aproximateConstant = 0.25f;
        return groundYpos + (relativeAlt / defalutZoomLevel) * tiltScaleUnity * _map.transform.lossyScale.y * aproximateConstant;

    }

    //data pro vykreslení
    public class MapObjectData
    {
        public GameObject spawnetGameObject = null;

        public MapObject mapObject=null;

        public MapObjectData(MapObject mapObject)
        {
            this.mapObject = mapObject;
        }

    }
}


