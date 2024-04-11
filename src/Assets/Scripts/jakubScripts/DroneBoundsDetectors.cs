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
using System.Collections;
public class DroneBoundsDetectors : MonoBehaviour
{
    MapData mapData;
    private bool ingoreColizions = true;
    private void Start()
    {
        mapData = MapData.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (ingoreColizions)
            return;
        if (other.CompareTag("barrierWS"))
        {
            mapData.droneEnterBarier(false);
        }else if (other.CompareTag("warningWS"))
        {
            mapData.droneEnterBarier(true);
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
        if (ingoreColizions)
            return;
        if (other.CompareTag("barrierWS"))
        {
            mapData.droneLeaveBarier(false);
        }
        else if (other.CompareTag("warningWS"))
        {
            mapData.droneLeaveBarier(true);
        }
    }

    
    public void setIngoreColizions(bool val)
    {
        if (val == false)
        {
            StartCoroutine(PerformActionAfterDelay());
        }
        else
        {
            ingoreColizions = val;
        }
    }

    // vyèkej na romístìní všech objektù a pak zaèni detekovat kolize
    IEnumerator PerformActionAfterDelay()
    {
        yield return new WaitForSeconds(0.4f);
        this.ingoreColizions = false;
    }
}
