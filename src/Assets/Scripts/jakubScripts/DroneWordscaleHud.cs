/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// logika hudu, kter� je um�st�n na dronu ve sv�t�
/// </summary>
using TMPro;
using System;
using UnityEngine;

public class DroneWordscaleHud : MonoBehaviour
{
    private float droneDistance = 0;

    [SerializeField]
    private float minDistanceLimit = 0;


    [SerializeField]
    public bool disableHud = false;

    [SerializeField]
    bool forceActiveHud = false;


    [SerializeField]
    TextMeshProUGUI distance;

    private DroneManager droneManager;

    [SerializeField]
    private GameObject hudChild;

    private ToggleWordscaleDrone toggleWordscaleDrone;
    [SerializeField]
    private GameObject vlc;

    [SerializeField]
    GameObject gpsLostIcon = null;
    [SerializeField]
    TMP_Text speed = null;

    void Start()
    {
        droneManager = DroneManager.Instance;
        toggleWordscaleDrone = ToggleWordscaleDrone.Instance;

    }

    void Update()
    {
        Drone drone = droneManager.ControlledDrone;

        disableHud = !toggleWordscaleDrone.droneWordscaleHud;


        vlc.SetActive(toggleWordscaleDrone.droneWordscaleCamera);

        if (!disableHud && drone != null)
        {
            droneDistance = Vector3.Distance(this.transform.position, Camera.main.transform.position);


            float scale = droneDistance*0.001f; ;
            /*scale=Mathf.Round(scale*100)* 0.00001f;*/
            hudChild.gameObject.transform.localScale = new Vector3(scale, scale, scale);



           // Z�skej rotaci, kter� sm��uje k hr��i
            hudChild.gameObject.transform.LookAt(Camera.main.transform);


            distance.text = "D:" + Mathf.Round(droneDistance).ToString()+"m";
            if (gpsLostIcon != null)
                gpsLostIcon.SetActive(drone.FlightData.InvalidGps);


            if (speed != null)
            {
                double newSpeed = Math.Abs(drone.FlightData.VelocityX) + Math.Abs(drone.FlightData.VelocityY) + Math.Abs(drone.FlightData.VelocityZ);
                newSpeed = newSpeed * 3.6; // to km/h
                speed.text = "S:" + newSpeed.ToString("F1") + "km/h";
            }


            hudChild.gameObject.SetActive(true);
        }
        else
        {
            hudChild.gameObject.SetActive(false||forceActiveHud);
        }

    }
}
