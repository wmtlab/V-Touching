/******************************************************************************
* Copyright © NeuroDigital Technologies, S.L. 2018							  *
* Licensed under the Apache License, Version 2.0 (the "License");			  *
* you may not use this file except in compliance with the License.			  *
* You may obtain a copy of the License at 									  *
* http://www.apache.org/licenses/LICENSE-2.0								  *
* Unless required by applicable law or agreed to in writing, software		  *
* distributed under the License is distributed on an "AS IS" BASIS,			  *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.	  *
* See the License for the specific language governing permissions and		  *
* limitations under the License.										      *
*******************************************************************************/

using UnityEngine;
using UnityDLL.Haptic;

namespace NDTest
{
    // <summary>
    // Inner class to delimitate the position of the joystick ball
    // </summary>
    [System.Serializable]
    public class Boundary
    {
        public float xMin, xMax, yMin, yMax, zMin, zMax;
    }

    // <summary>
    // Joystick behaviour. Control the limits, haptic feedback and action of itself.
    // </summary>
    public class Joystick : MonoBehaviour
    {
        public Transform boxCenter;

        [Header("Motion")]
        public Boundary boundary;
        public float speed;
        private Vector3 initialPos;
        private Rigidbody rbToMove;

        [Header("Haptic")]
        public Mesh hapticAreaGizmo;
        public Vector3 boxSize;
        public float intensityFactor;
        public LayerMask layer;
        private int counter;
        public int maxCounter;
        private int previousCounter;

        private bool reseted;

        void Start()
        {
            reseted = false;
            initialPos = this.transform.position;
            rbToMove = GameObject.Find("SphereJoystick").GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (GetComponentInParent<ConfigurableJoint>() != null)
            {
                Vector3 deltaPos = this.transform.position - initialPos;
                float moveHorizontal = deltaPos.x;
                float moveVertical = deltaPos.z;

                Vector3 mov = new Vector3(moveHorizontal, moveVertical, 0f);
                mov = rbToMove.transform.position + (mov * Time.deltaTime * speed);

                mov = new Vector3(Mathf.Clamp(mov.x, boundary.xMin, boundary.xMax),
                    Mathf.Clamp(mov.y, boundary.yMin, boundary.yMax),
                    Mathf.Clamp(mov.z, boundary.zMin, boundary.zMax));

                rbToMove.MovePosition(mov);

                Collider[] cols = Physics.OverlapBox(this.transform.position, boxSize, transform.rotation, layer);
                if (Vector3.Distance(transform.position, initialPos) > 0.13f)
                {
                    Destroy(GetComponentInParent<ConfigurableJoint>());
                }

                // From this, control the maximum value of the joystick and perform a haptic feedback
                if (deltaPos.magnitude > 0.07f)
                {
                    counter--;
                }
                else
                {
                    counter = maxCounter;
                }

                if (counter <= 0)
                    counter = maxCounter;

                if (counter == maxCounter)
                {
                    foreach (Collider col in cols)
                    {
                        ActuatorInfo act = col.GetComponent<ActuatorInfo>();
                        if (act != null)
                            HapticSystem.PlayPulse(intensityFactor * deltaPos.magnitude, 100, act.location, act.userIndex, act.actuator);
                    }
                }
            }
            else
            {
                if (!reseted)
                {
                    reseted = true;
                    Invoke("SpawnJoystick", 1f);
                }
            }
        }

        private void SpawnJoystick()
        {
            FindObjectOfType<JoystickSpawn>().SpawnJoystick(transform.root.gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireMesh(hapticAreaGizmo, boxCenter.position, transform.rotation, boxSize);
        }
    }
}