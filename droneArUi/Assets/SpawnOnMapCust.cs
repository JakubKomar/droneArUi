// author jakub kom�rek

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
using UnityEditor;

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

    // v hlavn�m sd�len�m modulu se zm�nily objekty - je nutn� je p�etvo�it
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
        if(mapCustumeObject.underManipulation) 
            return;

        GameObject gameObject = mapCustumeObject.spawnetGameObject;


        // pokud objekt nem� v map� fyzickou reprezataci, ud�lej novou
        if (gameObject == null) 
        {
            gameObject = Instantiate(_markerPrefab);
            mapCustumeObject.spawnetGameObject = gameObject;
        }

        // props�n� zm�n po manipulaci
        if (mapCustumeObject.manipulationDirtyFlag) // z objektem bylo manipulov�no - zm�ny je nutn� propsat
        {
            Vector2d vector2d= _map.WorldToGeoPosition(gameObject.transform.localPosition);

            CultureInfo culture = new CultureInfo("en-US");
            mapCustumeObject.mapObject.locationString = string.Format("{0}, {1}", vector2d.x.ToString(culture), vector2d.y.ToString(culture)); // .ToString(culture) proto�e pod�lanej c#
            if (isMinimap)
            {
                var newTransformation = _map.GeoToWorldPosition(vector2d, true);
                float sceneHeight = newTransformation.y;//v��ka k zemi ve sc�n�
                float deltaHeight = gameObject.transform.localPosition.y - sceneHeight;
                float calculatedHeight = calcAbsoluteHeight(deltaHeight); // might be wrong
                mapCustumeObject.mapObject.relativeAltitude = calculatedHeight;
            }
            else
            {
                mapCustumeObject.mapObject.relativeAltitude = gameObject.transform.localPosition.y;
            }

            mapCustumeObject.manipulationDirtyFlag = false; // zm�ny po manipulaci props�ny
        }


        // logika v�po�tu pozice
        Vector2d vector2D = Conversions.StringToLatLon(mapCustumeObject.mapObject.locationString);  

        gameObject.transform.localPosition = _map.GeoToWorldPosition(vector2D, true);

        float calcHeight;
        if (isMinimap)
        {   // aproxima�n� rovnice pro minimapu
            //mapCustumeObject.mapObject.relativeAltitude = 100;
            calcHeight = calcScenePosition(gameObject.transform.localPosition.y, mapCustumeObject.mapObject.relativeAltitude);
        }
        else
        {   // v��ka nepot�ebuje p�epo�et
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


    //data pro vykreslen�
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


