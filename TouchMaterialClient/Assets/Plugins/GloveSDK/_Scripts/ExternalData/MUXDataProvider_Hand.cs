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
using UnityEngine;

/// <summary>
/// MuxDataProvider specific for the hand data. It gets values directly from the device.
/// </summary>
public class MUXDataProvider_Hand : PCI_MUXDataProvider
{
    private HandModelController hmc;

    private Quaternion previousHandRotation;

    override public void Awake()
    {
        base.Awake();

        sourceId = NDDevice.RAW_SOURCE_TAG;

        previousHandRotation = Quaternion.identity;

        hmc = GetComponent<HandModelController>();

        if (hmc)
        {
            userId = hmc.user;
            handLocation = hmc.handLocation;
        }
    }

    override public void Start()
    {
        // Put the same value of that finger in other phalanges
        bonesToControlRotation[(int)SensorID.Thumb2] = bonesToControlRotation[(int)SensorID.Thumb1];
        bonesToControlRotation[(int)SensorID.Index2] = bonesToControlRotation[(int)SensorID.Index1] = bonesToControlRotation[(int)SensorID.Index];
        bonesToControlRotation[(int)SensorID.Middle2] = bonesToControlRotation[(int)SensorID.Middle1] = bonesToControlRotation[(int)SensorID.Middle];
        bonesToControlRotation[(int)SensorID.Ring2] = bonesToControlRotation[(int)SensorID.Ring1] = bonesToControlRotation[(int)SensorID.Ring];
        bonesToControlRotation[(int)SensorID.Pinky2] = bonesToControlRotation[(int)SensorID.Pinky1] = bonesToControlRotation[(int)SensorID.Pinky];

        //bonesToControlRotation[(int)SensorID.Chest] = bonesToControlRotation[(int)SensorID.Arm] = bonesToControlRotation[(int)SensorID.Forearm] = true;
        bonesToControlRotationType[(int)SensorID.Hand] = Type_MUXValueType.worldSpace;

        base.Start();
    }

    /// <summary>
    /// Trigger called everytime this mux provider is enabled
    /// </summary>
    public override void OnProviderEnabled()
    {
    }

    public override void CalculateHandRotation()
    {
        throw new NotImplementedException();
    }

    public override void CalculateHandPosition()
    {
        throw new NotImplementedException();
    }

    public void UpdateRotations()
    {
        bonesToControlRotation[(int)SensorID.Thumb2] = bonesToControlRotation[(int)SensorID.Thumb1];
        bonesToControlRotation[(int)SensorID.Index2] = bonesToControlRotation[(int)SensorID.Index1] = bonesToControlRotation[(int)SensorID.Index];
        bonesToControlRotation[(int)SensorID.Middle2] = bonesToControlRotation[(int)SensorID.Middle1] = bonesToControlRotation[(int)SensorID.Middle];
        bonesToControlRotation[(int)SensorID.Ring2] = bonesToControlRotation[(int)SensorID.Ring1] = bonesToControlRotation[(int)SensorID.Ring];
        bonesToControlRotation[(int)SensorID.Pinky2] = bonesToControlRotation[(int)SensorID.Pinky1] = bonesToControlRotation[(int)SensorID.Pinky];

        if (Application.isPlaying)
        {
            handModelController.UpdateSourcesWithNewPriorities();
        }
    }
}
