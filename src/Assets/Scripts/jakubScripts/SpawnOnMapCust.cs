/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
///  star� se o zanesen� vlastn�ch objekt� do abstraktn� mapy, za�izuje manipulaci a propisov�n� �prav do ostatn�ch map, obsahuje metody pro vytvo�en� nov�ch objekt�
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
        // pokud mimo prostor mapy - objekt nevytv��ej
        if (boxCollider == null || !boxCollider.bounds.Contains(gameObject.transform.position))
        {
            //Debug.Log("Object not in space of map - creation stoped");
            return;
        }

        MapObject mapObject = new MapObject(null);
        mapObject.type = type;

        //v�po�et pozice
        Vector2d vector2d = _map.WorldToGeoPosition(gameObject.transform.position); // ziskej pozici gps na map�

        CultureInfo culture = new CultureInfo("en-US");
        mapObject.locationString = string.Format("{0}, {1}", vector2d.x.ToString(culture), vector2d.y.ToString(culture));

        // v�po�et v��ky
        var newTransformation = _map.GeoToWorldPosition(vector2d, true);
        float sceneHeight = newTransformation.y;//v��ka k zemi ve sc�n�
        float deltaHeight = gameObject.transform.position.y - sceneHeight;
        float calculatedHeight = calcAbsoluteHeight(deltaHeight);

        if (calculatedHeight < 0) { calculatedHeight = 0; }
        mapObject.relativeAltitude = calculatedHeight;

        if (type == MapObject.ObjType.Barier || type == MapObject.ObjType.Warning)
        {
            mapObject.rotation = Quaternion.identity;
            mapObject.scale = new Vector3(15, 15, 15); // vytvo� krychly o rozm�rech 15x15x15m

            mapObject.relativeAltitude = 7.5f; // st�ed je v p�lce

        }
        if (type == MapObject.ObjType.Waypoint)
        {
            createWaypoint(mapObject, gameObject);
            return;
        }

        mapData.addObject(mapObject);
    }

    //tato funkce definuje kam sem� waypoint p�idat
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

        if (nearestWpIndex1 < 0) // pokud nen� ��dn� nejbli��� vlo� nakone
        {
            mapData.addObject(mapObject);
        }
        else if (nearestWpIndex1 >= 0 && nearestWpIndex2 >= 0 && Math.Abs(nearestWpIndex1 - nearestWpIndex2) == 1)
        { // vl�d�n� mezi je podporov�no pouze pokud dva nejbli��� jsou indexov� za sebou

            float distanceBettweenCloses = Vector3.Distance(nearestWp2.spawnetGameObject.transform.position, nearestWp1.spawnetGameObject.transform.position);
            if (nearestWpIndex1 - nearestWpIndex2 > 0) // nejbli��� p�edch�z� idexov� druh�ho 
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
            else //nejbli��� nep�edch�z� idexov� druh�ho 
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
        { // jinak vlo� nakonec
            mapData.addObject(mapObject);
        }
    }


    // v hlavn�m sd�len�m modulu se zm�nily objekty - je nutn� je p�etvo�it
    public void reCreateGameObjects()
    {
        // smaz�n� star�ch objekt�
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
                else // pokud je objekt mimo minimapu je v��ka odvozena ze st�edu minimapy (aby nedoch�zelo ke skreslen� trasy)
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


        // pokud objekt nem� v map� fyzickou reprezataci, ud�lej novou a nastav j� vlastnosti dle typu objektu - vol�no pouze jednou
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


            gameObject.transform.parent = this.transform; // jako rodi� je nastavena mapa


            mapCustumeObject.spawnetGameObject = gameObject;
            mapCustumeObject.manipulator = gameObject.GetComponent<ObjectManipulator>();
            mapCustumeObject.isInMinimap = isMinimap;

            if (mapCustumeObject.manipulator != null) // nastaven� bind� pro manipulaci s objektem
            {
                ObjectManipulator objectManipulator = mapCustumeObject.manipulator;
                objectManipulator.AllowFarManipulation = !isMinimap;
                objectManipulator.OnManipulationStarted.AddListener(mapCustumeObject.onManipultaionStart);
                objectManipulator.OnManipulationEnded.AddListener(mapCustumeObject.onManipulationEnd);

                //objectManipulator.OnHoverEntered.AddListener(mapCustumeObject.onHoverStart);
                //objectManipulator.OnHoverExited.AddListener(mapCustumeObject.onHoverEnd);


                mapCustumeObject.boundsControl = gameObject.GetComponent<BoundsControl>();
                if (mapCustumeObject.boundsControl != null) // tyto manipul�tory jsou pro z�ny
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


            MapGameObjectData mapGameObjectData = gameObject.GetComponent<MapGameObjectData>(); // zp�tn� odkaz v prefabu
            if (mapGameObjectData != null)
            {
                mapGameObjectData.mapObjectData = mapCustumeObject;
            }
        } // konec vytv��ec�ho bloku

        // props�n� zm�n po a p�i manipulaci
        if (mapCustumeObject.underManipulation || mapCustumeObject.manipulationDirtyFlag) // z objektem bylo manipulov�no - zm�ny je nutn� propsat
        {
            if ((!mapCustumeObject.underManipulation) && (isMinimap))
            {
                // pokud byl objekt p�eta�en mimo boundingbox minimapy, u�ivatel ho cht�l smazat
                if (!boxCollider.bounds.Contains(gameObject.transform.position))
                {
                    removalList.Add(mapCustumeObject.mapObject);
                }
            }


            // zp�tn� p�evod na gps sou�adnice
            Vector2d vector2d = _map.WorldToGeoPosition(gameObject.transform.position);

            CultureInfo culture = new CultureInfo("en-US");
            //propis do sd�len�ch dat
            mapCustumeObject.mapObject.locationString = string.Format("{0}, {1}", vector2d.x.ToString(culture), vector2d.y.ToString(culture)); // .ToString(culture) proto�e pod�lanej c#


            float sceneHeight;//v��ka k zemi ve sc�n�
            float deltaHeight;

            if (isMinimap) // rozv�tven� dle manipulace skze minimapu nebo ve wordscale
            {
                var newTransformation = _map.GeoToWorldPosition(vector2d, true);

                // bari�ry maj� nastavenou v��ku tak aby spodn� hrana byla na zemi
                if (mapCustumeObject.mapObject.type == MapObject.ObjType.Barier || mapCustumeObject.mapObject.type == MapObject.ObjType.Warning)
                {
                    mapCustumeObject.mapObject.relativeAltitude = calcAbsoluteHeight(mapCustumeObject.spawnetGameObject.transform.localScale.y * 0.5f);
                }
                else // u ostatn�ch ur�uje v��ku u�ivatel
                {
                    sceneHeight = newTransformation.y;//v��ka k zemi ve sc�n�
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
                    sceneHeight = newTransformation.y;//v��ka k zemi ve sc�n�
                    deltaHeight = gameObject.transform.position.y - sceneHeight;
                    mapCustumeObject.mapObject.relativeAltitude = deltaHeight;
                }
            }

            // pokud je v��ka negativn�, klipne se do nuly
            if (mapCustumeObject.mapObject.relativeAltitude < 0)
                mapCustumeObject.mapObject.relativeAltitude = 0;


            if (isMinimap)
            {
                // ulo�en� scalu - v�znam pouze pro bari�ry
                mapCustumeObject.mapObject.scale = gameObject.transform.localScale / _barierMinimapScale;
            }
            else
                mapCustumeObject.mapObject.scale = gameObject.transform.localScale;
            //ulo�en� rotace - v�znam pouze pro bari�ry
            mapCustumeObject.mapObject.rotation = gameObject.transform.localRotation;

            // pokud ji� z objektem nen� manipulov�no, o�ist� se
            if (mapCustumeObject.manipulationDirtyFlag)
                mapCustumeObject.manipulationDirtyFlag = false; // zm�ny po manipulaci props�ny
        }

        // pokud se z objektem nemanipuluje, nastavujem pozici dle zd�len�ch dat
        if (!mapCustumeObject.underManipulation)
        {
            // logika v�po�tu pozice
            Vector2d vector2D = Conversions.StringToLatLon(mapCustumeObject.mapObject.locationString);

            if (mapCustumeObject.mapObject.type == MapObject.ObjType.Drone && !isMinimap) { } // dron m� ve wordscalu vlstn� vykreslovac� logiku - ned�l�m nic
            else
            {
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.position = _map.GeoToWorldPosition(vector2D, true); // pozice nastavena dle gps sou�adnic do v��ky 0m nad ter�nem
            }

            // bari�ry maj� vlstn� blok pro nastaven� m���tka objektu
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
                // ostatn� objekty maj� m���tko pevn� dle nastaven� konstanty
                gameObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                gameObject.transform.rotation = _map.transform.rotation;
            }

            //zde se nastav� v��ka objektu
            float calcHeight; 
            if (isMinimap)
            {  
                calcHeight = calcScenePosition(gameObject.transform.position.y, mapCustumeObject.mapObject.relativeAltitude);
            }
            else
            {   // v��ka nepot�ebuje p�epo�et
                calcHeight = gameObject.transform.position.y + mapCustumeObject.mapObject.relativeAltitude;
            }

            if(mapCustumeObject.mapObject.type==MapObject.ObjType.Drone && !isMinimap) // dron ve wordscalu pot�ebuje zn�t gps pozici dle gps pro dop�esn�n�
            {
                return;
            }
            else // pro ostatn� objekty pozici propisuji rovnou
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, calcHeight, gameObject.transform.position.z);

        }

        // vykresluj v minimap� pouze objekty v bounding boxu - ostan� skryj
        if (boxCollider == null || boxCollider.bounds.Contains(gameObject.transform.position) || mapCustumeObject.underManipulation)
        {
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);

        LabelTextSetter labelTextSetter = gameObject.GetComponent<LabelTextSetter>();
        //dodate�n� zm�ny pro ur�it� typy objekt�
        switch (mapCustumeObject.mapObject.type)
        {
            case MapObject.ObjType.Player:
                if (mapCustumeObject.mapObject is Player) // p�etypov�n�
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
                if (mapCustumeObject.mapObject is Waypoint) // p�etypov�n�
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


    //data pro vykreslen�
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


