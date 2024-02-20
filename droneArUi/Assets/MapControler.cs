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
        //mapInitY = mapTransform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        float val = 19f- zoom;

        float scaleIndex = val;

        float newY = -0.434f;
        if (val < 1 && val > -1) { 


        }

        else 
        {
            newY = newY / math.pow(2, scaleIndex);
        }
 
        mapTransform.localPosition = new Vector3(0, -0.44f * mapTransform.localScale.y, 0);
  
    }
    public AbstractMap abstractMap = null;
    public PinchSlider pinchSlider = null;
    public Transform mapTransform = null;
    private float mapInitY = -0.44f;
    private float sliderVal = 0.5f;
    private float zoom = 19f;

    public void setZoom()
    {
        sliderVal = pinchSlider.SliderValue;
        zoom = 19f + math.round((sliderVal * 6f) - 3f);

        abstractMap.UpdateMap(zoom);
    }
}
