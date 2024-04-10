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
    public bool disableHud = false;

    [SerializeField]
    bool forceActiveHud = false;


    [SerializeField]
    TextMeshProUGUI distance;
    [SerializeField]
    TextMeshProUGUI alt;

    private DroneManager droneManager;

    private Transform player;
    [SerializeField]
    private GameObject hudChild;

    private ToggleWordscaleDrone toggleWordscaleDrone;
    [SerializeField]
    private GameObject vlc;

    void Start()
    {
        droneManager = DroneManager.Instance;
        player = Camera.main.transform;
        toggleWordscaleDrone = ToggleWordscaleDrone.Instance;
    }

    void Update()
    {
        Drone drone = droneManager.ControlledDrone;

        disableHud = !toggleWordscaleDrone.droneWordscaleHud;


        vlc.SetActive(toggleWordscaleDrone.droneWordscaleCamera);

        if (!disableHud && drone != null || forceActiveHud)
        {
            droneDistance = Vector3.Distance(this.transform.position, player.position);


            float scale = droneDistance;
            scale=Mathf.Round(scale*100)* 0.00001f;
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

    }
}
