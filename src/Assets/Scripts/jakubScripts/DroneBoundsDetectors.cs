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
    private bool ignoreCollisions = true;
    private Collider targetCollider; // Collider, ve kterém budeme kontrolovat
    private void Start()
    {
        mapData = MapData.Instance;
        
        targetCollider = this.gameObject.GetComponent<Collider>();
        StartCoroutine(PerformActionAfterDelay());

    }

   /* void Update()
    {
        CheckForObject();
    }
    
    
    bool droneFoundWarnRp = false;
    bool droneFoundBarRp = false;
    
    private void CheckForObject()
    {
        if (ignoreCollisions)
            return;
        // Kontrola, zda se objekt daného typu nachází v colideru
        Collider[] colliders = Physics.OverlapBox(targetCollider.bounds.center, targetCollider.bounds.extents, Quaternion.identity);

        bool droneFoundWarn = false;
        bool droneFoundBar = false;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("barrierWS"))
            {
                mapData.droneEnterBarier(false);
                droneFoundBarRp=true;
            }
            else if (col.CompareTag("warningWS"))
            {
                mapData.droneEnterBarier(true);
                droneFoundWarnRp=true;
            }
            else if (col.CompareTag("waypointWS"))
            {
                MapGameObjectData mapGameObjectData = col.gameObject.GetComponent<MapGameObjectData>();
                if (mapGameObjectData && mapGameObjectData.mapObjectData != null && mapGameObjectData.mapObjectData.mapObject != null)
                {
                    Waypoint waypoint = (Waypoint)mapGameObjectData.mapObjectData.mapObject;
                    if (waypoint != null)
                        waypoint.onDroneEnterColider();
                }
            }
        }

        if (!droneFoundWarn)
        {
            mapData.droneLeaveBarier(true);
        }
        if (!droneFoundBar)
        {
            mapData.droneLeaveBarier(false);
        }
    }*/
    private void OnTriggerEnter(Collider other)
    {
        if (ignoreCollisions)
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
        if (ignoreCollisions)
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

    private void OnEnable()
    {
        StartCoroutine(PerformActionAfterDelay());
    }
    public void setIngoreColizions(bool val)
    {
        if (val == false)
        {
            StartCoroutine(PerformActionAfterDelay());
        }
        else
        {
            ignoreCollisions = val;
        }
    }

    // vyèkej na romístìní všech objektù a pak zaèni detekovat kolize
    IEnumerator PerformActionAfterDelay()
    {
        yield return new WaitForSeconds(0.4f);
        this.ignoreCollisions = false;
    }
}
