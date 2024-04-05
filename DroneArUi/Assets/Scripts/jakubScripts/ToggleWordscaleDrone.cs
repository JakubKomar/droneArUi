/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Vypíná zapíná vizualizaci wordspace objektu dronu 
/// </summary>

using UnityEngine;

public class ToggleWordscaleDrone : MonoBehaviour
{
    public void onToggleWordScaleDrone()
    {
        DroneWordscaleHud[] droneWordscaleHuds = FindObjectsOfType<DroneWordscaleHud>();

        foreach(DroneWordscaleHud droneWordscaleHud in droneWordscaleHuds) { droneWordscaleHud.disableDrone = !droneWordscaleHud.disableDrone; }
    }
}
