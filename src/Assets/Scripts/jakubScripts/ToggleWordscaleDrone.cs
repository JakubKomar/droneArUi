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

public class ToggleWordscaleDrone : Singleton<ToggleWordscaleDrone>
{
    public bool droneWordscaleHud = true;
    public bool droneWordscaleCamera = true;
    public bool droneWordscaleDebug = false;
    public void onToggleWordScaleDrone()
    {
        droneWordscaleDebug = !droneWordscaleDebug;
    }

    public void onToggleWordScaleDroneHud()
    {
        droneWordscaleHud = !droneWordscaleHud;
    }
    public void onToggleWordScaleHudCamera()
    {
        droneWordscaleCamera = !droneWordscaleCamera;
    }
}
