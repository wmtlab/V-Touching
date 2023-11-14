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

namespace NDTest
{

    public class SpawnSphere : MonoBehaviour
    {
        /// <summary>
        /// The sphere that will be spawned.
        /// </summary>
        public GameObject sphere;

        // Update is called once per frame
        void Update()
        {
            // When the user presses the S key, spawns a sphere
            if (Input.GetKeyDown(KeyCode.S))
            {
                // Sets an offset to spawn the sphere
                Vector3 offset = new Vector3(Random.Range(-0.001f, 0.001f), 0f, Random.Range(-0.001f, 0.001f));
                // Spawns the sphere
                Instantiate(sphere, transform.position + offset, Quaternion.identity);
            }
        }
    }
}