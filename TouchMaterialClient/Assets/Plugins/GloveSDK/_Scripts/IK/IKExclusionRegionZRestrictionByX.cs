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
    /// This class establishes a limit around the torso mesh. As this script is designed specifically for our torso,
    /// the local X acts as left/right and Z, as backward/forward (check the "root" Transform)
    /// </summary>
    public class IKExclusionRegionZRestrictionByX : PC_IKExclusionRegion
    {
        /// <summary>
        /// Transform to analyze the local positions
        /// </summary>
        public Transform tfLocal;
        /// <summary>
        /// Min Allowed value to analyze the X value
        /// </summary>
        public float minLocalXToApplyLimit;
        /// <summary>
        /// Max Allowed value to analyze the X value
        /// </summary>
        public float maxLocalXToApplyLimit;
        /// <summary>
        /// Min Z that is allowed in the region between minLocalXToApplyLimit and maxLocalXToApplyLimit
        /// </summary>
        public float minZAllowed;



        private Vector3 localPosition;


        /// <summary>
        /// Gets a world position and returns a position based on it that matches certain rules
        /// </summary>
        /// <param name="worldPosition">Current hand world position</param>
        /// <returns>Limited hand position</returns>
        override public Vector3 ApplyExclusion(Vector3 worldPosition)
        {
            //converting world position to local position
            localPosition = tfLocal.InverseTransformPoint(worldPosition);

            //Analyzing the X local value, if it's to the left of the minimum X value allowed or bigger than the biggest one, we return the input
            if (localPosition.x < minLocalXToApplyLimit || localPosition.x > maxLocalXToApplyLimit) return tfLocal.TransformPoint(localPosition);

            //Otherway, we check if Y is tinier than the min Z allowed value. In this case, we modify the local position to keep on top of the min Z allowed
            if (localPosition.z < minZAllowed) localPosition.z = minZAllowed;

            //Finally, we return the modified local position converting it previously to world space
            return tfLocal.TransformPoint(localPosition);
        }
    }
}
