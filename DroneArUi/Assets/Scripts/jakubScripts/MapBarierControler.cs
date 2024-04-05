using Mapbox.Unity.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBarierControler : Singleton<MapBarierControler>
{
    // Start is called before the first frame update

    [SerializeField]
    public bool dynamicBarierEnable=false;

    [SerializeField]
    List<AbstractMap> abstractMaps= new List<AbstractMap>();

    [SerializeField]
    float testAlt =0;
    [SerializeField]
    float startWidth = 3;

    float barierWidth = 3;

    DroneManager droneManager = null;
    void Start()
    {
        droneManager = DroneManager.Instance;
    }


    private float timer = 0f;
    private float interval = 0.5f; // Interval in seconds
    void Update()
    {
        timer += Time.deltaTime;

        // Check if the timer has reached the interval
        if (timer >= interval)
        {
            float newBarierWidth;
            if (dynamicBarierEnable)
            {
                float droneAlt;
                if (droneManager.ControlledDrone != null)
                {
                    droneAlt = (float)droneManager.ControlledDrone.FlightData.Altitude;
                }
                else
                {
                    droneAlt = testAlt;
                }
                newBarierWidth = Mathf.Round(startWidth + droneAlt);
            }
            else
            {
                newBarierWidth = startWidth;
            }

            if(newBarierWidth == barierWidth)
            {
                return;
            }

            foreach (AbstractMap map in abstractMaps)
            {
                if (map == null) { continue; }
                VectorSubLayerProperties vc = map.VectorData.FindFeatureSubLayerWithName("RedBarier-roads");
                if (vc == null)
                {
                    Debug.LogWarning("MapBarierControler l32");
                    continue;
                }
                if (vc.Modeling == null || vc.Modeling.LineOptions == null)
                {
                    Debug.LogWarning("MapBarierControler l40");
                    continue;
                }
                vc.Modeling.LineOptions.SetLineWidth(newBarierWidth);

            }

            // Reset the timer
            timer = 0f;
        }

    
       
    }
}
