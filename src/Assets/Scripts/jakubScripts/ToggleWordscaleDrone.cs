/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Vyp�n� zap�n� vizualizaci wordspace objektu dronu 
/// </summary>

using UnityEngine;

public class ToggleWordscaleDrone : MonoBehaviour
{
    public void onToggleWordScaleDrone()
    {
        DroneWordscaleHud[] droneWordscaleHuds = FindObjectsOfType<DroneWordscaleHud>();

        foreach(DroneWordscaleHud droneWordscaleHud in droneWordscaleHuds) { droneWordscaleHud.disableDrone = !droneWordscaleHud.disableDrone; }
    }

    public void onToggleWordScaleDroneHud()
    {
        DroneWordscaleHud[] droneWordscaleHuds = FindObjectsOfType<DroneWordscaleHud>();

        foreach (DroneWordscaleHud droneWordscaleHud in droneWordscaleHuds) { droneWordscaleHud.disableHud = !droneWordscaleHud.disableHud; }
    }
}
