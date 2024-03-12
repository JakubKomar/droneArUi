// author jakub komárek

using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using System;
using Mapbox.Examples;
using System.Globalization;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UIElements;

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
                Debug.LogException(e);
            }
        }

        foreach (var mapGameObject in planedRoute)
        {
            try
            {
                renderObject(mapGameObject);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
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
                case MapObject.ObjType.Waypoint:
                    if (_waypointPrefab == null)
                        return;
                    gameObject = Instantiate(_waypointPrefab);
                    mapCustumeObject.spawnetGameObject = gameObject;
                    break;
                default:
                    if (_defaultPrefab != null)
                        gameObject = Instantiate(_defaultPrefab);
                    break;

            };

            if (gameObject == null)
                return;
            gameObject.transform.parent = this.transform;
            mapCustumeObject.spawnetGameObject = gameObject;
            mapCustumeObject.manipulator = gameObject.GetComponent<ObjectManipulator>();
            if (mapCustumeObject.manipulator != null)
            {
                ObjectManipulator objectManipulator = mapCustumeObject.manipulator;

                objectManipulator.OnManipulationStarted.AddListener(mapCustumeObject.onManipultaionStart);
                objectManipulator.OnManipulationEnded.AddListener(mapCustumeObject.onManipulationEnd);
                objectManipulator.OnHoverEntered.AddListener(mapCustumeObject.onHoverStart);
                objectManipulator.OnHoverExited.AddListener(mapCustumeObject.onHoverEnd);
            }

        }


        // propsání zmìn po manipulaci
        if (mapCustumeObject.manipulationDirtyFlag) // z objektem bylo manipulováno - zmìny je nutné propsat
        {
            Vector2d vector2d= _map.WorldToGeoPosition(gameObject.transform.position);

            CultureInfo culture = new CultureInfo("en-US");
            mapCustumeObject.mapObject.locationString = string.Format("{0}, {1}", vector2d.x.ToString(culture), vector2d.y.ToString(culture)); // .ToString(culture) protože podìlanej c#
            if (isMinimap)
            {
                var newTransformation = _map.GeoToWorldPosition(vector2d, true);
                float sceneHeight = newTransformation.y;//výška k zemi ve scénì
                float deltaHeight = gameObject.transform.position.y - sceneHeight;
                Debug.Log(deltaHeight);
                float calculatedHeight = calcAbsoluteHeight(deltaHeight); // might be wrong
                mapCustumeObject.mapObject.relativeAltitude = calculatedHeight;
            }
            else
            {
                var newTransformation = _map.GeoToWorldPosition(vector2d, true);
                float sceneHeight = newTransformation.y;//výška k zemi ve scénì
                float deltaHeight = gameObject.transform.position.y - sceneHeight;
                mapCustumeObject.mapObject.relativeAltitude = deltaHeight;
            }

            mapCustumeObject.manipulationDirtyFlag = false; // zmìny po manipulaci propsány
        }

        if (!mapCustumeObject.underManipulation)
        {
            // logika výpoètu pozice
            Vector2d vector2D = Conversions.StringToLatLon(mapCustumeObject.mapObject.locationString);

            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.position = _map.GeoToWorldPosition(vector2D, true);
            gameObject.transform.rotation = _map.transform.rotation;

            float calcHeight;
            if (isMinimap)
            {   // aproximaèní rovnice pro minimapu
                //mapCustumeObject.mapObject.relativeAltitude = 100;
                calcHeight = calcScenePosition(gameObject.transform.position.y, mapCustumeObject.mapObject.relativeAltitude);
            }
            else
            {   // výška nepotøebuje pøepoèet
                calcHeight = gameObject.transform.position.y + mapCustumeObject.mapObject.relativeAltitude;
            }

            gameObject.transform.position = new Vector3(gameObject.transform.position.x, calcHeight, gameObject.transform.position.z);
            gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

            LabelTextSetter labelTextSetter = gameObject.GetComponent<LabelTextSetter>();
            if (labelTextSetter != null)
            {
                labelTextSetter.Set(new Dictionary<String, object> { { "name", mapCustumeObject.mapObject.name }, });
            }
        }
       
        // vykresluj v minimapì pouze objekty v bounding boxu
        if (boxCollider==null|| boxCollider.bounds.Contains(gameObject.transform.position)|| mapCustumeObject.underManipulation)
        { 
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);


        //dodateèné zmìny pro urèité typy
        switch (mapCustumeObject.mapObject.type)
        {
            case MapObject.ObjType.Player:
                if (mapCustumeObject.mapObject is Player) // pøetypování
                {
                    Player player = (Player)mapCustumeObject.mapObject;
                    Vector3 elulerRot;

                    if (isMinimap)
                        elulerRot = new Vector3(0, player.heading, 0);
                    else 
                        elulerRot = new Vector3(0, player.heading, 0);

                    gameObject.transform.localEulerAngles = elulerRot;
                }
                break;
            case MapObject.ObjType.LandingPad:

                break;
            case MapObject.ObjType.ObjOfInterest:

                break;
            case MapObject.ObjType.Drone:

                break;
            default:
                break;

        }
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
        return (deltaHeight * defalutZoomLevel) / (tiltScaleUnity * _map.transform.lossyScale.y * aproximateConstant);
    }


    //data pro vykreslení
    public class MapObjectData
    {
        public GameObject spawnetGameObject = null;

        public MapObject mapObject=null;

        public ObjectManipulator manipulator = null;

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

        public void onHoverStart(ManipulationEventData eventData)
        {
            Debug.Log("hover start");
        }

        public void onHoverEnd(ManipulationEventData eventData)
        {
            Debug.Log("hover end");
        }
        public void onManipultaionStart(ManipulationEventData eventData)
        {
            this.underManipulation = true;
            Debug.Log("manipulataion start");
        }
        public void onManipulationEnd(ManipulationEventData eventData)
        {
            this.underManipulation = false;
            this.manipulationDirtyFlag = true;
            Debug.Log("manipulataion ends");
        }

    }
}


