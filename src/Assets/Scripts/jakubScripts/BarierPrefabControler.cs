/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// logika baréry - detekuje pøítomnost drona a reportujej ji dále
/// </summary>
using UnityEngine;

public class BarierPrefabControler : MapGameObjectData
{
    private Collider targetCollider; // Collider, ve kterém budeme kontrolovat

    public string objectTagToDetect = "drone"; // Tag objektù, které chceme detekovat

    public float checkInterval = 0.2f; // Interval kontrol pro detekci

    private float timeSinceLastCheck = 0.05f;
    
    void Start()
    {
        targetCollider = this.gameObject.GetComponent<Collider>();
    }

    void Update()
    {
        bool checkForBounds = this.mapObjectData != null && this.mapObjectData.mapObject != null && (!this.mapObjectData.isInMinimap);
        if (checkForBounds && timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f;
            CheckForObject();

            // nastavení materiálu dle interní reprezetace waypointu
        }
        else if (checkForBounds)
        {
            // update každých 0.2s
            timeSinceLastCheck += Time.deltaTime;
        }

    }

    private bool reported=false;

    private void CheckForObject()
    {
        // Kontrola, zda se objekt daného typu nachází v colideru
        Collider[] colliders = Physics.OverlapBox(targetCollider.bounds.center, targetCollider.bounds.extents, Quaternion.identity);
        
        bool droneFound=false;
        foreach (Collider col in colliders)
        {
            if (col.CompareTag(objectTagToDetect))
            {
                droneFound=true;
            }

        }

        if(droneFound&& !reported)
        {
            if (this.mapObjectData != null && this.mapObjectData.mapObject != null
                    && this.mapObjectData.mapObject is Barier)
            {
                    Barier barier = (Barier)this.mapObjectData.mapObject;
                    barier.onDroneEnterColider();
                    reported = true;
            }
        }
        if(reported&& !droneFound)
        {
            if (this.mapObjectData != null && this.mapObjectData.mapObject != null
                   && this.mapObjectData.mapObject is Barier)
            {
                Barier barier = (Barier)this.mapObjectData.mapObject;
                barier.onDroneLeaveColider();
                reported = false;
            }
        }
    }
}
