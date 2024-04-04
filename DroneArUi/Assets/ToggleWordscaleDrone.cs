using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWordscaleDrone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onToggleWordScaleDrone()
    {
        DroneWordscaleHud[] droneWordscaleHuds = FindObjectsOfType<DroneWordscaleHud>();

        foreach(DroneWordscaleHud droneWordscaleHud in droneWordscaleHuds) { droneWordscaleHud.disableDrone = !droneWordscaleHud.disableDrone; }
    }
}
