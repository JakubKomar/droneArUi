/*
 * Drone Simulator - manages the visual and physics of simulated drone
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
    public class DroneSimulator : MonoBehaviour
    {
        public MirrorDrone mirrorDrone { get; private set; }
        Rigidbody rigidBody;
        public float inputDrag, drag;
        SceneManager sceneManager;
        Vector3 lastPosition = Vector3.zero;
        //public static float speedVelocity;
        public static Vector3 speedDirection;

        Transform rotors ;
        public Transform drone;

        // Use this for initialization
        public void CustomStart(SceneManager sceneManager)
        {
            
            this.sceneManager = sceneManager;
            rigidBody = drone.GetComponent<Rigidbody>();

            rotors = drone.GetChild(3);
            if (!drone)
                Debug.Log("Drone not found");

        }

        public void FixedUpdate()
        {
            //spinning rotors
            foreach (Transform rotor in rotors)
            {
                rotor.transform.Rotate(0, 10.0f, 0);
            }

            //levitating drone
            rigidBody.AddForce(transform.up * 9.81f);
                
            ///TODO
            bool receivingInput = false;
            var pitchInput = sceneManager.pitch;
            rigidBody.AddForce(drone.transform.forward * pitchInput);
            if (System.Math.Abs(pitchInput) > 0)
            {
                receivingInput = true;
            }
            var elvInput = sceneManager.elv;
            rigidBody.AddForce(drone.transform.up * elvInput);
            if (System.Math.Abs(elvInput) > 0)
            {
                Debug.Log("Receiving input TRUE elvInput > 0, elvinput: " + elvInput);
                receivingInput = true;
            }
            var rollInput = sceneManager.roll;
            rigidBody.AddForce(drone.transform.right * rollInput);
            if (System.Math.Abs(rollInput) > 0)
            {

                receivingInput = true;
            }

            var yawInput = sceneManager.yaw;
            rigidBody.AddTorque(drone.transform.up * yawInput);
            if (System.Math.Abs(yawInput) > 0)
            {

                receivingInput = true;
            }

            if (receivingInput & rigidBody.drag != inputDrag)
            {
                Debug.Log("receining input ,rigidBody.drag != inputDrag, inputDrag: " + inputDrag);
                rigidBody.drag = inputDrag;
                rigidBody.angularDrag = inputDrag ;
            }
            else if (!receivingInput & rigidBody.drag != drag)
            {
                Debug.Log("NOT receining input ,rigidBody.drag != drag, drag: " + drag);
                rigidBody.drag = drag;
                rigidBody.angularDrag = drag * .9f;
            }



            //drone rotation
            drone.transform.rotation = Quaternion.Euler(10.0f * pitchInput, drone.transform.eulerAngles.y, -10.0f * rollInput);

            //saving movement values (timestep = 0.02)
            //speedVelocity = ((drone.transform.position - lastPosition).magnitude)*50;
            //speedDirection = ((drone.transform.position - lastPosition)*50);
            lastPosition = drone.transform.position;
            
        }
        /// <summary>
        /// Resets position of simulated drone a stops his movement.
        /// </summary>
        public void ResetSimulator()
        {
            drone.position = Vector3.zero;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;

        }
    }

}