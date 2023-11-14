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
    /// Determines the position of the IK target
    /// </summary>
    public class ControllerIKGizmoPosition : MonoBehaviour
    {

        /// <summary>
        /// Transform that will be overwritten with the filtered position value
        /// </summary>
        public Transform tfToApplyFilteredPosition;


        /// <summary>
        /// Shoulder guide transform
        /// </summary>
        public Transform tfArm;
        /// <summary>
        /// Hand transform. Used to track the hand relative position to the shoulder
        /// </summary>
        public Transform tfHand;
        /// <summary>
        /// Defines if this controller will be applied to a right hand
        /// </summary>
        public bool isRightHand;
        /// <summary>
        /// When enabled, forces z to the max distance available in case a wrong rotation is detected
        /// </summary>
        public bool forceMaxZExtensionOnWrongRotation;

        /// <summary>
        /// Array of exclusion rules to avoid the hand getting inside undesired zones
        /// </summary>
        public PC_IKExclusionRegion[] arrayIKExclusionRegion;

        /// <summary>
        /// Following the Z+ direction of the arm, X+ value that can be reached with the hand
        /// </summary>
        public float maxElbowOpening;

        private Vector3 localPosition;
        private Vector3 worldPosition;
        private float zDistanceOnFullExtend;

        private Vector3 lastLerpedPosition;


        private void Awake()
        {
            if (tfToApplyFilteredPosition == null) tfToApplyFilteredPosition = transform;


            //Save the initial z distance to the hand on initial pose.
            //May be modified later if the initial reset rotates the hand before this distance can be obtained
            zDistanceOnFullExtend = tfArm.InverseTransformPoint(tfHand.position).z;
        }

        void Update()
        {

            lastLerpedPosition = Vector3.Lerp(lastLerpedPosition, tfHand.position, Time.deltaTime * 100);

            //converting hand world position to shoulder local position
            localPosition = tfArm.InverseTransformPoint(lastLerpedPosition);

            //If the local x position is not possible due to the rotation rules, we perform some changes
            if ((localPosition.x > maxElbowOpening && isRightHand) || (localPosition.x < maxElbowOpening && !isRightHand))
            {
                //local position in x axis will become 0 (base position in 0 rotation)
                localPosition.x = maxElbowOpening;
                //and also, if forceMaxZExtensionOnWrongRotation is enabled, we will force the hand to move to the full extended position
                if (forceMaxZExtensionOnWrongRotation) localPosition.z = zDistanceOnFullExtend;
            }

            //The resulting local position is converted to world space
            worldPosition = tfArm.TransformPoint(localPosition);


            //And the world space position is analyzed and modified by each IKExclusion module included. This will modify the world position according to their own rules and the current candidate position
            for (int i = 0; i < arrayIKExclusionRegion.Length; i++)
                worldPosition = arrayIKExclusionRegion[i].ApplyExclusion(worldPosition);

            //Finally, we apply it to the transform using this script
            tfToApplyFilteredPosition.position = worldPosition;

        }
    }
}