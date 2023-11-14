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

using NDAPIWrapperSpace;
using System.Collections;
using UnityEngine;
using UnityDLL.Haptic;

namespace NDTest
{
    // <summary>
    // Reset the painting panel
    // </summary>
    public class ClearPanel : MonoBehaviour
    {
        /// <summary>
        /// Texture to paint.
        /// </summary>
        public Texture2D texture;
        /// <summary>
        /// First collider that touch this button.
        /// </summary>
        private Collider col;
        /// <summary>
        /// Haptic intensity.
        /// </summary>
        public float intensity;

        /// <summary>
        /// Resets all the texture pixels.
        /// </summary>
        /// <returns></returns>
        IEnumerator ResetScreen()
        {
            texture.SetPixels(FindObjectOfType<ScreenPaint>().initialColors);
            texture.Apply();
            yield return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            // If there is one collider touching the button already, returns
            if (col != null)
                return;

            // Gets the ActuatorInfo component
            ActuatorInfo act = other.GetComponent<ActuatorInfo>();
            // If the ActuatorInfo is not null and it is one of the fingers distal phalanx, activates the button
            if (act != null && act.phalanxIndex == NDModelAxesChanger.Phalanges.distal &&
                (act.actuator == Actuator.ACT_THUMB || act.actuator == Actuator.ACT_INDEX || act.actuator == Actuator.ACT_MIDDLE ||
                act.actuator == Actuator.ACT_RING || act.actuator == Actuator.ACT_PINKY))
            {
                // Sets the new collider
                col = other;
                // Resets the texture
                StartCoroutine("ResetScreen");
                // Play a pulse for the finger that touched the button
                HapticSystem.PlayPulse(intensity, 100, act.location, act.userIndex, act.actuator);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Resets the collider
            if (col != null && other.Equals(col))
                col = null;
        }
    }
}