/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
///  stará se o zanesení vlastních objektù do abstraktní mapy, zaøizuje manipulaci a propisování úprav do ostatních map, obsahuje metody pro vytvoøení nových objektù
/// </summary>

using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using System;
using Mapbox.Examples;
using System.Globalization;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

public class SpawnOnMap : MonoBehaviour
{
    [SerializeField]
    AbstractMap _map;


    private List<MapObjectData> allMapObjects = new List<MapObjectData>();
    private List<MapObjectData> planedRoute = new List<MapObjectData>();

    private List<MapObject> removalList = new List<MapObject>();

    private MapData mapData = null;


    [SerializeField]
    float _spawnScale = 100f;

    [SerializeField]
    float _barierMinimapScale = 0.02f;

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
    GameObject _barierPrefab;

    [SerializeField]
    GameObject _warningPrefab;

    [SerializeField]
    BoxCollider boxCollider = null;

    [SerializeField]
    public GameObject prefabPointOfInterest = null;

    [SerializeField]
    public bool isMinimap = false;

    [SerializeField]
    private LineRenderer lineRenderer = null;

    void Start()
    {
        mapData = FindObjectOfType<MapData>();
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

        mapData.spawnedObjectDeletion(removalList);
        removalList.Clear();

    }


    public void createNewObject(MapObject.ObjType type, GameObject gameObject)
    {
        if (!isMinimap)
        {
            Debug.LogError("object can be created only from minimap");
            return;
        }
        // pokud mimo prostor mapy - objekt nevytváøej
        if (boxCollider == null || !boxCollider.bounds.Contains(gameObject.transform.position))
        {
            //Debug.Log("Object not in space of map - creation stoped");
            return;
        }

        MapObject mapObject = new MapObject(null);
        mapObject.type = type;

        //výpoèet pozice
        Vector2d vector2d = _map.WorldToGeoPosition(gameObject.transform.position); // ziskej pozici gps na mapì

        CultureInfo culture = new CultureInfo("en-US");
        mapObject.locationString = string.Format("{0}, {1}", vector2d.x.ToString(culture), vector2d.y.ToString(culture));

        // výpoèet výšky
        var newTransformation = _map.GeoToWorldPosition(vector2d, true);
        float sceneHeight = newTransformation.y;//výška k zemi ve scénì
        float deltaHeight = gameObject.transform.position.y - sceneHeight;
        float calculatedHeight = calcAbsoluteHeight(deltaHeight);

        if (calculatedHeight < 0) { calculatedHeight = 0; }
        mapObject.relativeAltitude = calculatedHeight;

        if (type == MapObject.ObjType.Barier || type == MapObject.ObjType.Warning)
        {
            mapObject.rotation = Quaternion.identity;
            mapObject.scale = new Vector3(15, 15, 15); // vytvoø krychly o rozmìrech 15x15x15m

            mapObject.relativeAltitude = 7.5f; // støed je v pùlce

        }
        if (type == MapObject.ObjType.Waypoint)
        {
            createWaypoint(mapObject, gameObject);
            return;
        }

        mapData.addObject(mapObject);
    }

