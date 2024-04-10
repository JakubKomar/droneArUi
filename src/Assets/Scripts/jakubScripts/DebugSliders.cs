/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// setter pro ladìní parametrù sledování drona
/// </summary>


using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugSliders : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI m_TextMeshPro;
    void Start()
    {
        
    }

    void Update()
    {
        DronePositionCalculator scriptInstance = FindObjectOfType<DronePositionCalculator>();
        if (scriptInstance&& m_TextMeshPro)
        {
            m_TextMeshPro.text = "GPSw:" + scriptInstance.gpsWeight + " IMUw:" + scriptInstance.imuWeight + " Corr:" + scriptInstance.imuGpsMixRate;
        }
    }

    public void onMixerChanged(SliderEventData sl) {
        DronePositionCalculator scriptInstance = FindObjectOfType<DronePositionCalculator>();
        if (scriptInstance )
       scriptInstance.setMixRate(sl.NewValue);
    }

    public void onCorectionChanged(SliderEventData sl) {
        DronePositionCalculator scriptInstance = FindObjectOfType<DronePositionCalculator>();
        if (scriptInstance)
            scriptInstance.setCorrectionRate(sl.NewValue/10);
    }
}
