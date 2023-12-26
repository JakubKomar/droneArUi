/*
 * Scene Manager - manages the scene
 * 
 * author: Marek Václavík
 * login: xvacla26
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DroneSimulator
{
    public class SceneManager : MonoBehaviour
    {

        public HeadUpDisplay HeadUpDisplay;
        public GameObject cameraFrame;
        public GameObject canvas;
        public GameObject missions;

        public Quaternion finalInputs { get; private set; }
        public float elv;
        public float yaw;
        public float pitch;
        public float roll;


        private void Start()
        {
            Debug.Log("Begin Sim- START");
            HeadUpDisplay.CustomStart();
            //TryDisplayUI(true);
        }

        private void Update()
        {
            
            //RunFrame();
        }

        private void TryDisplayUI(bool isStartup=false)
        {
            if (!canvas.activeSelf || isStartup)
            {
                canvas.SetActive(DroneManager.Instance.ControlledDrone != null);
            }
        }

        public void Reset()
        {
            Debug.Log("Reset");

        }

        public void ToggleMesh()
        {
            Debug.Log("ToggleMesh");
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Spatial Awareness");

        }

        public void ChangeCamera()
        {
            Debug.Log("ChangeCamera");
            if (cameraFrame.transform.IsChildOf(HeadUpDisplay.transform))
            {
                cameraFrame.transform.SetParent(canvas.transform);
                cameraFrame.transform.localPosition = new Vector3(700,-150,0);
            }
            else
            {
                cameraFrame.transform.SetParent(HeadUpDisplay.transform);
                cameraFrame.transform.localPosition = new Vector3(500, 0, 0);
            }
                

        }

        public void ToggleMissions()
        {
            Debug.Log("ToggleMissions");
            if (missions.activeSelf)
            {
                missions.SetActive(false);
            }
            else
                missions.SetActive(true);
        }

        public void ToggleScale()
        {
            Debug.Log("ToggleScale");
            if (HeadUpDisplay.transform.localScale == new Vector3(1,1,1))
            {
                HeadUpDisplay.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            }
            else
            {
                HeadUpDisplay.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        public void RunFrame()
        {

            yaw = finalInputs.x;
            elv = finalInputs.y;
            roll = finalInputs.z;
            pitch = finalInputs.w;

           
        }

       
        Quaternion CalulateFinalInputs(float yaw, float elv, float roll, float pitch)
        {
            return new Quaternion(yaw, elv, roll, pitch);
        }

    }
}