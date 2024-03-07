using Mapbox.Unity.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class calibrationScript : MonoBehaviour
{
    // Start is called before the first frame update
    public AbstractMap wordScaleMap = null;
    public AbstractMap miniMap = null;

    public Transform playerCamera =null;
    public DroneManager droneManager = null;
    public Mapbox.Utils.Vector2d playerPosition;

    public MapControler mapControler = null;
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onCalibration()
    {
        double latitude;
        double longitude;
        double compas;

        Debug.Log("Calibration");
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
        
        Quaternion targetRotation = Quaternion.Euler(0, playeryRottation-((float) compas), 0);
        wordScaleMap.transform.rotation = targetRotation;
        wordScaleMap.transform.position = new Vector3(playerCamera.transform.position.x, 0, playerCamera.transform.position.z);

        miniMap.UpdateMap(actualCenter);
        mapControler.setCurentCenter();


    }
}
