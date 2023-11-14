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
    // <summary>
    // Control the scene "FullTest"
    // </summary>
    public class FullTestController : MonoBehaviour
    {
        /// <summary>
        /// This contains all the test tables.
        /// </summary>
        public Transform[] testsPosition;
        /// <summary>
        /// The player object.
        /// </summary>
        public Transform player;
        /// <summary>
        /// Represents the active test.
        /// </summary>
        private int testIndex;
        /// <summary>
        /// GunController used to enable or disable the gun.
        /// </summary>
        private GunTestController gunController;
        /// <summary>
        /// HapticDepthTest controller.
        /// </summary>
        private HapticDepthTest hapticCubeController;
        /// <summary>
        /// HapticPasswordButton array.
        /// </summary>
        private HapticPasswordButton[] hapticButtons;

        // Use this for initialization
        void Start()
        {
            // Sets the initial test index
            testIndex = 0;
            // Gets the GunController
            gunController = GameObject.FindObjectOfType<GunTestController>();
            hapticCubeController = GameObject.FindObjectOfType<HapticDepthTest>();
            hapticButtons = GameObject.FindObjectsOfType<HapticPasswordButton>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();

            // Change the active test when the user presses the right or left arrow
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Checks if the active index can increase or not
                if (testIndex < testsPosition.Length - 1)
                    testIndex++;

                // If the active test is not the haptic cube, clears colliders
                if (testIndex != 0)
                    hapticCubeController.ClearHaptics();

                // If the active test is not the numpad, clears colliders
                if (testIndex != testsPosition.Length - 2)
                {
                    foreach (HapticPasswordButton hb in hapticButtons)
                    {
                        hb.ResetCol();
                    }
                }

                // If the active test is the last one, enables the gun. Otherwise, disables it
                if (testIndex == 3)
                    GunTestController.gunEnabled = true;
                else
                    GunTestController.gunEnabled = false;

                // Checks the active test to show or hide some objects
                gunController.CheckTable();
                // Changes the player position to the active test position
                player.position = testsPosition[testIndex].position;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // Checks if the active index can decrease or not
                if (testIndex > 0)
                    testIndex--;

                // If the active test is the haptic cube, clears colliders
                if (testIndex != 0)
                    hapticCubeController.ClearHaptics();

                // If the active test is not the numpad, clears colliders
                if (testIndex != testsPosition.Length - 2)
                {
                    foreach (HapticPasswordButton hb in hapticButtons)
                    {
                        hb.ResetCol();
                    }
                }

                // If the active test is the last one, enables the gun. Otherwise, disables it
                if (testIndex == 3)
                    GunTestController.gunEnabled = true;
                else
                    GunTestController.gunEnabled = false;

                // Checks the active test to show or hide some objects
                gunController.CheckTable();
                // Changes the player position to the active test position
                player.position = testsPosition[testIndex].position;
            }
        }
    }
}