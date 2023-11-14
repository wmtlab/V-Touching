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
    /// Using the Unity native IK system, the Avatar must be controlled through an AnimatorController.
    /// When it is played, this avatar may can be displaced because of the animation. With this script,
    /// this displacement can be fixed and match both FK and IK avatars.
    /// </summary>
    public class TransformPositionKeeper : MonoBehaviour
    {

        /// <summary>
        /// This is a transform used to reposition the character in the world macthing the pelvis editor position with the initial runtime one.
        /// Must be a transform not affected by the own animation, because a displacement will be applied to it.
        /// </summary>
        public Transform tfRoot;
        /// <summary>
        /// Once it is calculated, this offset is applied to the tfRoot Transform, matching both avatars (FK-IK)
        /// </summary>
        public Vector3 offsetIkToFk;

        public bool destroyOnAwake;
        public Transform fkShoulder, ikShoulder;

        private IKControl ikControl;
        private bool calculateOffset;

        private void Awake()
        {
            tfRoot.position = tfRoot.position + tfRoot.rotation * offsetIkToFk;
            ikControl = GetComponent<IKControl>();
            if (ikControl)
            {
                ikControl.CreateNewSpineGizmo();
                ikControl.corrected = true;
            }

            if (destroyOnAwake)
                Destroy(this);
        }

        private void LateUpdate()
        {
            if (calculateOffset)
            {
                Vector3 offset = fkShoulder.position - ikShoulder.position;
                Debug.Log("Offset IK to FK: " + (offset).ToString("F8"));
                calculateOffset = false;
            }
        }

        public void CalculateOffset()
        {
            if (!calculateOffset)
                calculateOffset = true;
        }
    }
}