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
using System.Collections;
using UnityEngine;
using NDAPIWrapperSpace;
using UnityDLL.Contacts;

// <summary>
// Performing gestures with the glove, different balls are shown
// </summary>
public class GesturesTest : MonoBehaviour
{
    public GameObject sphere1, sphere2, sphere3;

    private HandModelController hmc;

    void Start()
    {
        // Hides all the spheres
        sphere1.SetActive(false);
        sphere2.SetActive(false);
        sphere3.SetActive(false);

        // Gets the HandModelController
        hmc = this.GetComponent<HandModelController>();
        StartCoroutine(SetListeners());
    }

    private IEnumerator SetListeners()
    {
        Func<bool> isDeviceNull = () =>
        {
            return hmc.device == null;
        };

        yield return new WaitWhile(isDeviceNull);

        // Sets the listeners for each gesture
        if (hmc.device)
        {
            hmc.device.isPinchGestureEvent += new NDDevice.isPinchGestureHandler(PinchGesture);
            hmc.device.isMiddlePinchGestureEvent += new NDDevice.isMiddlePinchGestureHandler(MiddlePinchGesture);
            hmc.device.isThreePinchGestureEvent += new NDDevice.isThreePinchGestureHandler(ThreePinchGesture);
        }
    }

    void Update()
    {
        if (hmc.device)
        {
            // We can hide all the spheres checking if contacts are not joined
            if (!ContactsSystem.AreContactsJoined(Contact.CONT_INDEX, Contact.CONT_THUMB, hmc.handLocation, hmc.user) &&
                !ContactsSystem.AreContactsJoined(Contact.CONT_MIDDLE, Contact.CONT_THUMB, hmc.handLocation, hmc.user))
            {
                sphere1.SetActive(false);
                sphere2.SetActive(false);
                sphere3.SetActive(false);
            }
        }
    }

    // This is called when the hand is making pinch gesture. Shows the sphere 1
    public void PinchGesture()
    {
        sphere1.SetActive(true);
        sphere2.SetActive(false);
        sphere3.SetActive(false);
    }

    // This is called when the hand is making middle pinch gesture. Shows the sphere 2
    public void MiddlePinchGesture()
    {
        sphere1.SetActive(false);
        sphere2.SetActive(true);
        sphere3.SetActive(false);
    }

    // This is called when the hand is making three pinch gesture. Shows the sphere 3
    public void ThreePinchGesture()
    {
        sphere1.SetActive(false);
        sphere2.SetActive(false);
        sphere3.SetActive(true);
    }
}
