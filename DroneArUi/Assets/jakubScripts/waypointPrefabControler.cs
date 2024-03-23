using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointPrefabControler : MapGameObjectData
{
    void Start()
    {
        targetCollider=this.gameObject.GetComponent<Collider>();
    }

    private Collider targetCollider; // Collider, ve kterém budeme kontrolovat

    public string objectTagToDetect = "drone"; // Tag objektù, které chceme detekovat

    public float checkInterval = 0.2f; // Interval kontrol pro detekci

    private float timeSinceLastCheck = 0.05f;

    public Material materialCrossed;
    public Material materialNotCrossed;
    public Material materialAsTarget;

    public GameObject spere;


    void Update()
    {
        // update každých 0.2s
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f;
            CheckForObject();

            // nastavení materiálu dle interní reprezetace waypointu
            MeshRenderer renderer = spere.GetComponent<MeshRenderer>();

            if (this.mapObjectData != null && this.mapObjectData.mapObject != null
                    && this.mapObjectData.mapObject is Waypoint&& spere!=null && renderer != null)
            {
                Waypoint waypoint = (Waypoint)this.mapObjectData.mapObject;

                if (waypoint.hasBeenVisited)
                {
                    renderer.material = materialCrossed;
                }
                else if (waypoint.setAsTarget)
                {
                    renderer.material = materialAsTarget;
                }
                else
                {
                    renderer.material = materialNotCrossed;
                }
                
            }
        }
    }
    private void CheckForObject()
    {
        // Kontrola, zda se objekt daného typu nachází v colideru
        Collider[] colliders = Physics.OverlapBox(targetCollider.bounds.center, targetCollider.bounds.extents, Quaternion.identity);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag(objectTagToDetect))
            {
                // Objekt daného typu nalezen v colideru      
                if (this.mapObjectData != null && this.mapObjectData.mapObject != null
                    && this.mapObjectData.mapObject is Waypoint) 
                { 
                    Waypoint waypoint =(Waypoint)this.mapObjectData.mapObject;
                    waypoint.onDroneEnterColider();
                }
            }
        }
    }
}
