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
    /// Using the Unity native IK, establishes the position of the IK
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class IKControl : MonoBehaviour
    {
        protected Animator animator;
        /// <summary>
        /// Right hand object to target
        /// </summary>
        public Transform rightHandObj = null;
        /// <summary>
        /// Left hand object to target
        /// </summary>
        public Transform leftHandObj = null;

        /// <summary>
        /// Main IK transform
        /// </summary>
        private Transform mainTfIK;


        /// <summary>
        /// Main root node of the animated skeleton 
        /// </summary>
        private Transform mainSkeletonIKRoot;

        /// <summary>
        /// Spine IK Transform
        /// </summary>
        public Transform spineIK;
        /// <summary>
        /// Spine FK Transform
        /// </summary>
        public Transform spineFK;
        /// <summary>
        /// Spine IK Transform corrected to be out of the animator hierarchy, in order to rotate/move it
        /// </summary>
        private Transform finalSpineIK;

        private Vector3 prevPosition;
        private Quaternion prevRotation;
        [HideInInspector]
        public bool corrected;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // When both avatars match (FK-IK), apply rotation to finalSpineIK
            if (corrected)
            {
                finalSpineIK.rotation = spineFK.rotation;
            }
        }

        /// <summary>
        /// Create a new parent for IK avatar. This new GameObject controls rotation of the avatar.
        /// It is needed because this avatar has an animation and it is not possible to rotate it directly.
        /// </summary>
        public void CreateNewSpineGizmo()
        {
            mainSkeletonIKRoot = this.transform;
            mainTfIK = this.transform.parent;

            GameObject go = new GameObject("IKSpine1");
            go.transform.parent = mainTfIK;
            go.transform.position = spineFK.position;
            go.transform.rotation = spineFK.rotation;

            finalSpineIK = go.transform;

            mainSkeletonIKRoot.transform.parent = finalSpineIK;

        }


        //a callback for calculating IK
        void OnAnimatorIK()
        {
            if (animator)
            {

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

                // Set the left hand target position and rotation, if one has been assigned
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}