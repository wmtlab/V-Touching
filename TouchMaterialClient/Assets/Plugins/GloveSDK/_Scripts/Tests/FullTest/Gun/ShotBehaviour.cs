﻿/******************************************************************************
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

namespace NDTest
{
    public class ShotBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The minimum speed for shots.
        /// </summary>
        public float minSpeed;
        /// <summary>
        /// The maximum speed for shots.
        /// </summary>
        public float maxSpeed;
        /// <summary>
        /// The explosion effect instantiated when the shot hits something.
        /// </summary>
        public GameObject explosion;

        /// <summary>
        /// Sets the shot velocity depending on the speedFactor.
        /// </summary>
        /// <param name="speedFactor">Modifies the shot velocity.</param>
        public void Shoot(float speedFactor)
        {
            GetComponent<Rigidbody>().velocity = transform.forward * (minSpeed + ((maxSpeed - minSpeed) * speedFactor));
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Instantiate the explosion
            Instantiate(explosion, transform.position, Quaternion.identity);
            // Destroy this object
            Destroy(gameObject);
        }
    }
}