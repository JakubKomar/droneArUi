/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// zajistuje kalibraci svìta kolem hráèe
/// </summary>

using Mapbox.Unity.Map;
using UnityEngine;
using UnityEngine.Events;

public class calibrationScript : Singleton<calibrationScript>
{
    public AbstractMap wordScaleMap = null;
    public AbstractMap miniMap = null;

    public Transform playerCamera = null;
    public DroneManager droneManager = null;
    public Mapbox.Utils.Vector2d playerPosition;

    public MapControler mapControler = null;

    public Mapbox.Utils.Vector2d playerGps;
    public float playerHading;

    public UnityEvent calibrationEvent = new UnityEvent();

    [SerializeField]
    GameObject manipulatorPrefab = null;
    [SerializeField]
    CalibrationGround ground;

    void Update()
    {
        playerGps = wordScaleMap.WorldToGeoPosition(playerCamera.position);

        float playerRottation = playerCamera.rotation.eulerAngles.y;
        playerHading = playerRottation - wordScaleMap.transform.rotation.eulerAngles.y;
    }

    public void onCalibration()
    {
        double latitude;
        double longitude;
        double compas;

        if (droneManager.ControlledDrone == null)
        {
            Debug.LogWarning("Nelze provést calibraci bez pøipojeného  drona");
            latitude = 49.22732;
            longitude = 16.59683;
            compas = 250;
        }
        else
        {
            latitude = droneManager.ControlledDrone.FlightData.Latitude;
            longitude = droneManager.ControlledDrone.FlightData.Longitude;

            //test purpeses
            //latitude = 49.226978092949324;
            //longitude = 16.59519006457966;


            compas = droneManager.ControlledDrone.FlightData.Compass;
        }
        //Debug.Log(playeryRottation);
        onCalibration( latitude,  longitude, compas);

    }
    //test budova
    public void testCalibration1()
    {
        onCalibration(49.22743894929612, 16.597058259073513, 248);
    }

    //test filmari
    public void testCalibration3()
    {

        onCalibration(49.22719967189906, 16.59721665902667, 347);
    }

    //test solary
    public void testCalibration2()
    {
        onCalibration(49.22733586222895, 16.597170390922592, 262);
    }


    private void onCalibration(double latitude, double longitude,double compas)
    {
        float playerRottation = playerCamera.rotation.eulerAngles.y;

        Mapbox.Utils.Vector2d actualCenter;

        actualCenter.x = latitude;
        actualCenter.y = longitude;
        playerPosition = actualCenter;

        wordScaleMap.UpdateMap(actualCenter);

        //reset soft calibrace
        this.transform.localScale = Vector3.one;
        this.transform.localRotation = Quaternion.identity;
        this.transform.position = Vector3.zero;

        // nastavení pozice dle hráèe a drona
        Quaternion targetRotation = Quaternion.Euler(0, playerRottation - ((float)compas), 0);
        wordScaleMap.transform.localRotation = targetRotation;
        wordScaleMap.transform.localPosition = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y - 1.8f, playerCamera.transform.position.z);
        wordScaleMap.transform.localScale = Vector3.one;

        manipulatorPrefab.transform.localRotation = Quaternion.identity;
        manipulatorPrefab.transform.localPosition = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y - 0.8f, playerCamera.transform.position.z); ;



        miniMap.UpdateMap(actualCenter);
        mapControler.setCurentCenter();

        calibrationEvent.Invoke();
        TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
        textToSpeechSyntetizer.say("Calibration finished.");
    }

    public void setHomeLocation(string localizationString)
    {
        Mapbox.Utils.Vector2d actualCenter = Mapbox.Unity.Utilities.Conversions.StringToLatLon(localizationString);

        playerPosition = actualCenter;

        wordScaleMap.UpdateMap(actualCenter);
        miniMap.UpdateMap(actualCenter);
        mapControler.setCurentCenter();
    }
    bool softCalibrationActive = false;
    private void softCalibrationStart()
    {

        VectorSubLayerProperties vc = wordScaleMap.VectorData.FindFeatureSubLayerWithName("Buildings");
        vc.SetActive(true);
        //vc.Modeling.LineOptions.SetLineWidth(1);

        manipulatorPrefab.SetActive(true);
        manipulatorPrefab.transform.rotation = Quaternion.Euler(0, playerCamera.transform.rotation.y, 0);
        manipulatorPrefab.transform.position = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y, playerCamera.transform.position.z); ; //new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y, playerCamera.transform.position.z); ;
        ground.setCalibration(true);

        TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
        textToSpeechSyntetizer.say("Soft calibration started.");
    }

    private void softCalibrationFinish()
    {
        VectorSubLayerProperties vc = wordScaleMap.VectorData.FindFeatureSubLayerWithName("Buildings");
        vc.SetActive(false);

        manipulatorPrefab.SetActive(false);
        ground.setCalibration(false);
        TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
        textToSpeechSyntetizer.say("Soft calibration finished.");
    }

    public void onToggleSoftCalibration()
    {
        softCalibrationActive = !softCalibrationActive;
        if (softCalibrationActive)
            softCalibrationStart();
        else
            softCalibrationFinish();
    }
}
