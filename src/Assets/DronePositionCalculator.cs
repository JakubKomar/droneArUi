/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
///  tento skript vypoèítává pozici drona dle letových dat, kombinuje data z gps, imu a predikuje následující pohyb dronu, pokud nemá aktuální data
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePositionCalculator : MonoBehaviour
{   
    [SerializeField]
    float gpsWeight = 0.0f;
    [SerializeField]
    float imuWeight =1;
    [SerializeField]
    float imuGpsMixRate = 0.01f;

    [SerializeField]
    float imuAltMixRate = 0.07f;

    float lastUpdate = 0f;

    // GPS belive pozice
    //[HideInInspector]
    public Vector3 gpsPosition = Vector3.zero;

    // imu belive pozice
    //[HideInInspector]
    public Vector3 imuPosition = Vector3.zero;

    // poslední data o akceleraci
    //[HideInInspector]
    public Vector3 aceleration = Vector3.zero;

    private DroneManager droneManager;
    [SerializeField]
    bool testFakePosition = false;
    void Start()
    {
        droneManager=DroneManager.Instance;

        calibrationScript cls = calibrationScript.Instance;
        cls.calibrationEvent.AddListener(onCalibration);
    }

    void Update()
    {
        if (droneManager.ControlledDrone == null ) // bez letových dat nemohu dìlat nic
        {
            lastUpdate = 0;
        }
        else if (droneManager.ControlledDrone == null && droneManager.ControlledDrone.usedForCalculation) // pokud nemáme data pozice se predikuje dle pøedchozích dat
        {
            lastUpdate += Time.deltaTime;
            this.transform.localPosition = aceleration * Time.deltaTime; // predikce dle pøedchozí velocity
        }
        else
        {
            lastUpdate += Time.deltaTime;
            ImuCalcPosition();

            Vector3 posGpsforCal = gpsPosition;

            if (droneManager.ControlledDrone.FlightData.InvalidGps &&!testFakePosition) // pokud dron nemá validní gps pozici vychází podle imu
            {
                posGpsforCal.x=imuPosition.x;
                posGpsforCal.z=imuPosition.z;
            }
            if (lastUpdate > 1.2) // pokud update byl proveden pozdìji než do 1.2 sekundy, pozice se bere z gps
            {
                this.transform.localPosition = gpsPosition;
                imuPosition = gpsPosition;
            }

            this.transform.localPosition = gpsPosition * gpsWeight + imuPosition * imuWeight; // zmixování obou pozic
            

            droneManager.ControlledDrone.usedForCalculation = true; //data již byly užity pro update
            lastUpdate = 0;
        }
    }

    void ImuCalcPosition()
    {
        Vector3 actualPosition;
        Vector3 corectionPosition; 

        if (droneManager.ControlledDrone.FlightData.InvalidGps && !testFakePosition) // pokud je navalidni gps koriguje se pouze výška
        {
            corectionPosition = imuPosition;
            corectionPosition.y=gpsPosition.y;
        }
        else
            corectionPosition= gpsPosition;

        // pro zajištìní konvergence se pozice imu dopøesnuje podle gps a to v pomìru daném paremetry, výška je potøeba dopøesòovat agresivnìji než pozice
        actualPosition.x = corectionPosition.x * imuGpsMixRate + imuPosition.x * (1f - imuGpsMixRate);
        actualPosition.z = corectionPosition.z * imuGpsMixRate + imuPosition.z * (1f - imuGpsMixRate);
        actualPosition.y = corectionPosition.y * imuAltMixRate + imuPosition.y * (1f - imuAltMixRate);
        // vectory jsou u dji pøeházené oproti unity
        aceleration = new Vector3((float)droneManager.ControlledDrone.FlightData.VelocityY, -(float)droneManager.ControlledDrone.FlightData.VelocityZ, (float)droneManager.ControlledDrone.FlightData.VelocityX);
        
        imuPosition = actualPosition + aceleration * lastUpdate; // nová pozice je poèítána dle pøedchozí + pøíbytek dle rychlosti a èasu podle pøechozího updatu
    }
    void onCalibration()
    {
        // pøi kalibraci je nutné imu pozici pøepsat 
        if (droneManager.ControlledDrone.FlightData == null || droneManager.ControlledDrone.FlightData.InvalidGps)
            imuPosition = Vector3.zero;
        else
            imuPosition = gpsPosition;
    }
}
