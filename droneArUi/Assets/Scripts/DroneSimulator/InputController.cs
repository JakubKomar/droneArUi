/*
 * Input Controller - Manages Input and cotrols
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
    public class InputController : MonoBehaviour
    {
        public enum InputType { XboxController, Keyboard}
        public InputType inputType = InputType.Keyboard;


        public float rawYaw, rawElv, rawRoll, rawPitch;
        public float speed;

        SceneManager sceneManager;


        public void CustomAwake(SceneManager sceneManager)
        {
            this.sceneManager = sceneManager;
        }

        public void GetFlightCommmands()
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Reset"))
            {
                sceneManager.Reset();
            }
            else if (Input.GetKeyDown(KeyCode.T) || Input.GetButtonDown("ToggleMesh"))
            {
                sceneManager.ToggleMesh();
            }
            else if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("ChangeCamera"))
            {
                sceneManager.ChangeCamera();
            }
            else if (Input.GetKeyDown(KeyCode.M) || Input.GetButtonDown("ToggleMissions")) 
            {
                sceneManager.ToggleMissions();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("ToggleScales"))
            {
                sceneManager.ToggleScale();
            }
        }
        public Quaternion CheckFlightInputs()
        {              
            float lx = 0f;
            float ly = 0f;
            float rx = 0f;
            float ry = 0f;

            switch (inputType)
            {
                case InputType.Keyboard:
                    lx = Input.GetAxis("Keyboard Yaw");
                    ly = Input.GetAxis("Keyboard Elv");
                    rx = Input.GetAxis("Keyboard Roll");
                    ry = Input.GetAxis("Keyboard Pitch");
                    break;
                case InputType.XboxController:
                    ly = (Input.GetAxis("Up") * 2);
                    rx =(Input.GetAxis("Roll") * 2);
                    ry =(-Input.GetAxis("Pitch") * 2);
                    lx =(Input.GetAxis("Yaw") * 2);
                    break;
            }

            if (speed == 0)
            {
                speed = .5f;
            }
            else if (speed < 0)
            {
                speed = 1 + speed;
                speed /= 2;
            }
            else
            {
                speed /= 2;
                speed += .5f;
            }

            rawYaw = lx;
            rawElv = ly;
            rawRoll = rx;
            rawPitch = ry;
            // return new Quaternion(lx, ly, rx, ry);
            return new Quaternion(rawYaw, rawElv, rawRoll, rawPitch);

        }
    }
}
