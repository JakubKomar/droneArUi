/*
 * Mirror Drone - manages rotation and value of distance to waypoints 
 * 
 * authors: Marek Václavík (xvacla26), Martin Kyjac (xkyjac00)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaypointManager : MonoBehaviour {

    public Transform cameraToFace; 
    Transform drone;
    TextMeshPro distance;

    void Start()
    {
        drone = DroneManager.Instance.ControlledDroneGameObject.transform;
        distance = transform.gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update() {
        transform.LookAt(transform.position + cameraToFace.transform.rotation * Vector3.forward, cameraToFace.transform.rotation * Vector3.up);
        float dist = Vector3.Distance(drone.position, transform.parent.position);
        distance.text = Mathf.Round(dist * 100.0f) * 0.01f + "m";

        var scale = (Vector3.Distance(cameraToFace.position, distance.transform.position) * 2) / 100;
        distance.transform.localScale = new Vector3(scale, scale, scale);
    }
}
