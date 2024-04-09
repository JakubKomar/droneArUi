/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// logika hudu, který je umístìn na dronu ve svìtì
/// </summary>
using TMPro;
using UnityEngine;

public class DroneWordscaleHud : MonoBehaviour
{
    private float droneDistance = 0;

    [SerializeField]
    private float minDistanceLimit = 0;

    [SerializeField]
    public bool disableDrone = true;

    [SerializeField]
    public bool disableHud = false;

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
        player = Camera.main.transform;
    }

    void Update()
    {
        Drone drone = droneManager.ControlledDrone;


        if (!disableHud && drone != null )
        {
            droneDistance = Vector3.Distance(this.transform.position, player.position);


            float scale = droneDistance * 0.001f;
            hudChild.gameObject.transform.localScale = new Vector3(scale, scale, scale);



           // Získej rotaci, která smìøuje k hráèi
            hudChild.gameObject.transform.LookAt(player.position);


            distance.text = "D:" + Mathf.Round(droneDistance).ToString()+"m";
            //alt.text = string.Format("A:{0:0.0}m", drone.FlightData.Altitude);
            hudChild.gameObject.SetActive(true);
        }
        else
        {
            hudChild.gameObject.SetActive(false);
        }

        droneObject.SetActive(!disableDrone && drone != null);
    }
}
