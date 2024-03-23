using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarierPrefabControler : MapGameObjectData
{
    private Collider targetCollider; // Collider, ve kter�m budeme kontrolovat

    public string objectTagToDetect = "drone"; // Tag objekt�, kter� chceme detekovat

    public float checkInterval = 0.2f; // Interval kontrol pro detekci

    private float timeSinceLastCheck = 0.05f;
    
    void Start()
    {
        targetCollider = this.gameObject.GetComponent<Collider>();
    }

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f;
            CheckForObject();
        }
    }

    private bool reported=false;

    private void CheckForObject()
    {
        // Kontrola, zda se objekt dan�ho typu nach�z� v colideru
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