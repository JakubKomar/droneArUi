// jakub komárek

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

    [SerializeField]
    TMP_Text altitute =null;
    [SerializeField]
    TMP_Text speed =null;

    [SerializeField]
    TMP_Text rotation = null;

    [SerializeField]
    TMP_Text battery = null;

    [SerializeField]
    TMP_Text cameraAngl = null;

    [SerializeField]
    TMP_Text time = null;

    [SerializeField]
    TMP_Text flyMod = null;

    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private Transform wordMapbox;
    public float playerHading = 0;
    public float playerHadingOffset = 0;

    [SerializeField]
    int batteryLevel = 99;

    [SerializeField]
    flyModEnum droneMod = flyModEnum.position;
    void Start()
    {
        droneManager = DroneManager.Instance;
    }

    // Update is called once per frame

    private float batterySimulationTimeStamp=0;
    private float batteryPercetInterval = 15;
    void Update()
    {
        /*if (rotation != null) {
             double normalizeCompas = myDrone.FlightData.Compass;
             if(normalizeCompas < 0) {
                 normalizeCompas=360 + normalizeCompas;
             }
             rotation.text = Math.Round((Decimal)normalizeCompas, 0, MidpointRounding.AwayFromZero).ToString();
         }*/

        playerHading=playerCamera.rotation.eulerAngles.y - wordMapbox.rotation.eulerAngles.y;
        playerHadingOffset = -wordMapbox.rotation.eulerAngles.y;
        playerHading = playerHading % 360;
        if (playerHading < 0)
            playerHading += 360;
        playerHading=Mathf.Round(playerHading);
        rotation.text=playerHading.ToString();

        Drone myDrone = droneManager.ControlledDrone;
        time.text = DateTime.Now.ToString("HH:mm");

        if (myDrone == null) {
            //batteryLevel = 99;
            return; 
        }

        if (altitute != null)
        {
            altitute.text = Math.Round((Decimal)myDrone.FlightData.Altitude, 1, MidpointRounding.AwayFromZero).ToString();
        }

        if (speed != null)
        {
            double newSpeed = Math.Abs(myDrone.FlightData.VelocityX) + Math.Abs(myDrone.FlightData.VelocityY) + Math.Abs(myDrone.FlightData.VelocityZ);
            newSpeed = newSpeed * 3.6; // to km/h
            speed.text = Math.Round((Decimal)newSpeed, 0, MidpointRounding.AwayFromZero).ToString();
        }

        //simulace spotøeby baterie
        if (Time.time - batterySimulationTimeStamp > batteryPercetInterval)
        {
            batteryLevel--;
            batterySimulationTimeStamp = Time.time;
        }
        battery.text = batteryLevel.ToString();


        cameraAngl.text =Math.Round( myDrone.FlightData.gimbalOrientation.pitch).ToString()+ "°";
        
        flyMod.text= droneMod.ToString();

    }

    enum flyModEnum
    {
        position,
        cinematic,
        sport
    }
}
