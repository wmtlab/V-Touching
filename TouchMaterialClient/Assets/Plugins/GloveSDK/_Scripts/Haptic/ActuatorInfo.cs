/******************************************************************************
* Copyright © NeuroDigital Technologies, S.L. 2018                            *
* Licensed under the Apache License, Version 2.0 (the "License");             *
* you may not use this file except in compliance with the License.            *
* You may obtain a copy of the License at                                     *
* http://www.apache.org/licenses/LICENSE-2.0                                  *
* Unless required by applicable law or agreed to in writing, software         *
* distributed under the License is distributed on an "AS IS" BASIS,           *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.    *
* See the License for the specific language governing permissions and         *
* limitations under the License.                                              *
*******************************************************************************/

using UnityEngine;
using NDAPIWrapperSpace;

/// <summary>
/// This class stores information about actuators.
/// </summary>
public class ActuatorInfo : MonoBehaviour
{
    /// <summary>
    /// The user index assigned to this actuators 
    /// </summary>
    public int userIndex;

    /// <summary>
    /// The phalanx index. This can be used to give different sensations when touching with each phalanx (only for fingers actuators,
    /// palm actuators are considered as distal phalanx with phalanxIndex = 2)
    /// </summary>
    public NDModelAxesChanger.Phalanges phalanxIndex;

    /// <summary>
    /// The location assigned. It can be Right or Left
    /// </summary>
    public Location location;

    /// <summary>
    /// The actuator assigned.
    /// </summary>
    public Actuator actuator;

    /// <summary>
    /// Sets the user index
    /// </summary>
    public void SetUserIndex()
    {
        HandModelController hmc = this.GetComponentInParent<HandModelController>();
        if (hmc)
            this.userIndex = hmc.user;
    }

    /// <summary>
    /// Sets the location
    /// </summary>
    public void SetLocation()
    {
        HandModelController hmc = this.GetComponentInParent<HandModelController>();
        if (hmc)
            this.location = hmc.handLocation;
    }

    /// <summary>
    /// Sets the phalanx index. Proximal = 0, Intermediate = 1, Distal = 2 
    /// </summary>
    public void SetPhalanxIndex()
    {
        NDModelAxesChanger axesChanger = GetComponent<NDModelAxesChanger>();

        if (axesChanger)
            this.phalanxIndex = axesChanger.boneIndex;
    }

    /// <summary>
    ///  Sets the actuator. This only works for fingers, palm actuators has to be set manually
    /// </summary>
    public void SetActuator()
    {
        NDModelAxesChanger axesChanger = GetComponent<NDModelAxesChanger>();

        if (axesChanger)
        {
            // Sets the actuator depending on the axesChanger part
            switch (axesChanger.part)
            {
                case NDModelAxesChanger.Parts.thumb0:
                case NDModelAxesChanger.Parts.thumb1:
                    this.actuator = Actuator.ACT_THUMB;
                    break;

                case NDModelAxesChanger.Parts.index:
                    this.actuator = Actuator.ACT_INDEX;
                    break;

                case NDModelAxesChanger.Parts.middle:
                    this.actuator = Actuator.ACT_MIDDLE;
                    break;

                case NDModelAxesChanger.Parts.ring:
                    this.actuator = Actuator.ACT_RING;
                    break;

                case NDModelAxesChanger.Parts.pinky:
                    this.actuator = Actuator.ACT_PINKY;
                    break;

                default:
                    break;
            }
        }
    }
}
