using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugSliders : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshProUGUI m_TextMeshPro;
    void Start()
    {
        
    }

    // Update is called once per frame
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
