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

public class SpawnOnMap : MonoBehaviour
{
    [SerializeField]
    AbstractMap _map;

    [SerializeField]
    MapCustumeObject[] _locationStrings;
        

    Vector2d[] _locations;

    [SerializeField]
    float _spawnScale = 100f;

    [SerializeField]
    GameObject _markerPrefab;

    List<GameObject> _spawnedObjects;


    public BoxCollider boxCollider = null;

    void Start()
    {
        _locations = new Vector2d[_locationStrings.Length];
        _spawnedObjects = new List<GameObject>();
        for (int i = 0; i < _locationStrings.Length; i++)
        {
            var locationString = _locationStrings[i];
            _locations[i] = Conversions.StringToLatLon(locationString.locationString);
            GameObject instance = Instantiate(_markerPrefab);
            instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
            instance.transform.localPosition=new Vector3(instance.transform.localPosition.x, instance.transform.localPosition.y+locationString.relativeAltitude, instance.transform.localPosition.z);

            instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

            LabelTextSetter labelTextSetter = instance.GetComponent<LabelTextSetter>();
            labelTextSetter.Set(new Dictionary<String, object> {{ "name", locationString.name },} );
            _spawnedObjects.Add(instance);
        }
    }

    private void Update()
    {
        int count = _spawnedObjects.Count;
        for (int i = 0; i < count; i++)
        {
            var spawnedObject = _spawnedObjects[i];
            var location = _locations[i];
            var locationString = _locationStrings[i];
            spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);


            float scaleFactor = Mathf.Pow(2, _map.Zoom);

            const float tiltScaleUnity = 0.115f;
            const float defalutZoomLevel = 19;
            const float aproximateConstant = 0.25f;
            float calcHeight = spawnedObject.transform.localPosition.y + (locationString.relativeAltitude/ defalutZoomLevel) * tiltScaleUnity * _map.transform.lossyScale.y * aproximateConstant;

            spawnedObject.transform.localPosition = new Vector3(spawnedObject.transform.localPosition.x, calcHeight, spawnedObject.transform.localPosition.z);
            spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

            if (boxCollider.bounds.Contains(spawnedObject.transform.localPosition)){
                spawnedObject.SetActive(true);
            }
            else
                spawnedObject.SetActive(false);
        }
    }
}

[Serializable]
public class MapCustumeObject
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

}
