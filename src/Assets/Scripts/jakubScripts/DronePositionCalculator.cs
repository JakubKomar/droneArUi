/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
///  tento skript vypoèítává pozici drona dle letových dat, kombinuje data z gps, imu a predikuje následující pohyb dronu, pokud nemá aktuální data
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

    // poslední data o akceleraci
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
        if (droneManager.ControlledDrone == null || droneManager.ControlledDrone.FlightData == null) // pokud nemám letová data nedìlám nic
            return;

        // pozice je spoètena pouze na neèisto
        Vector3 droneTransform = map.GeoToWorldPosition(new Vector2d(droneManager.ControlledDrone.FlightData.Latitude, droneManager.ControlledDrone.FlightData.Longitude), true); // spoèti pozici pro drona
                                                                                                                                                                                   // výška odvozena z letových dat
        float calcHeight = droneTransform.y + (float)(droneManager.ControlledDrone.FlightData.Altitude); //výška je brána z letových dat

        Vector3 gpsPos = new Vector3(droneTransform.x, calcHeight, droneTransform.z);

        // spoètení lokální pozice - skript pracuje pouze s lokálními pozicemi
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
            calcGps();
            ImuCalcPosition();

            Vector3 posGpsforCal = gpsPosition;

            if (gpsWeight == 0)
            {
                this.transform.localPosition = imuPosition;

                if (debugMode|| droneManager.ControlledDrone.FlightData.InvalidGps) // pokud dron nemá validní gps pozici vychází podle imu
                {
                    posGpsforCal.x = imuPosition.x;
                    posGpsforCal.z = imuPosition.z;
                }
            }
            else
            {
                if (droneManager.ControlledDrone.FlightData.InvalidGps) // pokud dron nemá validní gps pozici vychází podle imu
                {
                    posGpsforCal.x = imuPosition.x;
                    posGpsforCal.z = imuPosition.z;
                }
                this.transform.localPosition = posGpsforCal * gpsWeight + imuPosition * imuWeight; // zmixování obou pozic
            }

            droneManager.ControlledDrone.usedForCalculation = true; //data již byly užity pro update
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

        if (droneManager.ControlledDrone.FlightData.InvalidGps ) // pokud je navalidni gps koriguje se pouze výška
        {
            corectionPosition = imuPosition;
            corectionPosition.y=gpsPosition.y;
        }

        // pro zajištìní konvergence se pozice imu dopøesnuje podle gps a to v pomìru daném paremetry, výška je potøeba dopøesòovat agresivnìji než pozice

        actualPosition.x = corectionPosition.x * imuGpsMixRate + imuPosition.x * (1f - imuGpsMixRate);
        actualPosition.z = corectionPosition.z * imuGpsMixRate + imuPosition.z * (1f - imuGpsMixRate);
        actualPosition.y = corectionPosition.y * imuAltMixRate + imuPosition.y * (1f - imuAltMixRate);

        // vectory jsou u dji pøeházené oproti unity
        aceleration.x = (float)droneManager.ControlledDrone.FlightData.VelocityY;
        aceleration.y = -(float)droneManager.ControlledDrone.FlightData.VelocityZ;
        aceleration.z =(float)droneManager.ControlledDrone.FlightData.VelocityX;
        
        imuPosition = actualPosition + aceleration * lastUpdate; // nová pozice je poèítána dle pøedchozí + pøíbytek dle rychlosti a èasu podle pøechozího updatu
    }
    void onCalibration()
    {
        // pøi kalibraci je nutné imu pozici pøepsat 
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
