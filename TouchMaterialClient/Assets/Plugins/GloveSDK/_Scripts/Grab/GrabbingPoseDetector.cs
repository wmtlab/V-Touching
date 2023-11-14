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
using System;
using System.Collections;
using System.Collections.Generic;
using NDAPIUnity;
using NDAPIWrapperSpace;

/// <summary>
/// This class checks the pose of the fingers and determine if it is a grabbing pose
/// (With the palm contact, thumb contact or fingers closed)
/// </summary>
public class GrabbingPoseDetector : MonoBehaviour
{
    /// <summary>
    /// Determine if it is performing the grabbing pose.
    /// </summary>
    /// <value>True if hand is grabbing; otherwise, False.</value>
    public bool HandIsGrabbing { get; private set; }

    /// <summary>
    /// The closeness percentage per finger.
    /// </summary>
    [Range(0f, 1f)]
    public float closenessPercentagePerFinger = 0.5f;

    /// <summary>
    /// Number of closed fingers to trigger.
    /// </summary>
    [Range(0, 5)]
    public int closedFingersToTrigger = 3;

    /// <summary>
    /// Finger's distal phalanges.
    /// </summary>
    public Transform[] fingers;

    /// <summary>
    /// The HapticController assigned to the hand.
    /// </summary>
    private HapticController hc;

    private Dictionary<Transform, float> fingerExtendedDistance;

    /// <summary>
    /// Lock and set the variable to True.
    /// </summary>
    public bool alwaysGrabbing = false;

    void Start()
    {
        fingerExtendedDistance = new Dictionary<Transform, float>(5);
        HandModelController hmc = GetComponent<HandModelController>();
        if (hmc != null)
        {
            StartCoroutine(SetUp(hmc));
        }
        foreach (Transform t in fingers)
        {
            fingerExtendedDistance.Add(t, Vector3.Distance(transform.position, t.position));
        }

        HandIsGrabbing = false;
    }

    private IEnumerator SetUp(HandModelController hmc)
    {
        Func<bool> isDeviceNull = () =>
        {
            return hmc.device == null;
        };

        yield return new WaitWhile(isDeviceNull);

        // Gets the haptic controller
        if (hmc.device)
        {
            NDController.GetController(hmc.device.serialKey, out hc);
        }
    }

    void Update()
    {
        int closedFingers = 0;
        foreach (Transform t in fingers)
        {
            float distance = Vector3.Distance(transform.position, t.position);
            // Checks if the finger is closed
            if (distance <= fingerExtendedDistance[t] * closenessPercentagePerFinger)
            {
                // If it is closed, increased the number of closed fingers
                closedFingers++;
            }
        }
        bool contactsJoined = false;
        if (hc != null)
        {
            // If the user has the palm or the thumb contact with any other one, sets contactsJoined to true
            contactsJoined = hc.AreContactsJoined(Contact.CONT_PALM, Contact.CONT_ANY) != 0 || hc.AreContactsJoined(Contact.CONT_THUMB, Contact.CONT_ANY) != 0;
        }

        // If the user has enough closed fingers or contactsJoined is true, the hand is grabbing. Otherwise, HandIsGrabbing is false
        if (closedFingers >= closedFingersToTrigger || contactsJoined)
        {
            HandIsGrabbing = true;
        }
        else
        {
            HandIsGrabbing = false;
        }

        // If alwaysGrabbing is checked, sets HandIsGrabbing to true
        if (alwaysGrabbing)
        {
            HandIsGrabbing = true;
        }
    }
}
