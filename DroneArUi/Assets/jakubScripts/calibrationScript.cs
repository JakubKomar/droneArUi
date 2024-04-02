// autor: jakub komárek

using Mapbox.Unity.Map;
using UnityEngine;
using UnityEngine.Events;

public class calibrationScript : Singleton<calibrationScript>
{
    // Start is called before the first frame update
    public AbstractMap wordScaleMap = null;
    public AbstractMap miniMap = null;

    public Transform playerCamera =null;
    public DroneManager droneManager = null;
    public Mapbox.Utils.Vector2d playerPosition;

    public MapControler mapControler = null;

    public Mapbox.Utils.Vector2d playerGps;
    public float playerHading;

    public UnityEvent calibrationEvent = new UnityEvent();
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        playerGps= wordScaleMap.WorldToGeoPosition(playerCamera.position);

        float playerRottation = playerCamera.rotation.eulerAngles.y;
        playerHading= playerRottation- wordScaleMap.transform.rotation.eulerAngles.y;
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
            compas =250;
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
        float playeryRottation = playerCamera.rotation.eulerAngles.y;
        //Debug.Log(playeryRottation);

        Mapbox.Utils.Vector2d actualCenter;
       
        actualCenter.x= latitude;
        actualCenter.y= longitude;
        playerPosition = actualCenter;

        wordScaleMap.UpdateMap(actualCenter);
        
        //reset soft calibrace
        this.transform.localScale = Vector3.one;
        this.transform.localRotation = Quaternion.identity;
        this.transform.position = Vector3.zero;

        // nastavení pozice dle hráèe a drona
        Quaternion targetRotation = Quaternion.Euler(0, playeryRottation - ((float)compas), 0);
        wordScaleMap.transform.localRotation = targetRotation;
        wordScaleMap.transform.localPosition = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y - 1.8f, playerCamera.transform.position.z);
        wordScaleMap.transform.localScale = Vector3.one;

        miniMap.UpdateMap(actualCenter);
        mapControler.setCurentCenter();

        calibrationEvent.Invoke();
        TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
        textToSpeechSyntetizer.say("Calibration finished.");

    }

    public void setHomeLocation(string localizationString)
    {
        Mapbox.Utils.Vector2d actualCenter= Mapbox.Unity.Utilities.Conversions.StringToLatLon(localizationString);

        playerPosition = actualCenter;

        wordScaleMap.UpdateMap(actualCenter);
        miniMap.UpdateMap(actualCenter);
        mapControler.setCurentCenter();
    }
}
