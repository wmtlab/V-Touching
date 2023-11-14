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

namespace IK
{

    /// <summary>
    /// Controls the IK torso. If the useIK variable is "true", arm and forearm are going to be moved
    /// with the IK system; if it is "false", arm and forearm are going to be moved with the device sensors value.
    /// </summary>
    public class IKController : MonoBehaviour
    {
        /// <summary>
        /// Determines if the IK system is going to be used or not
        /// </summary>
        public bool useIK;
        /// <summary>
        /// Forward-Kinematics torso
        /// </summary>
        public GameObject fk_torso;
        /// <summary>
        /// Inverse-Kinematics torso
        /// </summary>
        public GameObject ik_torso;

        private void Awake()
        {
            if (useIK)
            {
                ik_torso.SetActive(true);
            }
            else
            {
                Destroy(ik_torso);
                ik_torso.SetActive(false);
                Destroy(this);
            }
        }
    }
}