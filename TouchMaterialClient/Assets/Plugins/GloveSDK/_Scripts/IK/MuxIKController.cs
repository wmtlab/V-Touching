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

using System.Collections.Generic;
using UnityEngine;

namespace IK
{
    /// <summary>
    /// Bridge class between MuxIKData and ControllerIKGizmoPositionExternal
    /// </summary>
    public class MuxIKController : MonoBehaviour
    {
        /// <summary>
        /// Control the position of the IK target
        /// </summary>
        private ControllerIKGizmoPositionExternal controllerIKGizmoPositionExternal;
        /// <summary>
        /// List of PCI_MuxIKDataProvider that can set the IK target position
        /// </summary>
        private List<PCI_MuxIKDataProvider> listMuxIKDataProviders = new List<PCI_MuxIKDataProvider>();
        /// <summary>
        /// Highest priority of the MuxIKDataProvider that is setting values
        /// </summary>
        private float cHighestMuxPriority;
        /// <summary>
        /// Controller index that is being used from listMuxIKDataProviders
        /// </summary>
        private int indexController;

        private void Awake()
        {
            controllerIKGizmoPositionExternal = GetComponent<ControllerIKGizmoPositionExternal>();
        }

        public void Register(PCI_MuxIKDataProvider muxIKDataProvider)
        {
            listMuxIKDataProviders.Add(muxIKDataProvider);
        }

        public void Unregister(PCI_MuxIKDataProvider muxIKDataProvider)
        {
            listMuxIKDataProviders.Remove(muxIKDataProvider);

        }

        void Update()
        {
            cHighestMuxPriority = -Mathf.Infinity;

            for (int i = 0; i < listMuxIKDataProviders.Count; i++)
            {
                if (listMuxIKDataProviders[i].priorityValue > cHighestMuxPriority)
                {
                    cHighestMuxPriority = listMuxIKDataProviders[i].priorityValue;
                    indexController = i;
                }
            }

            // Set enabled or disabled signals to the Mux controllers
            for (int i = 0; i < listMuxIKDataProviders.Count; i++)
                listMuxIKDataProviders[i].MuxSelectionStatus(i == indexController);

            controllerIKGizmoPositionExternal.SetCandidatePosition(listMuxIKDataProviders[indexController].GetIKPosition(), listMuxIKDataProviders[indexController].UseLerp());
        }
    }
}