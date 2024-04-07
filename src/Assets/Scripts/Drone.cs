/*
 * Drone - class to store all drone data necessary to rendering
 * 
 * Author : Martin Kyjac (xkyjac00)
 *          Jakub Komárek
 */

using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Drone {


    public DroneFlightData FlightData {
        get; set;
    }

    public bool IsControlled {
        get; set;
    }

    public DateTime lastUpdate;

    private readonly IEnumerable<string> DronesWithZeroAltitude = new List<string> { "DJI-Mavic2", "DJI-MAVIC_PRO", "DJI-MAVIC_MINI" };

    public bool usedForCalculation=false;

    public float RelativeAltitude {
        get
        {

            return (float)FlightData.Altitude;
        }
    }

    public Drone(DroneFlightData flightData, bool isControlled=false, float rotationOffset=0) {
        FlightData = flightData;
        IsControlled = isControlled;
        lastUpdate= DateTime.Now;
        usedForCalculation = false;
    }

    public void UpdateDroneFlightData(DroneFlightData flightData) {
        FlightData = flightData ;
        lastUpdate = DateTime.Now;
        usedForCalculation = false;
    }


    private float GetRelativeAltitude()
    {
        return (float)FlightData.Altitude - 1.8f; 

    }
}
