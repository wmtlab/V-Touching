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
using UnityDLL.Haptic;

public class HapticCubeTest : MonoBehaviour
{
    public float speed = 5.0f;
    public float intensity = 0.5f;

    // Update is called once per frame
    void Update()
    {
        // Moves the cube along the z axis
        float movementZ = Input.GetAxis("Vertical");
        this.transform.Translate(new Vector3(0f, 0f, movementZ * Time.deltaTime));
    }

    void OnTriggerEnter(Collider col)
    {
        // Gets the HapticData
        ActuatorInfo data = col.GetComponent<ActuatorInfo>();
        // If the collider has the component HapticData, plays a pulse with the given information
        if (data != null)
        {
            HapticSystem.PlayPulse(intensity, 100, data.location, data.userIndex, data.actuator);
        }
    }

    void OnTriggerStay(Collider col)
    {
        // Gets the HapticData
        ActuatorInfo data = col.GetComponent<ActuatorInfo>();
        // If the collider has the component HapticData, plays a value with the given information
        if (data != null)
        {
            HapticSystem.PlayValue(intensity / 2, data.location, data.userIndex, data.actuator);
        }
    }

    void OnTriggerExit(Collider col)
    {
        // Gets the HapticData
        ActuatorInfo data = col.GetComponent<ActuatorInfo>();
        // If the collider has the component HapticData, stops the actuator
        if (data != null)
        {
            HapticSystem.StopActuators(data.location, data.userIndex, data.actuator);
        }
    }
}
