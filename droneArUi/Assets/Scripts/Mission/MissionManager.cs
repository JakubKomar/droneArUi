/*
 * Mission manager - creating waypoints and zones according to GPS coordinates given
 * 
 * Author : Martin Kyjac (xkyjac00)
 */

using Mapbox.Utils;
using Mission;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionManager : Singleton<MissionManager>
{
    public GameObject WaypointPrefab;
    public GameObject WaypointZonePrefab;
    public GameObject ZonePrefab;
    public Transform MissionGameObject;
    public bool ChangeReachedWaypointsState;
    public Color ActiveWaypoint;
    public Color ReachedWaypoint;
    public Color NonActiveWaypoint;

    private IEnumerable<MissionObject> Waypoints = new List<MissionObject>();
    private IEnumerable<Zone> Zones = new List<Zone>();
    public MissionObject CurrentTarget;

    /// <summary>
    /// Check if drone reached active waypoint. If yes, set the next active waypoint
    /// </summary>
    public void Update()
    {
        var drone = DroneManager.Instance.ControlledDroneGameObject;

        if (CurrentTarget == null || drone == null) return;

        var waypointPosition = CurrentTarget.WaypointGameObject.transform.position;
        if (CurrentTarget.IsInside(drone.transform))
        {
            CurrentTarget.State = WaypointState.Reached;
            CurrentTarget = Waypoints.FirstOrDefault(x => x.State == WaypointState.NotActive);
            if (CurrentTarget != null)
            {
                CurrentTarget.State = WaypointState.Active;
            }
        }
    }

    /// <summary>
    /// Create waypoints and zones in scene and add them to
    /// Waypoints and Zones properties
    /// </summary>
    public void GenerateWaypoints()
    {
        // If mission already exists, need to remove all waypoints from scene
        RemoveMissionElements();

        var waypointObjects = new List<MissionObject>();

        var waypoints = new List<Vector2d>
        {
            new Vector2d(49.22730055356414, 16.597094392527428),
            new Vector2d(49.22742667169548, 16.5971091446734),
            new Vector2d(49.227448129262044, 16.59725398394325),
        };

        var zone = new Vector2d[2] { new Vector2d(49.22716285112991, 16.597185306750877), new Vector2d(49.22724780597617, 16.597090758882445) };

        foreach (var waypoint in waypoints)
        {
            var altitude = waypoints.IndexOf(waypoint) == 0 ? 6f : 2f;
            waypointObjects.Add(CreateWaypoint(waypoint, altitude));
        }

        waypointObjects.Add(CreateWaypointZone(new Vector2d(49.22724789838533, 16.597418592577164), 1.5f));
        Waypoints = waypointObjects;

        Zones = Zones.Append(CreateZone(zone));
    }

    /// <summary>
    /// Create new Waypoint and render it in the scene. Newly created waypoint has
    /// NonActive state and red color and is created under the MissionGameObject.
    /// </summary>
    /// <param name="location">Latitude and longitude of waypoint</param>
    /// <param name="altitude">Relative altitude above the ground</param>
    public Waypoint CreateWaypoint(Vector2d location, float altitude)
    {
        return new Waypoint(MissionGameObject, WaypointState.NotActive, location, altitude);
    }

    public WaypointZone CreateWaypointZone(Vector2d location, float diameter)
    {
        return new WaypointZone(MissionGameObject, WaypointState.NotActive, location, diameter);
    }

    public Zone CreateZone(Vector2d[] points)
    {
        return new Zone(MissionGameObject, ZoneType.NoFlyZone, points, Color.red);
    }

    public void StartMission(WaypointsOrder order=WaypointsOrder.OrderInList)
    {
        if (!Waypoints.Any())
        {
            return;
        }

        if  (order == WaypointsOrder.FromClosest)
        {
            Waypoints = Waypoints.OrderBy(w => Vector3.Distance(w.WaypointGameObject.transform.position, Camera.main.transform.position));
        }

        CurrentTarget = Waypoints.First();
        CurrentTarget.State = WaypointState.Active;
    }

    public void ResetMission()
    {
        if (!Waypoints.Any())
        {
            return;
        }

        for (int i = 0; i < Waypoints.Count(); i++)
        {
            var newState = i == 0 ? WaypointState.Active : WaypointState.NotActive;
            var waypoint = Waypoints.ElementAt(i);
            waypoint.State = newState;
            waypoint.WaypointGameObject.SetActive(true);
        }

        CurrentTarget = Waypoints.First();
    }

    /// <summary>
    /// If ChangeReachedWaypointsState is set to true, waypoints
    /// will change color and will display text according to their states.
    /// </summary>
    public void ToggleMissionStateChange()
    {
        ChangeReachedWaypointsState = !ChangeReachedWaypointsState;
    }

    /// <summary>
    /// Delete all mission elemets - waypoints and zones
    /// from scene and from MissionManager properties Waypoints and Zones
    /// </summary>
    private void RemoveMissionElements()
    {
        if (Waypoints.Any())
        {
            Waypoints = new List<Waypoint>();
        }
        if (Zones.Any())
        {
            Zones = new List<Zone>();
        }

        for (int i = 0; i < MissionGameObject.transform.childCount; i++)
        {
            MissionGameObject.transform.GetChild(i).Destroy();
        }
    }

}

public enum WaypointsOrder
{
    FromClosest,
    OrderInList
}

