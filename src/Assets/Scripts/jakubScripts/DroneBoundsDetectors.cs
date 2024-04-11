/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// optimalizovaná metoda pro detekci kolizí s bariérami a waypointy
/// </summary>

using UnityEngine;

public class DroneBoundsDetectors : MonoBehaviour
{
    MapData mapData;
    private void Start()
    {
        mapData= MapData.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("barrierWS"))
        {
            mapData.droneEnterBarier(true);
        }else if (other.CompareTag("warningWS"))
        {
            mapData.droneEnterBarier(false);
        }
        else if (other.CompareTag("waypointWS"))
        {
            MapGameObjectData mapGameObjectData = other.gameObject.GetComponent<MapGameObjectData>();
            if (mapGameObjectData&& mapGameObjectData.mapObjectData!=null&& mapGameObjectData.mapObjectData.mapObject!=null)
            {
                Waypoint waypoint = (Waypoint)mapGameObjectData.mapObjectData.mapObject;
                if(waypoint!=null)
                    waypoint.onDroneEnterColider();
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("barrierWS"))
        {
            mapData.droneLeaveBarier(true);
        }
        else if (other.CompareTag("warningWS"))
        {
            mapData.droneLeaveBarier(false);
        }
    }
}
