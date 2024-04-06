/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// nastavuje pozici a rotaci dynamického hudu- již se nepoužívá
/// </summary>

using TMPro;
using UnityEngine;
using Unity.Mathematics;

public class DynamicHudRotationSetter : Singleton<DynamicHudRotationSetter>
{
    public GameObject droneWordScale;
    [SerializeField]
    private GameObject hudChild;

    private float droneDistance = 0;

    [SerializeField]
    private float minDistanceLimit = 15;

    [SerializeField]
    TextMeshProUGUI distance;
    [SerializeField]
    TextMeshProUGUI alt;

    private DroneManager droneManager;

    void Start()
    {
        droneManager = DroneManager.Instance;
    }

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

            distance.text = "D:" + math.round(droneDistance).ToString();
            alt.text = string.Format("A:{0:0.0}m", drone.FlightData.Altitude);
            hudChild.gameObject.SetActive(true);
        }
        else
        {
            hudChild.gameObject.SetActive(false);
        }
    }
}
