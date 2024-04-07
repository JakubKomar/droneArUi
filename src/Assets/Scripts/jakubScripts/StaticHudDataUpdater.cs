/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Updatu statická data hudu, bateria a letový mód je simulován
/// </summary>

using System;
using TMPro;
using UnityEngine;


public class StaticHudDataUpdater : MonoBehaviour
{
    private DroneManager droneManager;

    [SerializeField]
    TMP_Text altitute = null;
    [SerializeField]
    TMP_Text speed = null;

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

    [SerializeField]
    GameObject droneLostIcon=null;
    [SerializeField]
    GameObject gpsLostIcon = null;
    [SerializeField]
    GameObject altWarningIcon = null;

    void Start()
    {
        droneManager = DroneManager.Instance;
        playerCamera = Camera.main.transform;
    }


    private float batterySimulationTimeStamp = 0;
    private float batteryPercetInterval = 15;
    void Update()
    {

        if (wordMapbox != null && playerCamera != null)
        {
            playerHading = playerCamera.rotation.eulerAngles.y - wordMapbox.rotation.eulerAngles.y;
            playerHadingOffset = -wordMapbox.rotation.eulerAngles.y;
            playerHading = playerHading % 360;
            if (playerHading < 0)
                playerHading += 360;
            playerHading = Mathf.Round(playerHading);
            rotation.text = playerHading.ToString();
        }
        Drone myDrone = droneManager.ControlledDrone;
        if (time != null)
            time.text = DateTime.Now.ToString("HH:mm");


        if (myDrone == null)
        {
            //batteryLevel = 99;
            if (droneLostIcon != null)
                droneLostIcon.SetActive(true);
            return;
        }
        if (droneLostIcon != null)
            droneLostIcon.SetActive(false);
        if(altWarningIcon!=null)
            altWarningIcon.SetActive(myDrone.FlightData.Altitude > 100);
        if (gpsLostIcon != null)
            gpsLostIcon.SetActive(myDrone.FlightData.InvalidGps);

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
        if (battery != null)
            battery.text = batteryLevel.ToString();

        if (cameraAngl != null)
            cameraAngl.text = Math.Round(myDrone.FlightData.gimbalOrientation.pitch).ToString() + "°";
        if (flyMod != null)
            flyMod.text = droneMod.ToString();

    }

    enum flyModEnum
    {
        position,
        cinematic,
        sport
    }
}
