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

using UnityDLL.Haptic;
using UnityEngine;

namespace NDTest
{
    public class SphereBehaviour : MonoBehaviour
    {
        public GameObject explosion;
        public float intensity = 0.5f;

        // Update is called once per frame
        void Update()
        {
            // When the sphere position is under the floor, destroys this sphere
            if (transform.position.y < -10f)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Ignores collisions with the grabbing table and the player
            if (other.name.Contains("Grabbing") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
                return;

            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // Gets the HapticData
                ActuatorInfo data = other.GetComponent<ActuatorInfo>();
                // If the collider has the component HapticData, plays a pulse with the given information
                if (data != null)
                {
                    HapticSystem.PlayPulse(intensity, 100, data.location, data.userIndex, data.actuator);
                }
            }
            else
            {
                // When the sphere hits something, destroys this sphere and instantiates an explosion
                Instantiate(explosion, this.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        void OnTriggerStay(Collider col)
        {
            // Gets the HapticData
            ActuatorInfo data = col.GetComponent<ActuatorInfo>();
            // If the collider has the component HapticData, plays pulses with the given information
            if (data != null)
            {
                HapticSystem.PlayPulse(intensity / 2, 100, data.location, data.userIndex, data.actuator);
            }
        }
    }
}