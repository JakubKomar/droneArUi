/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
///  tento skript vypo��t�v� pozici drona dle letov�ch dat, kombinuje data z gps, imu a predikuje n�sleduj�c� pohyb dronu, pokud nem� aktu�ln� data
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePositionCalculator : MonoBehaviour
{
    [SerializeField]
    GameObject testImuGm;
    [SerializeField]
    GameObject testGpsGm;
    [SerializeField]
    GameObject dronePrefab;


    [SerializeField]
    public float gpsWeight = 0.0f;
    [SerializeField]
    public float imuWeight =1;
    [SerializeField]
    public float imuGpsMixRate = 0.01f;

    [SerializeField]
    float imuAltMixRate = 0.07f;

    float lastUpdate = 0f;

    // GPS belive pozice
    //[HideInInspector]
    public Vector3 gpsPosition = Vector3.zero;

    // imu belive pozice
    //[HideInInspector]
    public Vector3 imuPosition = Vector3.zero;

    // posledn� data o akceleraci
    //[HideInInspector]
    public Vector3 aceleration = Vector3.zero;

    private DroneManager droneManager;

    [SerializeField]
    public bool debugMode = false;

    private ToggleWordscaleDrone toggleWordscaleDrone;

    DronePositionCalculator()
    {
        debugMode = false;
    }
    void Start()
    {
        droneManager=DroneManager.Instance;

        calibrationScript cls = calibrationScript.Instance;
        cls.calibrationEvent.AddListener(onCalibration);


        if (testGpsGm)
            testGpsGm.transform.parent = this.transform.parent;
        if (testImuGm)
            testImuGm.transform.parent = this.transform.parent;

        toggleWordscaleDrone=ToggleWordscaleDrone.Instance;

    }

    void Update()
    {
        debugMode = !toggleWordscaleDrone.droneWordscaleDebug;

        if (testGpsGm)
            testGpsGm.SetActive(debugMode);
        if (testImuGm)
            testImuGm.SetActive(debugMode);
        if (dronePrefab)
            dronePrefab.SetActive(debugMode);

        if (droneManager.ControlledDrone == null ) // bez letov�ch dat nemohu d�lat nic
        {
            lastUpdate = 0;
        }
        else if (droneManager.ControlledDrone == null && droneManager.ControlledDrone.usedForCalculation) // pokud nem�me data pozice se predikuje dle p�edchoz�ch dat
        {
            lastUpdate += Time.deltaTime;
            this.transform.localPosition = aceleration * Time.deltaTime; // predikce dle p�edchoz� velocity
        }
        else
        {
            lastUpdate += Time.deltaTime;
            ImuCalcPosition();

            Vector3 posGpsforCal = gpsPosition;

            if (droneManager.ControlledDrone.FlightData.InvalidGps ) // pokud dron nem� validn� gps pozici vych�z� podle imu
            {
                posGpsforCal.x=imuPosition.x;
                posGpsforCal.z=imuPosition.z;
            }
            if (lastUpdate > 1.2) // pokud update byl proveden pozd�ji ne� do 1.2 sekundy, pozice se bere z gps
            {
                this.transform.localPosition = gpsPosition;
                imuPosition = gpsPosition;
            }

            this.transform.localPosition = posGpsforCal * gpsWeight + imuPosition * imuWeight; // zmixov�n� obou pozic
            

            droneManager.ControlledDrone.usedForCalculation = true; //data ji� byly u�ity pro update
            lastUpdate = 0;

            if(testGpsGm)
            testGpsGm.transform.localPosition = posGpsforCal;
            if(testImuGm)
            testImuGm.transform.localPosition=  imuPosition;
        }
    }

    void ImuCalcPosition()
    {
        Vector3 actualPosition;
        Vector3 corectionPosition; 

        if (droneManager.ControlledDrone.FlightData.InvalidGps ) // pokud je navalidni gps koriguje se pouze v��ka
        {
            corectionPosition = imuPosition;
            corectionPosition.y=gpsPosition.y;
        }
        else
            corectionPosition= gpsPosition;

        // pro zaji�t�n� konvergence se pozice imu dop�esnuje podle gps a to v pom�ru dan�m paremetry, v��ka je pot�eba dop�es�ovat agresivn�ji ne� pozice
        actualPosition.x = corectionPosition.x * imuGpsMixRate + imuPosition.x * (1f - imuGpsMixRate);
        actualPosition.z = corectionPosition.z * imuGpsMixRate + imuPosition.z * (1f - imuGpsMixRate);
        actualPosition.y = corectionPosition.y * imuAltMixRate + imuPosition.y * (1f - imuAltMixRate);
        // vectory jsou u dji p�eh�zen� oproti unity
        aceleration = new Vector3((float)droneManager.ControlledDrone.FlightData.VelocityY, -(float)droneManager.ControlledDrone.FlightData.VelocityZ, (float)droneManager.ControlledDrone.FlightData.VelocityX);
        
        imuPosition = actualPosition + aceleration * lastUpdate; // nov� pozice je po��t�na dle p�edchoz� + p��bytek dle rychlosti a �asu podle p�echoz�ho updatu
    }
    void onCalibration()
    {
        // p�i kalibraci je nutn� imu pozici p�epsat 
        if (droneManager.ControlledDrone == null || droneManager.ControlledDrone.FlightData == null || droneManager.ControlledDrone.FlightData.InvalidGps)
            imuPosition = Vector3.zero;
        else
            imuPosition = gpsPosition;
    }

    public void setMixRate(float imuMixRate)
    {
        if (imuAltMixRate > 1)
        {
            Debug.LogError("mixrate invalid");
            return;
        }
        gpsWeight = 1f - imuMixRate;
        imuWeight = imuMixRate;
    }
    public void setCorrectionRate(float corr)
    {
        if (corr > 1)
        {
            Debug.LogError("correction invalid");
            return;
        }
        imuGpsMixRate =corr;
    }
}