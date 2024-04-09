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
        DronePositionCalculator[] droneWordscaleHuds = FindObjectsOfType<DronePositionCalculator>();

        foreach(DronePositionCalculator droneWordscaleHud in droneWordscaleHuds) { droneWordscaleHud.debugMode = !droneWordscaleHud.debugMode; }
    }

    public void onToggleWordScaleDroneHud()
    {
        DroneWordscaleHud[] droneWordscaleHuds = FindObjectsOfType<DroneWordscaleHud>();

        foreach (DroneWordscaleHud droneWordscaleHud in droneWordscaleHuds) { droneWordscaleHud.disableHud = !droneWordscaleHud.disableHud; }
    }
}
