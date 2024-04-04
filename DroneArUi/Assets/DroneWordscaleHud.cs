using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroneWordscaleHud :MonoBehaviour
{
    // Start is called before the first frame update

    private float droneDistance = 0;

    [SerializeField]
    private float minDistanceLimit = 15;

    [SerializeField]
    public bool disableDrone=true;

    Drone drone;

    [SerializeField]
    TextMeshProUGUI distance;
    [SerializeField]
    TextMeshProUGUI alt;

    private DroneManager droneManager;

    private Transform player;
    [SerializeField]
    private GameObject hudChild;

    [SerializeField]
    GameObject droneObject;
    void Start()
    {
        droneManager = DroneManager.Instance;
        player= Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Drone drone = droneManager.ControlledDrone;


        if (drone != null)
        {
            droneDistance = Vector3.Distance(this.transform.position, player.position);

            // objekt je moc blízko, vypneme sledování
            /*if (droneDistance < minDistanceLimit)
            {
                hudChild.gameObject.SetActive(false);
                return;
            }*/
            float scale = droneDistance * 0.001f;
            hudChild.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        


            Vector3 direction = (player.position - this.transform.position).normalized;

            // Získej rotaci, která smìøuje k hráèi
            Quaternion rotation = Quaternion.LookRotation(direction);
            hudChild.transform.rotation = rotation;


            distance.text = "D:" + Mathf.Round(droneDistance).ToString();
            //alt.text = string.Format("A:{0:0.0}m", drone.FlightData.Altitude);
            hudChild.gameObject.SetActive(true);
        }
        else
        {
            hudChild.gameObject.SetActive(false);
        }

        droneObject.SetActive(!disableDrone);
    }
}
