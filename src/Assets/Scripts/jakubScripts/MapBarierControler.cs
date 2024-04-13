/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// zaji�tuje pravido 1:1 u vygenerovan�ch bari�r nad silnicemi, nsatavuje ���ku bari�ry dle letov� v��ky
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
using System.Collections;
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
    void Awake()
    {
        droneManager = DroneManager.Instance;
        StartCoroutine(UpdateAsync());
    }
    /*
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
    }*/
    private IEnumerator barierResize()
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
            yield break;
        }
        yield return null;
        foreach (AbstractMap map in abstractMaps)
        {
            if (map == null) { continue; }
            VectorSubLayerProperties vc = map.VectorData.FindFeatureSubLayerWithName("RedBarier-roads");
            if (vc == null)
            {
                Debug.LogWarning("MapBarierControler vc == null");
                continue;
            }
            if (vc.Modeling == null || vc.Modeling.LineOptions == null)
            {
                Debug.LogWarning("MapBarierControler vc.Modeling == null || vc.Modeling.LineOptions == null");
                continue;
            }
            try
            {
                vc.Modeling.LineOptions.SetLineWidth(newBarierWidth);
            }
            catch { }
            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator UpdateAsync()
    {
        while (true)
        {
            yield return barierResize();
            yield return new WaitForSeconds(0.25f);
        }
    }
}
