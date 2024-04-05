// jakub komárek

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;

public class DynamicHudRotationSetter : Singleton<DynamicHudRotationSetter>
{
    // Start is called before the first frame update

    public GameObject droneWordScale;
    [SerializeField]
    private GameObject hudChild;

    private float droneDistance=0;

    [SerializeField]
    private float minDistanceLimit = 15;

    Drone drone;
    [SerializeField]
    TextMeshProUGUI distance;
    [SerializeField]
    TextMeshProUGUI alt;

    private DroneManager droneManager;

    void Start()
    {
        droneManager = DroneManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

        Drone drone = droneManager.ControlledDrone;


        if (drone != null && droneWordScale != null)
        {
            droneDistance = Vector3.Distance(this.transform.position, droneWordScale.transform.position);

            // objekt je moc blízko, vypneme sledování
            if (droneDistance < minDistanceLimit)
            {
                hudChild.gameObject.SetActive(false);
                return;
            }

            Vector3 targetDirection = droneWordScale.transform.position - transform.position;

            // Vypoèet rotace na základì smìru
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Nastavení rotace game objektu s canvasem
            transform.rotation = targetRotation;

            distance.text = "D:"+ math.round(droneDistance).ToString();
            alt.text = string.Format("A:{0:0.0}m", drone.FlightData.Altitude); 
            hudChild.gameObject.SetActive(true);
        }
        else
        {
            hudChild.gameObject.SetActive(false);
        }

    }
}