    //tato funkce definuje kam semá waypoint pøidat
    private void createWaypoint(MapObject mapObject, GameObject gameObject)
    {
        MapObjectData nearestWp1 = null;
        MapObjectData nearestWp2 = null;
        int nearestWpIndex1 = -1;
        int nearestWpIndex2 = -1;
        float closestDistance1 = float.MaxValue;
        float closestDistance2 = float.MaxValue;
        int index = 0;

        foreach (var waypoint in planedRoute)
        {
            float distance = Vector3.Distance(waypoint.spawnetGameObject.transform.position, gameObject.transform.position);

            if (distance < closestDistance1)
            {
                nearestWp2 = nearestWp1;
                nearestWpIndex2 = nearestWpIndex1;
                closestDistance2 = closestDistance1;

                nearestWp1 = waypoint;
                nearestWpIndex1 = index;
                closestDistance1 = distance;
            }
            else if (distance < closestDistance2)
            {
                nearestWp2 = waypoint;
                nearestWpIndex2 = index;
                closestDistance2 = distance;
            }
            index++;
        }

        if (nearestWpIndex1 < 0) // pokud není žádný nejbližší vlož nakone
        {
            mapData.addObject(mapObject);
        }
        else if (nearestWpIndex1 >= 0 && nearestWpIndex2 >= 0 && Math.Abs(nearestWpIndex1 - nearestWpIndex2) == 1)
        { // vládání mezi je podporováno pouze pokud dva nejbližší jsou indexovì za sebou

            float distanceBettweenCloses = Vector3.Distance(nearestWp2.spawnetGameObject.transform.position, nearestWp1.spawnetGameObject.transform.position);
            if (nearestWpIndex1 - nearestWpIndex2 > 0) // nejbližší pøedchází idexové druhého 
            {
                if (distanceBettweenCloses > closestDistance2)
                {
                    mapData.addObject(mapObject, nearestWpIndex1);
                }
                else
                {
                    mapData.addObject(mapObject, nearestWpIndex1 + 1);
                }
            }
            else //nejbližší nepøedchází idexové druhého 
            {
                if (distanceBettweenCloses < closestDistance2)
                {
                    mapData.addObject(mapObject, nearestWpIndex1);
                }
                else
                {
                    mapData.addObject(mapObject, nearestWpIndex1 + 1);
                }
            }
        }
        else
        { // jinak vlož nakonec
            mapData.addObject(mapObject);
        }
    }


    // v hlavním sdíleném modulu se zmìnily objekty - je nutné je pøetvoøit
    public void reCreateGameObjects()
    {
        // smazání starých objektù
        foreach (var obj in allMapObjects)
        {
            Destroy(obj.spawnetGameObject);
        }
        foreach (var obj in planedRoute)
        {
            Destroy(obj.spawnetGameObject);
        }


        allMapObjects.Clear();
        foreach (var obj in mapData.allObjects)
        {
            allMapObjects.Add(new MapObjectData(obj));
        }

        planedRoute.Clear();
        foreach (var obj in mapData._planedRoute)
        {
            planedRoute.Add(new MapObjectData(obj));
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
            if (planedRoute[i].spawnetGameObject != null)
            {
                if (planedRoute[i].spawnetGameObject.activeSelf || !isMinimap)
                {
                    lineRenderer.SetPosition(i, planedRoute[i].spawnetGameObject.transform.position);
                }
                else // pokud je objekt mimo minimapu je výška odvozena ze støedu minimapy (aby nedocházelo ke skreslení trasy)
                {
                    Vector3 vector = planedRoute[i].spawnetGameObject.transform.position;

                    vector.y = calcScenePosition(_map.GeoToWorldPosition(_map.CenterLatitudeLongitude).y, planedRoute[i].mapObject.relativeAltitude);
                    lineRenderer.SetPosition(i, vector);
                }
            }
        }
    }

