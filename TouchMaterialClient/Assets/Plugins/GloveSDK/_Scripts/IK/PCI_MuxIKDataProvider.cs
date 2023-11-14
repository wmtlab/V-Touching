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
    /// Base class for IK data provider
    /// </summary>
    public abstract class PCI_MuxIKDataProvider : MonoBehaviour
    {
        /// <summary>
        /// Mux IK controller
        /// </summary>
        [HideInInspector]
        public MuxIKController muxIKController;

        /// <summary>
        /// Priority value. The MuxIKDataProvider with the highest value is the one which sets values.
        /// </summary>
        public int priorityValue;

        /// <summary>
        /// If true, performs a lerp.
        /// </summary>
        [SerializeField]
        private bool useLerp;

        /// <summary>
        /// Controls that the current selected Mux is the same as the previous
        /// </summary>
        private bool previousMuxSelectionStatus = false;

        /// <summary>
        /// Returns the current position for the IK target
        /// </summary>
        /// <returns>Position of IK target</returns>
        public abstract Vector3 GetIKPosition();

        /// <summary>
        /// Register this MuxIKDataProvider into providers list
        /// </summary>
        public virtual void Awake()
        {
            RegisterMux(muxIKController);
        }

        /// <summary>
        /// Unregister this MuxIKDataProvider from providers list
        /// </summary>
        private void OnDestroy()
        {
            if (muxIKController)
                muxIKController.Unregister(this);
        }

        public void RegisterMux(MuxIKController muxIK)
        {
            muxIKController = muxIK;
            if (muxIKController)
                muxIKController.Register(this);
        }

        /// <summary>
        /// If the Leap Motion hand is active or not, set this value.
        /// </summary>
        /// <param name="setActive">True if the hand is active; false otherwise</param>
        public void SetActive(bool setActive)
        {
            if (setActive)
                priorityValue = Mathf.Abs(priorityValue);
            else
                priorityValue = -Mathf.Abs(priorityValue);

        }

        /// <summary>
        /// Compare if this MuxIKDataProvider is the previous in use
        /// </summary>
        /// <param name="muxSelectionStatus">True if this MuxIKDataProvider is the active one</param>
        public void MuxSelectionStatus(bool muxSelectionStatus)
        {
            if (previousMuxSelectionStatus != muxSelectionStatus)
                NewPickedByMuxStatus(muxSelectionStatus);

            previousMuxSelectionStatus = muxSelectionStatus;
        }

        /// <summary>
        /// Is using lerp?
        /// </summary>
        /// <returns>True if lerp is active; false otherwise</returns>
        public virtual bool UseLerp()
        {
            return useLerp;
        }

        public virtual void NewPickedByMuxStatus(bool hasBeenPickedByMux)
        {

        }
    }
}