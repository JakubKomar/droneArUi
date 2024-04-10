/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
///  tento skript vypo��t�v� pozici drona dle letov�ch dat, kombinuje data z gps, imu a predikuje n�sleduj�c� pohyb dronu, pokud nem� aktu�ln� data
/// </summary>

using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class DronePositionCalculator : MonoBehaviour
{
    [SerializeField]
    GameObject testImuGm;
    [SerializeField]
    GameObject testGpsGm;
    [SerializeField]
    GameObject dronePrefab;


    public float gpsWeight = 0.0f;
    public float imuWeight =1;
    public float imuGpsMixRate = 0.01f;

    [SerializeField]
    float imuAltMixRate = 0.07f;

    float lastUpdate = 0f;

    // GPS belive pozice
    private Vector3 gpsPosition = Vector3.zero;

    // imu belive pozice
    private Vector3 imuPosition = Vector3.zero;

    // posledn� data o akceleraci
    private Vector3 aceleration = Vector3.zero;

    private DroneManager droneManager;
    public bool debugMode = false;

    private ToggleWordscaleDrone toggleWordscaleDrone;
    private AbstractMap map;
    DronePositionCalculator()
    {
        debugMode = false;
    }
    void Start()
    {
        droneManager=DroneManager.Instance;

        calibrationScript cls = calibrationScript.Instance;
        cls.calibrationEvent.AddListener(onCalibration);
        map = this.transform.parent.GetComponent<AbstractMap>();

        if (testGpsGm)
            testGpsGm.transform.parent = this.transform.parent;
        if (testImuGm)
            testImuGm.transform.parent = this.transform.parent;

        toggleWordscaleDrone=ToggleWordscaleDrone.Instance;

    }

    void calcGps()
    {
        if (droneManager.ControlledDrone == null || droneManager.ControlledDrone.FlightData == null) // pokud nem�m letov� data ned�l�m nic
            return;

        // pozice je spo�tena pouze na ne�isto
        Vector3 droneTransform = map.GeoToWorldPosition(new Vector2d(droneManager.ControlledDrone.FlightData.Latitude, droneManager.ControlledDrone.FlightData.Longitude), true); // spo�ti pozici pro drona
                                                                                                                                                                                   // v��ka odvozena z letov�ch dat
        float calcHeight = droneTransform.y + (float)(droneManager.ControlledDrone.FlightData.Altitude); //v��ka je br�na z letov�ch dat

        Vector3 gpsPos = new Vector3(droneTransform.x, calcHeight, droneTransform.z);

        // spo�ten� lok�ln� pozice - skript pracuje pouze s lok�ln�mi pozicemi
        gpsPosition= map.transform.InverseTransformPoint(gpsPos);
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
            calcGps();
            ImuCalcPosition();

            Vector3 posGpsforCal = gpsPosition;

            if (gpsWeight == 0)
            {
                this.transform.localPosition = imuPosition;

                if (debugMode|| droneManager.ControlledDrone.FlightData.InvalidGps) // pokud dron nem� validn� gps pozici vych�z� podle imu
                {
                    posGpsforCal.x = imuPosition.x;
                    posGpsforCal.z = imuPosition.z;
                }
            }
            else
            {
                if (droneManager.ControlledDrone.FlightData.InvalidGps) // pokud dron nem� validn� gps pozici vych�z� podle imu
                {
                    posGpsforCal.x = imuPosition.x;
                    posGpsforCal.z = imuPosition.z;
                }
                this.transform.localPosition = posGpsforCal * gpsWeight + imuPosition * imuWeight; // zmixov�n� obou pozic
            }

            droneManager.ControlledDrone.usedForCalculation = true; //data ji� byly u�ity pro update
            lastUpdate = 0;

            CompassIndicator compasIndicator = FindObjectOfType<CompassIndicator>();
            if (compasIndicator != null)
                compasIndicator.drone = gameObject;

            DynamicHudRotationSetter dynamicHudRotationSetter = FindObjectOfType<DynamicHudRotationSetter>();
            if (dynamicHudRotationSetter != null)
                dynamicHudRotationSetter.droneWordScale = gameObject;

            if (!debugMode) {
                return;            
            }
            if(testGpsGm)
                testGpsGm.transform.localPosition = posGpsforCal;
            if(testImuGm)
                testImuGm.transform.localPosition=  imuPosition;
        }
    }

    void ImuCalcPosition()
    {
        Vector3 actualPosition;
        Vector3 corectionPosition= gpsPosition; 

        if (droneManager.ControlledDrone.FlightData.InvalidGps ) // pokud je navalidni gps koriguje se pouze v��ka
        {
            corectionPosition = imuPosition;
            corectionPosition.y=gpsPosition.y;
        }

        // pro zaji�t�n� konvergence se pozice imu dop�esnuje podle gps a to v pom�ru dan�m paremetry, v��ka je pot�eba dop�es�ovat agresivn�ji ne� pozice

        actualPosition.x = corectionPosition.x * imuGpsMixRate + imuPosition.x * (1f - imuGpsMixRate);
        actualPosition.z = corectionPosition.z * imuGpsMixRate + imuPosition.z * (1f - imuGpsMixRate);
        actualPosition.y = corectionPosition.y * imuAltMixRate + imuPosition.y * (1f - imuAltMixRate);

        // vectory jsou u dji p�eh�zen� oproti unity
        aceleration.x = (float)droneManager.ControlledDrone.FlightData.VelocityY;
        aceleration.y = -(float)droneManager.ControlledDrone.FlightData.VelocityZ;
        aceleration.z =(float)droneManager.ControlledDrone.FlightData.VelocityX;
        
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
