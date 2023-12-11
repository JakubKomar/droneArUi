/*
 * MissionObject - class to create and display mission object as waypoints or zones
 * 
 * Author : Martin Kyjac (xkyjac00)
 */

using Mapbox.Utils;
using UnityEngine;

namespace Mission
{
    public abstract class MissionObject
    {
        public GameObject WaypointGameObject { get; set; }
        public Vector2d Location { get; set; }
        public WaypointState State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
                var changeState = MissionManager.Instance.ChangeReachedWaypointsState;
                if (changeState)
                {
                    ChangeColor(value);
                }
                ToggleText(value == WaypointState.Active || !changeState);
            }
        }
        private WaypointState _state { get; set; }

        public abstract bool IsInside(Transform transform);

        public MissionObject(Transform parent, Vector2d location, float altitude, GameObject prefab)
        {
            var map = GPSManager.Instance.Map;
            var position = map.GeoToWorldPosition(location, false);

            Location = location;

            WaypointGameObject = Object.Instantiate(prefab);
            WaypointGameObject.transform.parent = parent;

            var textForHololens = WaypointGameObject.transform.Find("TextForHololens");
            textForHololens.GetComponent<WaypointManager>().cameraToFace = Camera.main.transform;

            position.y = altitude - UserProfileManager.Instance.Height;
            WaypointGameObject.transform.position = position;

            var renderer = WaypointGameObject.GetComponent<Renderer>();
            renderer.material.color = MissionManager.Instance.NonActiveWaypoint;

            // Waypoint state must be set after game object is created because of custom setter
            State = WaypointState.NotActive;
        }

        // Change waypoint color according to state
        private void ChangeColor(WaypointState state)
        {
            var renderer = WaypointGameObject?.GetComponent<Renderer>();
            if (renderer == null) return;

            var missionManager = MissionManager.Instance;
            Color color;
            switch (state)
            {
                case WaypointState.Active:
                    color = missionManager.ActiveWaypoint;
                    break;
                case WaypointState.Reached:
                    //color = missionManager.ReachedWaypoint;
                    //break;
                    WaypointGameObject.SetActive(false);
                    return;
                default:
                    color = missionManager.NonActiveWaypoint;
                    break;
            }

            renderer.material.color = color;
        }

        private void ToggleText(bool value)
        {
            var textForHololens = WaypointGameObject.transform.Find("TextForHololens");
            textForHololens.gameObject.SetActive(value);
        }
    }
    public enum WaypointState
    {
        Active,
        Reached,
        NotActive
    }
}
