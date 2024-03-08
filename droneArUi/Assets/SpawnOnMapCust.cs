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


    private List<MapObjectData> allMapObjects = new List<MapObjectData>();

    private MapData mapData = null;


    [SerializeField]
    float _spawnScale = 100f;

    [SerializeField]
    GameObject _markerPrefab;


    public BoxCollider boxCollider = null;

    ~SpawnOnMap()
    {
    }
    void Start()
    {
        mapData= FindObjectOfType<MapData>();

    }

    public void reCreateGameObjects() { 
        
    }

    private void renderObject(MapObjectData mapCustumeObject)
    {/*
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




        if (boxCollider==null|| boxCollider.bounds.Contains(instance.transform.localPosition))
        { // if obejct is in boundig box, show it
            instance.SetActive(true);
        }
        else
            instance.SetActive(false);+*/
    }

    private void Update()
    {/*
        foreach (var mapGameObject in allMapObjects)
        {
            try
            {
                renderObject(mapGameObject);
            }
            catch (Exception e)
            {
            }
        }*/
    }

    private float calcScenePosition(float groundYpos, float relativeAlt)
    {
        const float tiltScaleUnity = 0.115f;
        const float defalutZoomLevel = 19;
        const float aproximateConstant = 0.25f;
        return groundYpos + (relativeAlt / defalutZoomLevel) * tiltScaleUnity * _map.transform.lossyScale.y * aproximateConstant;

    }

    //data pro vykreslení
    [Serializable]
    public class MapObjectData
    {
        [JsonIgnore]
        [HideInInspector]
        public GameObject spawnetGameObject = null;

        [JsonIgnore] 
        [HideInInspector]
        public Vector2d vector2D;
    }
}


