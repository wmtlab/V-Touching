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
    /// Limits the maximum distance of the IK target
    /// </summary>
    public class IKExclusionRegionMaxDistance : PC_IKExclusionRegion
    {
        /// <summary>
        /// Origin of IK. In this case, FK 'shoulder' is the common Transform
        /// </summary>
        public Transform start;
        /// <summary>
        /// Target of IK. In this case, FK 'wrist' is the common Transform
        /// </summary>
        public Transform limitedTarget;

        private Vector3 v3_maxDistance;
        private float f_maxDistance;

        private void Start()
        {
            v3_maxDistance = limitedTarget.position - start.position;
            f_maxDistance = v3_maxDistance.magnitude;
        }

        /// <summary>
        /// Gets a world position and limits the maximum distance to the center of the tfPosition
        /// </summary>
        /// <param name="worldPosition">Current world position of the hand</param>
        /// <returns>Limited world position of the hand</returns>
        override public Vector3 ApplyExclusion(Vector3 worldPosition)
        {
            Vector3 newEnd = worldPosition;

            // vector pointing from the planet to the player
            Vector3 difference = worldPosition - start.position;

            // the distance between the player and the planet
            float distance = difference.magnitude;

            if (distance >= f_maxDistance)
            {
                // the direction of the launch, normalized
                Vector3 directionOnly = difference.normalized;

                newEnd = start.position + (directionOnly * f_maxDistance);

            }

            return newEnd;
        }
    }
}