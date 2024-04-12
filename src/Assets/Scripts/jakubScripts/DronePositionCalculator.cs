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
using System.Collections;
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
    public float height = 0;
    private float baseHeight = 0;

    [SerializeField]
    float imuAltMixRate = 0.07f;

    float lastUpdate = 0f;

    // GPS belive pozice
    [SerializeField]
    private Vector3 gpsPosition = Vector3.zero;

    // imu belive pozice
    [SerializeField]
    private Vector3 imuPosition = Vector3.zero;

    // poslední data o akceleraci
    [SerializeField]
    private Vector3 aceleration = Vector3.zero;

    [SerializeField]
    private Vector3 posGpsforCal = Vector3.zero;

    private DroneManager droneManager;
    
    private bool _debugMode;
    public bool debugMode
    {
        get { return _debugMode; }
        set
        {
            if (_debugMode == value) { return; }
            _debugMode = value;
            if (testGpsGm)
                testGpsGm.SetActive(_debugMode);
            if (testImuGm)
                testImuGm.SetActive(_debugMode);
            if (dronePrefab)
                dronePrefab.SetActive(_debugMode);
        }
    }

    private ToggleWordscaleDrone toggleWordscaleDrone;
    private AbstractMap map;
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

        debugMode = false;
        if (testGpsGm)
            testGpsGm.SetActive(false);
        if (testImuGm)
            testImuGm.SetActive(false);
        if (dronePrefab)
            dronePrefab.SetActive(false);

        toggleWordscaleDrone =ToggleWordscaleDrone.Instance;

        CompassIndicator compasIndicator = FindObjectOfType<CompassIndicator>();
        if (compasIndicator != null)
            compasIndicator.drone = gameObject;

    }

    private void OnDestroy()
    {
        if (testImuGm)        
            Destroy(testImuGm);
        
        if(testGpsGm)
            Destroy(testGpsGm);
    }

    void calcGps()
    {
        if (droneManager.ControlledDrone == null || droneManager.ControlledDrone.FlightData == null) // pokud nemám letová data nedìlám nic
            return;

        // pozice je spoètena pouze na neèisto
        Vector3 droneTransform = map.GeoToWorldPosition(new Vector2d(droneManager.ControlledDrone.FlightData.Latitude, droneManager.ControlledDrone.FlightData.Longitude), true); // spoèti pozici pro drona
        baseHeight = droneTransform.y;
        height = (float)(droneManager.ControlledDrone.FlightData.Altitude);
        float calcHeight = baseHeight + height; //výška je brána z letových dat
        
        Vector3 gpsPos = new Vector3(droneTransform.x, calcHeight, droneTransform.z);

        // spoètení lokální pozice - skript pracuje pouze s lokálními pozicemi
        gpsPosition= map.transform.InverseTransformPoint(gpsPos);
    }

    void Update()
    {
        debugMode = !toggleWordscaleDrone.droneWordscaleDebug;

        if (droneManager.ControlledDrone == null ) // bez letových dat nemohu dìlat nic
        {
            lastUpdate = 0;
        }
        else if (droneManager.ControlledDrone.usedForCalculation) // pokud nemáme data pozice se predikuje dle pøedchozích dat
        {
            lastUpdate += Time.deltaTime;
            Vector3 newPos = this.transform.localPosition + aceleration * Time.deltaTime; // predikce dle pøedchozí velocity
            if (newPos.y < 0) //pokud je výška záporná jedná se o chybu - typicky pøi pøistání
                newPos.y = 0;
            this.transform.localPosition = newPos; // predikce dle pøedchozí velocity
           
        }
        else
        {
            lastUpdate += Time.deltaTime;
            droneManager.ControlledDrone.usedForCalculation = true; //data již byly užity pro update
            calcNewPosSingleFrame();
        }

        height = this.transform.localPosition.y; // predikce výšky 

        if (!debugMode)
        {
            return;
        }
        if (testGpsGm)
            testGpsGm.transform.localPosition = posGpsforCal;
        if (testImuGm)
            testImuGm.transform.localPosition = imuPosition;
    }

    // více snímkový výpoèet nepoužitelný
    IEnumerator calcNewPosMultiFrame()
    {
        calcGps();
        yield return null;
        ImuCalcPosition();
        lastUpdate = 0;
        yield return null;
        MixPositions();
    }

    void calcNewPosSingleFrame()
    {
        calcGps();
        ImuCalcPosition();
        MixPositions();
        lastUpdate = 0;
    }

    void MixPositions()
    {
        posGpsforCal = gpsPosition;
        Vector3 newPos;
        if (gpsWeight == 0)
        {
            newPos = imuPosition; // zmixování obou pozic
            if (debugMode || droneManager.ControlledDrone.FlightData.InvalidGps) // pokud dron nemá validní gps pozici vychází podle imu
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
            newPos = posGpsforCal * gpsWeight + imuPosition * imuWeight; // zmixování obou pozic


        }
        if (newPos.y < 0) //pokud je výška záporná jedná se o chybu - typicky pøi pøistání
            newPos.y = 0;
        this.transform.localPosition = newPos;
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
