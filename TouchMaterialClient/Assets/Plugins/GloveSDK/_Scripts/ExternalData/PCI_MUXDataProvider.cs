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
using NDAPIWrapperSpace;

public enum Type_MUXValueType
{
    localSpace,
    worldSpace,
    deltaLocalSpace,
    deltaWorldSpace,
}

public abstract class PCI_MUXDataProvider : MonoBehaviour
{
    public string sourceId;
    public int userId;
    public Location handLocation;
    public int priorityPositiveValue;
    private int priorityValue;

    public bool[] bonesToControlPosition = new bool[1];
    public bool[] bonesToControlRotation = new bool[Enum.GetNames(typeof(SensorID)).Length];

    public Type_MUXValueType[] bonesToControlPositionType = new Type_MUXValueType[1];
    public Type_MUXValueType[] bonesToControlRotationType = new Type_MUXValueType[Enum.GetNames(typeof(SensorID)).Length];

    protected HandModelController handModelController;
    private Quaternion[][] quatValues = new Quaternion[10][];

    public abstract void CalculateHandRotation();
    public abstract void CalculateHandPosition();

    /// <summary>
    /// Data array toset this controller rotations and positions
    /// </summary>
    private MuxData muxDataToManipulate;

    public virtual void Awake()
    {
        if (priorityPositiveValue < 0) priorityPositiveValue = -priorityPositiveValue;
    }

    public virtual void Start()
    {
        GetHMC();
        if (handModelController)
        {
            muxDataToManipulate = handModelController.CreateSourceInDictionary(this);
            if (sourceId == NDDevice.RAW_SOURCE_TAG)
            {
                SetActive(true);
            }
        }
        else
            Debug.LogError("HandModelController not found with UserID: " + userId + " and hand location: " + handLocation);
    }

    public Type_MUXValueType GetBoneRotationType(int index)
    {
        return bonesToControlRotationType[index];
    }
    public Type_MUXValueType GetBonePositionType(int index)
    {
        return bonesToControlPositionType[index];
    }

    private void GetHMC()
    {
        HandModelController[] hmcs = GameObject.FindObjectsOfType<HandModelController>();
        if (hmcs != null)
        {
            foreach (HandModelController hmc in hmcs)
            {
                if (hmc.user == userId && hmc.handLocation == handLocation)
                {
                    handModelController = hmc;
                    break;
                }
            }
        }
    }

    public void SetMUXValues(Vector3 v)
    {
        quatValues[0] = new Quaternion[] { Quaternion.Euler(v), Quaternion.identity };
        muxDataToManipulate.FillSensorsData(muxDataToManipulate);
    }

    public void SetMuxHandPosition(Vector3 v)
    {
        muxDataToManipulate.UpdateHandPosition(v);
    }

    public void SetMuxHandRotation(Quaternion q)
    {
        if (handModelController)
            muxDataToManipulate.SetData((int)SensorID.Hand, new Quaternion[] { q, Quaternion.identity });
    }

    public void SetActive(bool setActive)
    {
        if (handModelController)
        {
            if (setActive)
            {
                priorityValue = priorityPositiveValue;
                OnProviderEnabled();
            }
            else
            {
                priorityValue = -priorityPositiveValue;
            }

            handModelController.UpdateSourcesWithNewPriorities();
        }
    }

    public abstract void OnProviderEnabled();

    public int GetPriority()
    {
        return priorityValue;
    }

    public bool MustControlBoneRotation(int index)
    {
        return bonesToControlRotation[index];
    }

    public bool MustControlBonePosition(int index)
    {
        return bonesToControlPosition[index];
    }
}



