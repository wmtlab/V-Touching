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

using NDAPIUnity;
using NDAPIWrapperSpace;
using UnityEngine;

namespace NDTest
{
    // <summary>
    // Control the gun section and its targets
    // </summary>
    public class GunTestController : MonoBehaviour
    {
        /// <summary>
        /// The right gun game object.
        /// </summary>
        public GameObject rightGun;
        /// <summary>
        /// The left gun game object.
        /// </summary>
        public GameObject leftGun;
        /// <summary>
        /// Targets game objects.
        /// </summary>
        public GameObject[] targets;
        /// <summary>
        /// The target pillars.
        /// </summary>
        public Renderer[] targetPillars;
        /// <summary>
        /// ShowHideRoboticMesh used to hide or show the hands depending on if the gun is enabled or not.
        /// </summary>
        public ShowHideRoboticMesh rendererController;
        /// <summary>
        /// Is the gun enabled?
        /// </summary>
        public static bool gunEnabled;

        // Use this for initialization
        void Start()
        {
            gunEnabled = false;
            CheckTable();
        }

        /// <summary>
        /// If gunEnabled is true, hides the hands and shows the targets and the gun. Otherwise, shows the hands again and hides the gun and the targets.
        /// </summary>
        public void CheckTable()
        {
            HandModelController hmcR = rightGun.GetComponentInParent<HandModelController>();
            HandModelController hmcL = leftGun.GetComponentInParent<HandModelController>();

            if (gunEnabled)
            {
                if (hmcR && hmcR.device)
                    rightGun.GetComponent<Renderer>().enabled = true;
                if (hmcL && hmcL.device)
                    leftGun.GetComponent<Renderer>().enabled = true;

                foreach (GameObject target in targets)
                {
                    target.SetActive(true);
                }
                foreach (Renderer floor in targetPillars)
                {
                    floor.enabled = true;
                }

                rendererController.HideActiveHands();
                rendererController.HideActiveArms();
            }
            else
            {
                if (hmcR && hmcR.device)
                    rightGun.GetComponent<Renderer>().enabled = false;
                if (hmcL && hmcL.device)
                    leftGun.GetComponent<Renderer>().enabled = false;

                foreach (GameObject target in targets)
                {
                    target.SetActive(false);
                }
                foreach (Renderer floor in targetPillars)
                {
                    floor.enabled = false;
                }

                rendererController.ShowActiveHands();
                rendererController.ShowActiveArms();
            }
        }
    }
}