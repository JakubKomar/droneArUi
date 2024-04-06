/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// skryje drona pokud je odpojený
/// </summary>

using UnityEngine;

public class DroneHidder : MonoBehaviour
{

    [SerializeField]
    GameObject objToHide=null;  

    
    void Update()
    {
        DroneManager droneManager=DroneManager.Instance;
        objToHide.SetActive(droneManager != null&& droneManager.ControlledDrone!=null);
    }
}
