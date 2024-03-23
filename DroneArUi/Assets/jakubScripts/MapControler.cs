using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox;
using Mapbox.Unity.Map;
using Microsoft.MixedReality.Toolkit.UI;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using Mapbox.Utils;
using Mapbox.Map;

public class MapControler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        setCurentCenter();

        abstractMap.OnMapRedrawn += onMapRedrawn;
        abstractMap.OnUpdated += onMapLoaded;
    }

    private void onMapRedrawn()
    {
        isLoaded = false;
    }
    private void onMapLoaded() {
        isLoaded=true;
    }


    public void setCurentCenter() {
        initCenter = abstractMap.CenterLatitudeLongitude;
    }
    // Update is called once per frame

    bool isLoaded=true;
    void Update()
    {
        if (isLoaded) // dopozicování minimapy do úrovnì stolu
        {
            Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude; // referenèní bod je støed mapy
            Vector3 wordPosOfCenter = abstractMap.GeoToWorldPosition(actualCenter, true);
            Vector3 tablePos = tableTransform.position;
            // rozdíl polohy stolu a mapy je odeèten on pozice
            mapTransform.localPosition = new Vector3(0, mapTransform.localPosition.y + (tablePos.y - wordPosOfCenter.y), 0);
        }
    }

    private Mapbox.Utils.Vector2d initCenter;
    public AbstractMap abstractMap = null;
    public Transform mapTransform = null;
    public Transform tableTransform=null;

    //private float mapInitY = -0.51f;
    private float sliderVal = 0.5f;
    private float zoom = 19f;

    private float moveStepSize = 0.0005f;

    public void setZoom(SliderEventData sliderEventData)
    {
        sliderVal = sliderEventData.NewValue;
        zoom = math.round(19f + sliderVal * 4f - 2f);

        try
        {
            abstractMap.UpdateMap(zoom);
        }
        catch { }

       
    }

    private float getStepSize() {
        return moveStepSize / mapTransform.localScale.x;
    }

    public void moveUp()
    {
        Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude;
        actualCenter.y = actualCenter.y - getStepSize();

        abstractMap.UpdateMap(actualCenter, zoom);
    }

    public void moveDown()
    {
        Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude;
        actualCenter.y = actualCenter.y + getStepSize();

        abstractMap.UpdateMap(actualCenter, zoom);
    }
    public void moveLeft()
    {
        Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude;
        actualCenter.x = actualCenter.x + getStepSize();

        abstractMap.UpdateMap(actualCenter, zoom);
    }
    public void moveRight()
    {
        Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude;
        actualCenter.x = actualCenter.x - getStepSize();

        abstractMap.UpdateMap(actualCenter, zoom);
    }
    public void center()
    {
        abstractMap.UpdateMap(initCenter, zoom);
        reloadMap();
    }

    public void reloadMap()
    {
        //Debug.Log("dawdaw");
        abstractMap.UpdateMap(14);
        abstractMap.UpdateMap(zoom);
    }

    public void changeMapImage(int index)
    {
        switch (index)
        {
            default:
            case 0:
                abstractMap.ImageLayer.SetLayerSource(ImagerySourceType.MapboxSatelliteStreet);
                break; 
            case 1:
                abstractMap.ImageLayer.SetLayerSource(ImagerySourceType.MapboxStreets);
                break; 
            case 2:
                abstractMap.ImageLayer.SetLayerSource(ImagerySourceType.MapboxDark);
                break;
        }
    }
}
