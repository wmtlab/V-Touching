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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityDLL.Haptic;
using NDAPIWrapperSpace;

namespace NDTest
{
    public class HapticDepthTest : MonoBehaviour
    {
        // List of actuators inside of the box
        private List<ActuatorInfo> actInfoList = new List<ActuatorInfo>();
        // Actuator images used to show the intensity applied for each actuator
        public SpriteRenderer[] actuatorSprites;
        // Texture used to show the intensity for each actuator when hands are inside the box
        public Texture2D heatMap;
        // Collider attached
        private Collider col;
        // The Collider height
        private float colHeight;

        void Start()
        {
            // Gets the Collider
            col = GetComponent<Collider>();
            // Gets the Collider height
            colHeight = col.bounds.max.y - col.bounds.min.y;
        }

        void Update()
        {
            if (actInfoList != null)
            {
                // For each actuator inside the box, sets its intensity depending on its position
                foreach (ActuatorInfo actInfo in actInfoList)
                {
                    // Calculates the intensity
                    float intensity = Mathf.Clamp01((col.bounds.max.y - actInfo.gameObject.transform.position.y) / colHeight);

                    // Calls the HapticSystem and sets the value
                    if (HapticSystem.PlayValue(intensity, actInfo.location, actInfo.userIndex, actInfo.actuator))
                    {
                        // Sets the actuator images color depending on the intensity
                        switch (actInfo.actuator)
                        {
                            case Actuator.ACT_THUMB:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[0].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[10].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            case Actuator.ACT_INDEX:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[1].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[11].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            case Actuator.ACT_MIDDLE:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[2].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[12].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            case Actuator.ACT_RING:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[3].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[13].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            case Actuator.ACT_PINKY:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[4].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[14].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            case Actuator.ACT_PALM_INDEX_UP:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[5].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[15].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            case Actuator.ACT_PALM_INDEX_DOWN:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[6].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[16].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            case Actuator.ACT_PALM_MIDDLE_UP:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[7].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[17].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            case Actuator.ACT_PALM_PINKY_UP:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[8].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[18].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            case Actuator.ACT_PALM_PINKY_DOWN:
                                if (actInfo.location == Location.LOC_LEFT_HAND)
                                    actuatorSprites[9].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                else
                                    actuatorSprites[19].color = heatMap.GetPixel((int)Math.Round(intensity * 99f, MidpointRounding.AwayFromZero), 0);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public void ClearHaptics()
        {
            actInfoList.Clear();

            HapticSystem.StopActuators();

            for (int i = 0; i < actuatorSprites.Length; i++)
            {
                actuatorSprites[i].color = new Color(0, 0, 0, 0);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            ActuatorInfo actInfo = other.GetComponent<ActuatorInfo>();
            if (actInfo)
            {
                // Adds the actuator to the list
                actInfoList.Add(actInfo);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            ActuatorInfo actInfo = other.GetComponent<ActuatorInfo>();
            if (actInfo)
            {
                // Remove the actuator from the list
                actInfoList.Remove(actInfo);

                // Stops the actuator
                if (HapticSystem.StopActuators(actInfo.location, actInfo.userIndex, actInfo.actuator))
                {
                    // Resets the actuator images
                    switch (actInfo.actuator)
                    {
                        case Actuator.ACT_THUMB:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[0].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[10].color = new Color(0, 0, 0, 0);
                            break;
                        case Actuator.ACT_INDEX:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[1].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[11].color = new Color(0, 0, 0, 0);
                            break;
                        case Actuator.ACT_MIDDLE:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[2].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[12].color = new Color(0, 0, 0, 0);
                            break;
                        case Actuator.ACT_RING:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[3].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[13].color = new Color(0, 0, 0, 0);
                            break;
                        case Actuator.ACT_PINKY:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[4].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[14].color = new Color(0, 0, 0, 0);
                            break;
                        case Actuator.ACT_PALM_INDEX_UP:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[5].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[15].color = new Color(0, 0, 0, 0);
                            break;
                        case Actuator.ACT_PALM_INDEX_DOWN:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[6].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[16].color = new Color(0, 0, 0, 0);
                            break;
                        case Actuator.ACT_PALM_MIDDLE_UP:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[7].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[17].color = new Color(0, 0, 0, 0);
                            break;
                        case Actuator.ACT_PALM_PINKY_UP:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[8].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[18].color = new Color(0, 0, 0, 0);
                            break;
                        case Actuator.ACT_PALM_PINKY_DOWN:
                            if (actInfo.location == Location.LOC_LEFT_HAND)
                                actuatorSprites[9].color = new Color(0, 0, 0, 0);
                            else
                                actuatorSprites[19].color = new Color(0, 0, 0, 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}