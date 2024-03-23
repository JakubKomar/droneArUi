using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StaticHudDataUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    private DroneManager droneManager;
    public TMP_Text altitute =null;
    public TMP_Text speed =null;
    public TMP_Text rotation = null;
    void Start()
    {
        droneManager = DroneManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        Drone myDrone = droneManager.ControlledDrone;
        
        if (myDrone == null) { return; }

        if(altitute != null) {
            altitute.text =  Math.Round((Decimal)myDrone.FlightData.Altitude, 1, MidpointRounding.AwayFromZero).ToString();
        }

        if (rotation != null) {
            double normalizeCompas = myDrone.FlightData.Compass;
            if(normalizeCompas < 0) {
                normalizeCompas=360 + normalizeCompas;
            }
            rotation.text = Math.Round((Decimal)normalizeCompas, 0, MidpointRounding.AwayFromZero).ToString();
        }

        if (speed != null)
        {
            double newSpeed = Math.Abs(myDrone.FlightData.VelocityX) + Math.Abs(myDrone.FlightData.VelocityY) + Math.Abs(myDrone.FlightData.VelocityZ);
            newSpeed = newSpeed * 3.6; // to km/h
            speed.text = Math.Round((Decimal)newSpeed, 0, MidpointRounding.AwayFromZero).ToString();
        }


    }
}
