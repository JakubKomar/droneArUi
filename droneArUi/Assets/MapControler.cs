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

public class MapControler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        initCenter = abstractMap.CenterLatitudeLongitude;
        //mapInitY = mapTransform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
 
        mapTransform.localPosition = new Vector3(0, mapInitY * mapTransform.localScale.y, 0);
  
    }

    private Mapbox.Utils.Vector2d initCenter;
    public AbstractMap abstractMap = null;
    public PinchSlider pinchSlider = null;
    public Transform mapTransform = null;
    private float mapInitY = -0.51f;
    private float sliderVal = 0.5f;
    private float zoom = 19f;

    private float moveStepSize = 0.0005f;

    public void setZoom()
    {
        sliderVal = pinchSlider.SliderValue;
        zoom = math.round(19f + sliderVal * 4f - 2f);

        abstractMap.UpdateMap(zoom);
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
    }
}
