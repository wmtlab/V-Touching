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

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityDLL.Haptic;
using NDAPIWrapperSpace;

namespace NDTest
{
    // <summary>
    // Control the behaviour of numpad buttons
    // </summary>
    public class HapticPasswordButton : MonoBehaviour
    {
        /// <summary>
        /// Numpad screen
        /// </summary>
        public NumpadInput input;
        public Text buttonText;
        /// <summary>
        /// True if the button is a touchable screen
        /// </summary>
        public bool isTactile;
        /// <summary>
        /// If it is a tactile button, show an effect
        /// </summary>
        public Image effect;
        /// <summary>
        /// Visible mesh
        /// </summary>
        public GameObject buttonMesh;
        public Vector3 initialLocalPos;
        public Vector3 pressedLocalPos;
        public float pressEffectTime;
        public float intensity;
        public AudioSource audioSource;
        private Collider col;
        private bool isPressing;
        private float multTime;
        private float accumTime;

        /// <summary>
        /// Reset properties
        /// </summary>
        void Start()
        {
            if (effect != null)
                effect.enabled = false;

            isPressing = false;

            multTime = 1 / pressEffectTime;
        }

        void Update()
        {
            // Is a physical button, control the movement of the button
            if (!isTactile)
            {
                if (isPressing)
                {
                    accumTime += Time.deltaTime * multTime;
                    buttonMesh.transform.localPosition = Vector3.Lerp(initialLocalPos, pressedLocalPos, accumTime);
                    if (accumTime >= 1f)
                    {
                        accumTime = 0f;
                        isPressing = false;
                    }
                }
                else
                {
                    if (accumTime < 1f)
                    {
                        accumTime += Time.deltaTime * multTime;
                        buttonMesh.transform.localPosition = Vector3.Lerp(pressedLocalPos, initialLocalPos, accumTime);
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (col != null)
                return;

            // Haptic feedback
            ActuatorInfo act = other.GetComponent<ActuatorInfo>();
            if (act != null && act.phalanxIndex == NDModelAxesChanger.Phalanges.distal &&
                (act.actuator == Actuator.ACT_THUMB || act.actuator == Actuator.ACT_INDEX || act.actuator == Actuator.ACT_MIDDLE ||
                act.actuator == Actuator.ACT_RING || act.actuator == Actuator.ACT_PINKY))
            {
                col = other;
                input.EnterText(buttonText.text);
                HapticSystem.PlayPulse(intensity, 100, act.location, act.userIndex, act.actuator);
                audioSource.PlayOneShot(audioSource.clip);

                if (isTactile)
                {
                    if (effect != null)
                    {
                        StartCoroutine(ShowEffect());
                    }
                }
                else
                {
                    if (buttonMesh != null)
                    {
                        accumTime = 0f;
                        isPressing = true;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (col != null && other.Equals(col))
                col = null;
        }

        public void ResetCol()
        {
            col = null;
        }

        IEnumerator ShowEffect()
        {
            effect.enabled = true;
            yield return new WaitForSecondsRealtime(0.25f);
            effect.enabled = false;
        }
    }
}