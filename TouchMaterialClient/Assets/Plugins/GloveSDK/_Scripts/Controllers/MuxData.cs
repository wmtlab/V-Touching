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

[Serializable]
/// <summary>
/// Registry class of HMC mux data.
/// </summary>
public class MuxData
{
    /// <summary>
    /// Source ID
    /// </summary>
    public PCI_MUXDataProvider sourceProvider;
    /// <summary>
    /// Determines if it is active or not
    /// </summary>
    public bool isActive;
    /// <summary>
    /// Dictionary of Quaternion[] that stores rotations (Y and X components) of each component.
    /// Only fingers use both components separately
    /// </summary>
    private Dictionary<int, Quaternion[]> quatData;
    /// <summary>
    /// In case of Gloveone, fingers data is stored in this dictionary.
    /// </summary>
    private Dictionary<int, float> gloveoneFlexData;
    /// <summary>
    /// Mask that determines which component is going to be used from this MuxData
    /// </summary>
    private List<bool> useThis;

    /// <summary>
    /// Using this value, hand can be positioned
    /// </summary>
    private Vector3 handPosition;

    /// <summary>
    /// Constructor. Creates a MuxData registry in HMC mux and initializes properties
    /// </summary>
    /// <param name="sourceProvider">Source Provider Reference</param>
    public MuxData(PCI_MUXDataProvider sourceProvider)
    {
        this.sourceProvider = sourceProvider;
        quatData = new Dictionary<int, Quaternion[]>();
        gloveoneFlexData = new Dictionary<int, float>();
        useThis = new List<bool>(new bool[Enum.GetNames(typeof(SensorID)).Length]);
        ResetFingersMask();
        isActive = true;
    }

    /// <summary>
    /// Returns the mux data provider for this data source
    /// </summary>
    /// <returns></returns>
    public PCI_MUXDataProvider GetMuxSourceDataProvider()
    {
        return sourceProvider;
    }

    /// <summary>
    /// Returns useThis list
    /// </summary>
    /// <returns>List of bool that represents which component can be used from this MuxData</returns>
    public List<bool> GetUseThis()
    {
        return useThis;
    }

    /// <summary>
    /// With this method, it can be established which fingers are going to be used. Useful for gestures
    /// </summary>
    /// <param name="fingers">Array of integers (component ID)</param>
    public void SetFingersMask(params int[] fingers)
    {
        foreach (int i in fingers)
        {
            useThis[i] = true;
        }
    }

    /// <summary>
    /// Resets fingers mask
    /// </summary>
    public void ResetFingersMask()
    {
        for (int i = 0; i < useThis.Count; i++)
            useThis[i] = false;
    }

    /// <summary>
    /// Sets value of Y and X component to a specified bone
    /// </summary>
    /// <param name="bone">Bone ID</param>
    /// <param name="value">Y and X rotations</param>
    public void SetData(int bone, Quaternion[] value)
    {
        Quaternion[] q;
        if (quatData.TryGetValue(bone, out q))
            quatData[bone] = value;
        else
            quatData.Add(bone, value);
    }

    /// <summary>
    /// Given a bone ID, return its rotation
    /// </summary>
    /// <param name="bone">Bone ID</param>
    /// <returns>Y and X rotations</returns>
    public Quaternion[] GetSensorData(int bone)
    {
        Quaternion[] q;
        if (!quatData.TryGetValue(bone, out q))
            q = new Quaternion[] { Quaternion.identity, Quaternion.identity };

        return q;
    }

    /// <summary>
    /// Given a bone ID, return its value from
    /// </summary>
    /// <param name="bone">Bone ID</param>
    /// <returns>Float value of this finger, from 0 to 1</returns>
    public float GetFlexData(int bone)
    {
        float floatValue;
        gloveoneFlexData.TryGetValue(bone, out floatValue);

        return floatValue;
    }

    /// <summary>
    /// Fills the whole MuxData with Y and X rotations of each component
    /// </summary>
    /// <param name="quatValues">Array of Y and X rotations</param>
    public void FillSensorsData(Quaternion[][] quatValues)
    {
        Quaternion[] q;
        for (int i = 0; i < quatValues.Length; i++)
        {
            if (quatData.TryGetValue(i, out q))
                quatData[i] = quatValues[i];
            else
                quatData.Add(i, quatValues[i]);
        }
    }

    /// <summary>
    /// Fills the whole MuxData with Y and X rotations of each component copying data from an
    /// external MuxData
    /// </summary>
    /// <param name="muxData">Origin MuxData</param>
    public void FillSensorsData(MuxData muxData)
    {
        foreach (KeyValuePair<int, Quaternion[]> sensor in muxData.quatData)
        {
            SetData(sensor.Key, muxData.GetSensorData(sensor.Key));
        }

        handPosition = muxData.handPosition;
    }
    /// <summary>
    /// Sets hand rotation
    /// </summary>
    /// <param name="handRot">Hand rotation in Y and X components</param>
    public void UpdateHandRotation(Quaternion[] handRot)
    {
        quatData[(int)SensorID.Hand] = handRot;
    }

    /// <summary>
    /// Sets hand position
    /// </summary>
    /// <param name="position">position</param>
    public void UpdateHandPosition(Vector3 position)
    {
        handPosition = position;
    }

    /// <summary>
    /// Returns hand position
    /// </summary>
    /// <returns>Vector3 with hand global position</returns>
    public Vector3 GetHandPosition()
    {
        return handPosition;
    }

    /// <summary>
    /// Fills MuxData values using Quaternion[] for hand rotation and float[] for fingers values
    /// </summary>
    /// <param name="quatValue">Hand rotation</param>
    /// <param name="flexValues">Fingers values, from 0 to 1</param>
    public void FillSensorsData(Quaternion[] quatValue, float[] flexValues)
    {
        // Hand sensor
        Quaternion[] q;
        int handBone = (int)SensorID.Hand;
        if (quatData.TryGetValue(handBone, out q))
            quatData[handBone] = quatValue;
        else
            quatData.Add(handBone, quatValue);

        // Fingers
        float flexValue;
        for (int i = 0; i < flexValues.Length; i++)
        {
            if (gloveoneFlexData.TryGetValue(i, out flexValue))
                gloveoneFlexData[i] = flexValues[i];
            else
                gloveoneFlexData.Add(i, flexValues[i]);
        }
    }
}