    private void renderObject(MapObjectData mapCustumeObject)
    {

        GameObject gameObject = mapCustumeObject.spawnetGameObject;


        // pokud objekt nemá v mapì fyzickou reprezataci, udìlej novou a nastav jí vlastnosti dle typu objektu - voláno pouze jednou
        if (gameObject == null)
        {
            switch (mapCustumeObject.mapObject.type)
            {
                case MapObject.ObjType.Player:
                    if (_playerPrefab != null)
                    {
                        gameObject = Instantiate(_playerPrefab);
                        gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                    }
                    break;
                case MapObject.ObjType.LandingPad:
                    if (_homeLocationPrefab != null)
                    {
                        gameObject = Instantiate(_homeLocationPrefab);
                        gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                    }
                    break;
                case MapObject.ObjType.ObjOfInterest:
                    if (_poiPrefab != null)
                    {
                        gameObject = Instantiate(_poiPrefab);
                        gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                    }
                    break;
                case MapObject.ObjType.Drone:
                    if (_dronePrefab != null)
                    {
                        gameObject = Instantiate(_dronePrefab);
                        gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                    }
                    break;
                case MapObject.ObjType.Waypoint:
                    if (_waypointPrefab == null)
                    {
                        return;
                    }
                    gameObject = Instantiate(_waypointPrefab);
                    gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                    break;
                case MapObject.ObjType.Barier:
                    if (_barierPrefab == null)
                        return;
                    gameObject = Instantiate(_barierPrefab);

                    if (isMinimap)
                        gameObject.transform.localScale = mapCustumeObject.mapObject.scale;
                    else
                        gameObject.transform.localScale = mapCustumeObject.mapObject.scale;

                    gameObject.transform.rotation = mapCustumeObject.mapObject.rotation;

                    break;
                case MapObject.ObjType.Warning:
                    if (_warningPrefab == null)
                        return;
                    gameObject = Instantiate(_warningPrefab);

                    if (isMinimap)
                        gameObject.transform.localScale = mapCustumeObject.mapObject.scale * _barierMinimapScale;
                    else
                        gameObject.transform.localScale = mapCustumeObject.mapObject.scale;
                    gameObject.transform.rotation = mapCustumeObject.mapObject.rotation;
                    break;

                default:
                    if (_defaultPrefab != null)
                        gameObject = Instantiate(_defaultPrefab);
                    break;

            };

            if (gameObject == null)
                return;


            gameObject.transform.parent = this.transform; // jako rodiè je nastavena mapa


            mapCustumeObject.spawnetGameObject = gameObject;
            mapCustumeObject.manipulator = gameObject.GetComponent<ObjectManipulator>();
            mapCustumeObject.isInMinimap = isMinimap;

            if (mapCustumeObject.manipulator != null) // nastavení bindù pro manipulaci s objektem
            {
                ObjectManipulator objectManipulator = mapCustumeObject.manipulator;
                objectManipulator.AllowFarManipulation = !isMinimap;
                objectManipulator.OnManipulationStarted.AddListener(mapCustumeObject.onManipultaionStart);
                objectManipulator.OnManipulationEnded.AddListener(mapCustumeObject.onManipulationEnd);

                //objectManipulator.OnHoverEntered.AddListener(mapCustumeObject.onHoverStart);
                //objectManipulator.OnHoverExited.AddListener(mapCustumeObject.onHoverEnd);


                mapCustumeObject.boundsControl = gameObject.GetComponent<BoundsControl>();
                if (mapCustumeObject.boundsControl != null) // tyto manipulátory jsou pro zóny
                {
                    BoundsControl boundsControl = mapCustumeObject.boundsControl;

                    boundsControl.ScaleStarted.AddListener(mapCustumeObject.onManipultaionStart);
                    boundsControl.RotateStarted.AddListener(mapCustumeObject.onManipultaionStart);
                    boundsControl.TranslateStarted.AddListener(mapCustumeObject.onManipultaionStart);

                    boundsControl.ScaleStopped.AddListener(mapCustumeObject.onManipulationEnd);
                    boundsControl.RotateStopped.AddListener(mapCustumeObject.onManipulationEnd);
                    boundsControl.TranslateStopped.AddListener(mapCustumeObject.onManipulationEnd);
                }
            }


            MapGameObjectData mapGameObjectData = gameObject.GetComponent<MapGameObjectData>(); // zpìtný odkaz v prefabu
            if (mapGameObjectData != null)
            {
                mapGameObjectData.mapObjectData = mapCustumeObject;
            }
        } // konec vytváøecího bloku

        // propsání zmìn po a pøi manipulaci
        if (mapCustumeObject.underManipulation || mapCustumeObject.manipulationDirtyFlag) // z objektem bylo manipulováno - zmìny je nutné propsat
        {
            if ((!mapCustumeObject.underManipulation) && (isMinimap))
            {
                // pokud byl objekt pøetažen mimo boundingbox minimapy, uživatel ho chtìl smazat
                if (!boxCollider.bounds.Contains(gameObject.transform.position))
                {
                    removalList.Add(mapCustumeObject.mapObject);
                }
            }


            // zpìtný pøevod na gps souøadnice
            Vector2d vector2d = _map.WorldToGeoPosition(gameObject.transform.position);

            CultureInfo culture = new CultureInfo("en-US");
            //propis do sdílených dat
            mapCustumeObject.mapObject.locationString = string.Format("{0}, {1}", vector2d.x.ToString(culture), vector2d.y.ToString(culture)); // .ToString(culture) protože podìlanej c#


            float sceneHeight;//výška k zemi ve scénì
            float deltaHeight;

            if (isMinimap) // rozvìtvení dle manipulace skze minimapu nebo ve wordscale
            {
                var newTransformation = _map.GeoToWorldPosition(vector2d, true);

                // bariéry mají nastavenou výšku tak aby spodní hrana byla na zemi
                if (mapCustumeObject.mapObject.type == MapObject.ObjType.Barier || mapCustumeObject.mapObject.type == MapObject.ObjType.Warning)
                {
                    mapCustumeObject.mapObject.relativeAltitude = calcAbsoluteHeight(mapCustumeObject.spawnetGameObject.transform.localScale.y * 0.5f);
                }
                else // u ostatních urèuje výšku uživatel
                {
                    sceneHeight = newTransformation.y;//výška k zemi ve scénì
                    deltaHeight = gameObject.transform.position.y - sceneHeight;
                    mapCustumeObject.mapObject.relativeAltitude = calcAbsoluteHeight(deltaHeight);
                }
            }
            else
            {
                var newTransformation = _map.GeoToWorldPosition(vector2d, true);


                if (mapCustumeObject.mapObject.type == MapObject.ObjType.Barier || mapCustumeObject.mapObject.type == MapObject.ObjType.Warning)
                {
                    mapCustumeObject.mapObject.relativeAltitude = mapCustumeObject.spawnetGameObject.transform.localScale.y / 2;

                }
                else
                {
                    sceneHeight = newTransformation.y;//výška k zemi ve scénì
                    deltaHeight = gameObject.transform.position.y - sceneHeight;
                    mapCustumeObject.mapObject.relativeAltitude = deltaHeight;
                }
            }

            // pokud je výška negativní, klipne se do nuly
            if (mapCustumeObject.mapObject.relativeAltitude < 0)
                mapCustumeObject.mapObject.relativeAltitude = 0;


            if (isMinimap)
            {
                // uložení scalu - význam pouze pro bariéry
                mapCustumeObject.mapObject.scale = gameObject.transform.localScale / _barierMinimapScale;
            }
            else
                mapCustumeObject.mapObject.scale = gameObject.transform.localScale;
            //uložení rotace - význam pouze pro bariéry
            mapCustumeObject.mapObject.rotation = gameObject.transform.localRotation;

            // pokud již z objektem není manipulováno, oèistí se
            if (mapCustumeObject.manipulationDirtyFlag)
                mapCustumeObject.manipulationDirtyFlag = false; // zmìny po manipulaci propsány
        }

        // pokud se z objektem nemanipuluje, nastavujem pozici dle zdílených dat
        if (!mapCustumeObject.underManipulation)
        {
            // logika výpoètu pozice
            Vector2d vector2D = Conversions.StringToLatLon(mapCustumeObject.mapObject.locationString);

            if (mapCustumeObject.mapObject.type == MapObject.ObjType.Drone && !isMinimap) { } // dron má ve wordscalu vlstní vykreslovací logiku - nedìlám nic
            else
            {
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.position = _map.GeoToWorldPosition(vector2D, true); // pozice nastavena dle gps souøadnic do výšky 0m nad terénem
            }

            // bariéry mají vlstní blok pro nastavení mìøítka objektu
            if (mapCustumeObject.mapObject.type == MapObject.ObjType.Barier || mapCustumeObject.mapObject.type == MapObject.ObjType.Warning)
            {
                if (isMinimap)
                {
                    gameObject.transform.localScale = mapCustumeObject.mapObject.scale * _barierMinimapScale;
                }
                else
                    gameObject.transform.localScale = mapCustumeObject.mapObject.scale;

                gameObject.transform.localRotation = mapCustumeObject.mapObject.rotation;
            }
            else
            {
                // ostatní objekty mají mìøítko pevné dle nastavení konstanty
                gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                gameObject.transform.rotation = _map.transform.rotation;
            }

            //zde se nastaví výška objektu
            float calcHeight; 
            if (isMinimap)
            {  
                calcHeight = calcScenePosition(gameObject.transform.position.y, mapCustumeObject.mapObject.relativeAltitude);
            }
            else
            {   // výška nepotøebuje pøepoèet
                calcHeight = gameObject.transform.position.y + mapCustumeObject.mapObject.relativeAltitude;
            }

            if(mapCustumeObject.mapObject.type==MapObject.ObjType.Drone && !isMinimap) // dron ve wordscalu potøebuje znát gps pozici dle gps pro dopøesnìní
            {
                return;
            }
            else // pro ostatní objekty pozici propisuji rovnou
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, calcHeight, gameObject.transform.position.z);

        }

