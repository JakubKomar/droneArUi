/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// zajištuje pravido 1:1 u vygenerovaných bariér nad silnicemi, nsatavuje šíøku bariéry dle letové výšky
/// </summary>

using Mapbox.Unity.Map;
using System.Collections.Generic;
using UnityEngine;

public class MapBarierControler : Singleton<MapBarierControler>
{
    [SerializeField]
    public bool dynamicBarierEnable = false;

    [SerializeField]
    List<AbstractMap> abstractMaps = new List<AbstractMap>();

    [SerializeField]
    float testAlt = 0;
    [SerializeField]
    float startWidth = 3;

    float barierWidth = 3;

    DroneManager droneManager = null;
    void Start()
    {
        droneManager = DroneManager.Instance;
    }


    private float timer = 0f;
    private float interval = 0.5f;
    void Update()
    {
        timer += Time.deltaTime;


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

            if (newBarierWidth == barierWidth)
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
                try
                {
                    vc.Modeling.LineOptions.SetLineWidth(newBarierWidth);
                }
                catch { }

            }


            timer = 0f;
        }
    }
}
