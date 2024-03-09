// author jakub komárek

using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using System;
using Mapbox.Examples;
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
    GameObject _defaultPrefab;

    [SerializeField]
    GameObject _waypointPrefab;

    [SerializeField]
    GameObject _homeLocationPrefab;

    [SerializeField]
    GameObject _playerPrefab;

    [SerializeField]
    GameObject _poiPrefab;

    [SerializeField]
    GameObject _dronePrefab;

    [SerializeField]
    BoxCollider boxCollider = null;

    [SerializeField]
    public GameObject prefabPointOfInterest = null;

    [SerializeField]
    public bool isMinimap = false;

    [SerializeField]
    private LineRenderer lineRenderer = null;

    ~SpawnOnMap()
    {
    }
    void Start()
    {
        mapData= FindObjectOfType<MapData>();
        mapData.spawnOnMapScripts.Add(this);

        reCreateGameObjects();

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
        if (lineRenderer != null)
        {
            updateLineRenderer();
        }
    }

    // v hlavním sdíleném modulu se zmìnily objekty - je nutné je pøetvoøit
    public void reCreateGameObjects() {
        foreach (var obj in allMapObjects)
        {
            Destroy(obj.spawnetGameObject);
        }
        foreach (var obj in planedRoute)
        {
            Destroy(obj.spawnetGameObject);
        }


        allMapObjects.Clear();
        foreach (var obj in mapData.allObjects) {
            allMapObjects.Add(new MapObjectData(obj));
        }

        planedRoute.Clear();
        int index = 0;
        foreach (var obj in mapData._planedRoute)
        {
            MapObjectData mapObjectData = new MapObjectData(obj);
            mapObjectData.mapObject.name = index.ToString();
            planedRoute.Add(mapObjectData);
            index++;
        }

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = mapData._planedRoute.Count;
            updateLineRenderer();
        }

    }

    void updateLineRenderer()
    {
        for (int i = 0; i < planedRoute.Count; i++)
        {
            if(planedRoute[i].spawnetGameObject!=null)
                lineRenderer.SetPosition(i, planedRoute[i].spawnetGameObject.transform.position);
        }
    }

    private void renderObject(MapObjectData mapCustumeObject)
    {
        if(mapCustumeObject.underManipulation) 
            return;

        GameObject gameObject = mapCustumeObject.spawnetGameObject;


        // pokud objekt nemá v mapì fyzickou reprezataci, udìlej novou
        if (gameObject == null) 
        {
            switch (mapCustumeObject.mapObject.type)
            {
                case MapObject.ObjType.Player:
                    if(_playerPrefab != null)
                        gameObject = Instantiate(_playerPrefab);
                    break;
                case MapObject.ObjType.LandingPad:
                    if (_homeLocationPrefab!=null)
                        gameObject = Instantiate(_homeLocationPrefab);
                    break;
                case MapObject.ObjType.ObjOfInterest:
                    if(_poiPrefab!=null)
                        gameObject = Instantiate(_poiPrefab);
                    break;
                case MapObject.ObjType.Drone:
                    if (_dronePrefab != null)
                        gameObject = Instantiate(_dronePrefab);
                    break;
                default:
                    if (_defaultPrefab != null)
                        gameObject = Instantiate(_defaultPrefab);
                    break;

            }
            mapCustumeObject.spawnetGameObject = gameObject;
        }
        if (gameObject == null)
            return;

        // propsání zmìn po manipulaci
        if (mapCustumeObject.manipulationDirtyFlag) // z objektem bylo manipulováno - zmìny je nutné propsat
        {
            Vector2d vector2d= _map.WorldToGeoPosition(gameObject.transform.localPosition);

            CultureInfo culture = new CultureInfo("en-US");
            mapCustumeObject.mapObject.locationString = string.Format("{0}, {1}", vector2d.x.ToString(culture), vector2d.y.ToString(culture)); // .ToString(culture) protože podìlanej c#
            if (isMinimap)
            {
                var newTransformation = _map.GeoToWorldPosition(vector2d, true);
                float sceneHeight = newTransformation.y;//výška k zemi ve scénì
                float deltaHeight = gameObject.transform.localPosition.y - sceneHeight;
                float calculatedHeight = calcAbsoluteHeight(deltaHeight); // might be wrong
                mapCustumeObject.mapObject.relativeAltitude = calculatedHeight;
            }
            else
            {
                mapCustumeObject.mapObject.relativeAltitude = gameObject.transform.localPosition.y;
            }

            mapCustumeObject.manipulationDirtyFlag = false; // zmìny po manipulaci propsány
        }


        // logika výpoètu pozice
        Vector2d vector2D = Conversions.StringToLatLon(mapCustumeObject.mapObject.locationString);  

        gameObject.transform.localPosition = _map.GeoToWorldPosition(vector2D, true);

        float calcHeight;
        if (isMinimap)
        {   // aproximaèní rovnice pro minimapu
            //mapCustumeObject.mapObject.relativeAltitude = 100;
            calcHeight = calcScenePosition(gameObject.transform.localPosition.y, mapCustumeObject.mapObject.relativeAltitude);
        }
        else
        {   // výška nepotøebuje pøepoèet
            calcHeight = gameObject.transform.localPosition.y + mapCustumeObject.mapObject.relativeAltitude;
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
        if (mapCustumeObject.underManipulation)
            return;

        GameObject gameObject = mapCustumeObject.spawnetGameObject;


        // pokud objekt nemá v mapì fyzickou reprezataci, udìlej novou
        if (gameObject == null)
        {
            if (_waypointPrefab == null)
                return;
            gameObject = Instantiate(_waypointPrefab);
            mapCustumeObject.spawnetGameObject = gameObject;
        }

        // propsání zmìn po manipulaci
        if (mapCustumeObject.manipulationDirtyFlag) // z objektem bylo manipulováno - zmìny je nutné propsat
        {
            Vector2d vector2d = _map.WorldToGeoPosition(gameObject.transform.localPosition);

            CultureInfo culture = new CultureInfo("en-US");
            mapCustumeObject.mapObject.locationString = string.Format("{0}, {1}", vector2d.x.ToString(culture), vector2d.y.ToString(culture)); // .ToString(culture) protože podìlanej c#
            if (isMinimap)
            {
                var newTransformation = _map.GeoToWorldPosition(vector2d, true);
                float sceneHeight = newTransformation.y;//výška k zemi ve scénì
                float deltaHeight = gameObject.transform.localPosition.y - sceneHeight;
                float calculatedHeight = calcAbsoluteHeight(deltaHeight); // might be wrong
                mapCustumeObject.mapObject.relativeAltitude = calculatedHeight;
            }
            else
            {
                mapCustumeObject.mapObject.relativeAltitude = gameObject.transform.localPosition.y;
            }

            mapCustumeObject.manipulationDirtyFlag = false; // zmìny po manipulaci propsány
        }


        // logika výpoètu pozice
        Vector2d vector2D = Conversions.StringToLatLon(mapCustumeObject.mapObject.locationString);

        gameObject.transform.localPosition = _map.GeoToWorldPosition(vector2D, true);

        float calcHeight;
        if (isMinimap)
        {   // aproximaèní rovnice pro minimapu
            //mapCustumeObject.mapObject.relativeAltitude = 100;
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


    const float tiltScaleUnity = 0.115f;
    const float defalutZoomLevel = 19;
    const float aproximateConstant = 0.25f;

    private float calcScenePosition(float groundYpos, float relativeAlt)
    {
        return groundYpos + (relativeAlt / defalutZoomLevel) * tiltScaleUnity * _map.transform.lossyScale.y * aproximateConstant;

    }

    private float calcAbsoluteHeight(float deltaHeight)
    {
        return defalutZoomLevel * tiltScaleUnity * _map.transform.lossyScale.y * aproximateConstant * deltaHeight;
    }


    //data pro vykreslení
    public class MapObjectData
    {
        public GameObject spawnetGameObject = null;

        public MapObject mapObject=null;

        public bool underManipulation = false;
        public bool manipulationDirtyFlag = false;

        public MapObjectData(MapObject mapObject)
        {
            this.mapObject = mapObject;
        }

        public void endManipulation() {
            manipulationDirtyFlag=true;
            underManipulation=false;
        }
    }
}