        // vykresluj v minimapì pouze objekty v bounding boxu - ostaní skryj
        if (boxCollider == null || boxCollider.bounds.Contains(gameObject.transform.position) || mapCustumeObject.underManipulation)
        {
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);

        LabelTextSetter labelTextSetter = gameObject.GetComponent<LabelTextSetter>();
        //dodateèné zmìny pro urèité typy objektù
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
                if (!isMinimap)
                {
                    CompassIndicator compasIndicator = FindObjectOfType<CompassIndicator>();
                    if (compasIndicator != null)
                        compasIndicator.landingPad = gameObject;
                }
                break;
            case MapObject.ObjType.ObjOfInterest:

                if (labelTextSetter != null)
                {
                    labelTextSetter.Set(new Dictionary<String, object> { { "name", mapCustumeObject.mapObject.name }, });
                }
                break;
            case MapObject.ObjType.Drone:
                if (labelTextSetter != null)
                {
                    labelTextSetter.Set(new Dictionary<String, object> { { "name", ("(" + Math.Round(mapCustumeObject.mapObject.relativeAltitude).ToString() + "m)") }, });
                }
                break;
            case MapObject.ObjType.Waypoint:
                if (mapCustumeObject.mapObject is Waypoint) // pøetypování
                {
                    Waypoint waypoint = (Waypoint)mapCustumeObject.mapObject;
                    if (labelTextSetter != null)
                    {
                        labelTextSetter.Set(new Dictionary<String, object> { { "name", (waypoint.pos.ToString() + "(" + Math.Round(mapCustumeObject.mapObject.relativeAltitude).ToString() + "m)") }, });
                    }
                    if (!isMinimap && waypoint.setAsTarget)
                    {
                        CompassIndicator compasIndicator = FindObjectOfType<CompassIndicator>();
                        if (compasIndicator != null)
                            compasIndicator.activeWaypoint = gameObject;
                    }
                }
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

        public MapObject mapObject = null;

        public ObjectManipulator manipulator = null;
        public BoundsControl boundsControl = null;

        public bool underManipulation = false;
        public bool manipulationDirtyFlag = false;

        public bool isInMinimap;

        public MapObjectData(MapObject mapObject)
        {
            this.mapObject = mapObject;
        }

        public void endManipulation()
        {
            manipulationDirtyFlag = true;
            underManipulation = false;
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
            onManipultaionStart();
        }
        public void onManipulationEnd(ManipulationEventData eventData)
        {
            onManipulationEnd();
        }
        public void onManipultaionStart()
        {
            this.underManipulation = true;
            //Debug.Log("manipulataion start");
        }
        public void onManipulationEnd()
        {
            this.underManipulation = false;
            this.manipulationDirtyFlag = true;
            //Debug.Log("manipulataion ends");
        }
    }
}


