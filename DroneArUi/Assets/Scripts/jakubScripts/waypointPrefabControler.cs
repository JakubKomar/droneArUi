/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Star� se o spr�vu waypoint prefabu, nastavuje materi�ly dle stavu waypointu, kontroluje kontakt s drone a reporutuje ho do hlavn� struktury shared dat.
/// </summary>
using UnityEngine;

public class waypointPrefabControler : MapGameObjectData
{
    void Start()
    {
        targetCollider = this.gameObject.GetComponent<Collider>();
    }

    private Collider targetCollider; // Collider, ve kter�m budeme kontrolovat

    public string objectTagToDetect = "drone"; // Tag objekt�, kter� chceme detekovat

    public float checkInterval = 0.2f; // Interval kontrol pro detekci

    private float timeSinceLastCheck = 0.05f;

    public Material materialCrossed;
    public Material materialNotCrossed;
    public Material materialAsTarget;

    public GameObject spere;


    void Update()
    {
        bool checkForBounds = this.mapObjectData != null && this.mapObjectData.mapObject != null && (!this.mapObjectData.isInMinimap);
        if (checkForBounds && timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f;
            CheckForObject();

            // nastaven� materi�lu dle intern� reprezetace waypointu

        }
        else if (checkForBounds)
        {
            // update ka�d�ch 0.2s
            timeSinceLastCheck += Time.deltaTime;
        }

        MeshRenderer renderer = spere.GetComponent<MeshRenderer>();

        if (this.mapObjectData != null && this.mapObjectData.mapObject != null
                && this.mapObjectData.mapObject is Waypoint && spere != null && renderer != null)
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
    private void CheckForObject()
    {
        // Kontrola, zda se objekt dan�ho typu nach�z� v colideru
        Collider[] colliders = Physics.OverlapBox(targetCollider.bounds.center, targetCollider.bounds.extents, Quaternion.identity);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag(objectTagToDetect))
            {
                // Objekt dan�ho typu nalezen v colideru      
                if (this.mapObjectData != null && this.mapObjectData.mapObject != null
                    && this.mapObjectData.mapObject is Waypoint)
                {
                    Waypoint waypoint = (Waypoint)this.mapObjectData.mapObject;
                    waypoint.onDroneEnterColider();
                }
            }
        }
    }
}
