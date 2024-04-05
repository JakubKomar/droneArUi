using UnityEngine;

public class DroneHidder : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject objToHide=null;  

    

    // Update is called once per frame
    void Update()
    {
        DroneManager droneManager=DroneManager.Instance;
        objToHide.SetActive(droneManager != null&& droneManager.ControlledDrone!=null);
    }
}
