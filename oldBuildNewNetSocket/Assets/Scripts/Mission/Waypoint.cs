/*
 * Author: Martin Kyjac (xkyjac00)
 * 
 *  Mission waypoint object rendered as sphere with 1m diameter and color according
 *  to its state and setting in MissionManager. 
 */

using Mapbox.Utils;
using UnityEngine;

namespace Mission
{
    public class Waypoint : MissionObject
    {
        public Waypoint(Transform parent, WaypointState state, Vector2d location, float altitude)
            : base(parent, location, altitude, MissionManager.Instance.WaypointPrefab)
        {
        }

        public override bool IsInside(Transform transform)
        {
            var dronePosition = transform.position;
            var waypointPosition = WaypointGameObject.transform.position;
            return Vector3.Distance(dronePosition, waypointPosition) <= WaypointGameObject.transform.lossyScale.x / 2;
        }
    }
}

