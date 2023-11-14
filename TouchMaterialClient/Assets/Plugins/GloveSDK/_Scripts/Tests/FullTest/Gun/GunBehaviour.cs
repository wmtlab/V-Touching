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
using NDAPIWrapperSpace;
using UnityDLL.Contacts;
using UnityDLL.Haptic;

namespace NDTest
{
    // <summary>
    // Control the behaviour of the gun, such as intensity, gesture detection...
    // </summary>
    public class GunBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The shots that will be spawned.
        /// </summary>
        public GameObject shot;
        /// <summary>
        /// The origin for shots.
        /// </summary>
        public Transform shotSpawn;

        /// <summary>
        /// The maximum time in seconds that will take the fade out effect when shooting.
        /// </summary>
        public float maxSecondsToFade;

        /// <summary>
        /// The fade out haptic effect curve.
        /// </summary>
        public AnimationCurve accelerationToFade;
        /// <summary>
        /// Default intensity for shooting haptic effect.
        /// </summary>
        public float startIntensity;
        /// <summary>
        /// Factor applied for the fade in haptic effect.
        /// </summary>
        public float intensityFactor;

        /// <summary>
        /// Is the gun charging?
        /// </summary>
        private bool isCharging;
        /// <summary>
        /// Is the fade out effect activated?
        /// </summary>
        private bool activateFade;

        // Some values used for the fade out effect
        private float accumTimeToFade;
        private float timeMultTimeToFade;

        /// <summary>
        /// The intensity played when charging the gun.
        /// </summary>
        private float intensity;
        /// <summary>
        /// Last intensity played before starting the fade out effect.
        /// </summary>
        private float lastIntensity;
        /// <summary>
        /// The intensity played while the fade out effect is playing.
        /// </summary>
        private float intensityFade;


        private HandModelController hmc;

        void Awake()
        {
            accumTimeToFade = 0;
        }

        // Use this for initialization
        void Start()
        {
            isCharging = false;
            activateFade = false;
            intensity = startIntensity;
            lastIntensity = 0f;
            intensityFade = 0f;

            hmc = GetComponentInParent<HandModelController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!activateFade)
            {
                // Sets the gun emission color to green
                this.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.green);

                // When the user performs the gun gesture, starts charging the gun
                if (GunTestController.gunEnabled && ContactsSystem.AreContactsJoined(Contact.CONT_MIDDLE, Contact.CONT_PALM, hmc.handLocation, 1) &&
                     !ContactsSystem.AreContactsJoined(Contact.CONT_INDEX, Contact.CONT_ANY, hmc.handLocation, 1))
                {
                    isCharging = true;
                    activateFade = false;

                    intensity += Time.deltaTime * intensityFactor;
                    if (intensity >= 1f)
                        intensity = 1f;

                    HapticSystem.PlayValue(intensity, hmc.handLocation, 1);
                }
                else
                {
                    // If the user was charging and stops performing the gun gesture, starts the fade out effect and shoots
                    if (isCharging)
                    {
                        this.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.red);
                        lastIntensity = intensity;
                        timeMultTimeToFade = 1 / (maxSecondsToFade * lastIntensity);
                        intensity = startIntensity;
                        activateFade = true;

                        isCharging = false;

                        GameObject shotSpawned = Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                        shotSpawned.GetComponent<ShotBehaviour>().Shoot(lastIntensity);
                    }
                }
            }
            else
            {
                /**** FADE OUT EFFECT ****/

                accumTimeToFade += Time.deltaTime * timeMultTimeToFade;
                // Change the gun emission color from red to green.
                this.GetComponent<Renderer>().materials[3].SetColor("_EmissionColor", Color.Lerp(Color.red, Color.green, accumTimeToFade));

                // Sets the new intensity evaluating the animation curve
                intensityFade = lastIntensity - accelerationToFade.Evaluate(accumTimeToFade) * lastIntensity;

                // If accumTimeToFade is greater or equal to 1, deactivates the fade out effect
                if (accumTimeToFade >= 1)
                {
                    accumTimeToFade = 0;
                    activateFade = false;
                }

                // Sets the value for all actuators depending on intensityFade value
                HapticSystem.PlayValue(intensityFade, hmc.handLocation, 1);
            }
        }
    }
}