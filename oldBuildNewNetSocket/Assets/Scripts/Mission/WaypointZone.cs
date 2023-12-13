/*
 * WaypointZone - mission object of type zone - cylinder with radius given
 * 
 * Author : Martin Kyjac (xkyjac00)
 */

using Mapbox.Utils;
using UnityEngine;

namespace Mission
{
    public class WaypointZone : MissionObject
    {
        public WaypointZone(Transform parent, WaypointState state, Vector2d location, float diameter)
            : base(parent, location, 2.5f, MissionManager.Instance.WaypointZonePrefab)
        {
            var scale = WaypointGameObject.transform.localScale;
            WaypointGameObject.transform.localScale = new Vector3(diameter * 2, scale.y, diameter * 2);
        }

        public override bool IsInside(Transform transform)
        {
            var vA = new Vector2(transform.position.x, transform.position.z);
            var vB = new Vector2(WaypointGameObject.transform.position.x, WaypointGameObject.transform.position.z);
            return Vector2.Distance(vA, vB) <= WaypointGameObject.transform.localScale.x / 2;
        }
    }
}
