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
using UnityDLL.Core;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This class creates and stores Transforms of a specific gesture.
/// </summary>
[System.Serializable]
public class HandGesture : MonoBehaviour
{
    /// <summary>
    /// Gesture that is going to be stored.
    /// </summary>
    public HandGestures gesture;

    /// <summary>
    /// Thumb phalanges
    /// </summary>
    public Transform[] thumbBones;
    /// <summary>
    /// Index phalanges
    /// </summary>
    public Transform[] indexBones;
    /// <summary>
    /// Middle phalanges
    /// </summary>
    public Transform[] middleBones;
    /// <summary>
    /// Ring phalanges
    /// </summary>
    public Transform[] ringBones;
    /// <summary>
    /// Pinky phalanges
    /// </summary>
    public Transform[] pinkyBones;

    /// <summary>
    /// Quaternion[][] where are going to be stored finger phalanges and its components, Y and X.
    /// </summary>
    private Quaternion[][] quaternions;

    /// <summary>
    /// Name of the gesture prefab
    /// </summary>
    private string prefabFile;

    private string prefabFolder = "Assets/_Prefabs/Gestures/";

    /// <summary>
    /// Remove every script attached to GameObjects of the hand.
    /// </summary>
    public void CleanGameObjects()
    {
        Component[] components = GetComponentsInChildren<Component>();

        foreach (Component component in components)
        {
            if (component.GetType() != typeof(Transform) && component.GetType() != typeof(HandGesture))
            {
                DestroyImmediate(component);
            }
        }
    }

    /// <summary>
    /// Transform the bone's orientation to the one that is valid for our device, using NDModelAxesChanger.
    /// </summary>
    public void SetUpBones()
    {
        for (int i = 0; i < thumbBones.Length; i++)
        {
            NDModelAxesChanger axesChanger = thumbBones[i].GetComponent<NDModelAxesChanger>();

            if (axesChanger)
            {
                Transform newBone = axesChanger.HierarchySetup();
                thumbBones[i] = newBone;
            }
        }

        for (int i = 0; i < indexBones.Length; i++)
        {
            NDModelAxesChanger axesChanger = indexBones[i].GetComponent<NDModelAxesChanger>();

            if (axesChanger)
            {
                Transform newBone = axesChanger.HierarchySetup();
                indexBones[i] = newBone;
            }
        }

        for (int i = 0; i < middleBones.Length; i++)
        {
            NDModelAxesChanger axesChanger = middleBones[i].GetComponent<NDModelAxesChanger>();

            if (axesChanger)
            {
                Transform newBone = axesChanger.HierarchySetup();
                middleBones[i] = newBone;
            }
        }

        for (int i = 0; i < ringBones.Length; i++)
        {
            NDModelAxesChanger axesChanger = ringBones[i].GetComponent<NDModelAxesChanger>();

            if (axesChanger)
            {
                Transform newBone = axesChanger.HierarchySetup();
                ringBones[i] = newBone;
            }
        }

        for (int i = 0; i < pinkyBones.Length; i++)
        {
            NDModelAxesChanger axesChanger = pinkyBones[i].GetComponent<NDModelAxesChanger>();

            if (axesChanger)
            {
                Transform newBone = axesChanger.HierarchySetup();
                pinkyBones[i] = newBone;
            }
        }

        Debug.Log("Hierarchy generated. Modify IMU_Y and IMU_X rotations and apply to create the gesture prefab");
    }

    /// <summary>
    /// Calling this method, the prefab is stored in the specific folder.
    /// </summary>
    public void CreatePrefab()
    {
#if UNITY_EDITOR
        prefabFile = prefabFolder + gesture + ".prefab";
        PrefabUtility.CreatePrefab(prefabFile, transform.gameObject);

        Debug.Log(gesture + " prefab created in the folder " + prefabFolder);
#endif
    }

    /// <summary>
    /// When the gesture is going to be used as prefab, Quaternions are read from this method
    /// </summary>
    /// <returns></returns>
    public Quaternion[][] GetQuaternions()
    {
        quaternions = new Quaternion[Enum.GetNames(typeof(SensorID)).Length][];
        for (int i = 0; i < quaternions.Length; i++)
        {
            quaternions[i] = new Quaternion[2];
        }

        if (thumbBones != null && indexBones != null && middleBones != null && ringBones != null && pinkyBones != null)
        {
            // Hand
            quaternions[(int)SensorID.Hand][0] = Quaternion.identity;
            quaternions[(int)SensorID.Hand][1] = Quaternion.identity;

            // Thumb 0
            quaternions[(int)SensorID.Thumb0][0] = GetYChild(thumbBones[0]).localRotation;
            quaternions[(int)SensorID.Thumb0][1] = GetXChild(thumbBones[0]).localRotation;

            // Thumb 1
            quaternions[(int)SensorID.Thumb1][0] = GetYChild(thumbBones[1]).localRotation;
            quaternions[(int)SensorID.Thumb1][1] = GetXChild(thumbBones[1]).localRotation;

            // Index 0
            quaternions[(int)SensorID.Index][0] = GetYChild(indexBones[0]).localRotation;
            quaternions[(int)SensorID.Index][1] = GetXChild(indexBones[0]).localRotation;

            // Middle 0
            quaternions[(int)SensorID.Middle][0] = GetYChild(middleBones[0]).localRotation;
            quaternions[(int)SensorID.Middle][1] = GetXChild(middleBones[0]).localRotation;

            // Ring 0
            quaternions[(int)SensorID.Ring][0] = GetYChild(ringBones[0]).localRotation;
            quaternions[(int)SensorID.Ring][1] = GetXChild(ringBones[0]).localRotation;

            // Pinky 0
            quaternions[(int)SensorID.Pinky][0] = GetYChild(pinkyBones[0]).localRotation;
            quaternions[(int)SensorID.Pinky][1] = GetXChild(pinkyBones[0]).localRotation;

            // Chest
            quaternions[(int)SensorID.Chest][0] = Quaternion.identity;
            quaternions[(int)SensorID.Chest][1] = Quaternion.identity;

            // Arm
            quaternions[(int)SensorID.Arm][0] = Quaternion.identity;
            quaternions[(int)SensorID.Arm][1] = Quaternion.identity;

            // Forearm
            quaternions[(int)SensorID.Forearm][0] = Quaternion.identity;
            quaternions[(int)SensorID.Forearm][1] = Quaternion.identity;

            // Thumb 2
            quaternions[(int)SensorID.Thumb2][0] = GetYChild(thumbBones[2]).localRotation;
            quaternions[(int)SensorID.Thumb2][1] = GetXChild(thumbBones[2]).localRotation;

            // Index 1
            quaternions[(int)SensorID.Index1][0] = GetYChild(indexBones[1]).localRotation;
            quaternions[(int)SensorID.Index1][1] = GetXChild(indexBones[1]).localRotation;

            // Index 2
            quaternions[(int)SensorID.Index2][0] = GetYChild(indexBones[2]).localRotation;
            quaternions[(int)SensorID.Index2][1] = GetXChild(indexBones[2]).localRotation;

            // Middle 1
            quaternions[(int)SensorID.Middle1][0] = GetYChild(middleBones[1]).localRotation;
            quaternions[(int)SensorID.Middle1][1] = GetXChild(middleBones[1]).localRotation;

            // Middle 2
            quaternions[(int)SensorID.Middle2][0] = GetYChild(middleBones[2]).localRotation;
            quaternions[(int)SensorID.Middle2][1] = GetXChild(middleBones[2]).localRotation;

            // Ring 1
            quaternions[(int)SensorID.Ring1][0] = GetYChild(ringBones[1]).localRotation;
            quaternions[(int)SensorID.Ring1][1] = GetXChild(ringBones[1]).localRotation;

            // Ring 2
            quaternions[(int)SensorID.Ring2][0] = GetYChild(ringBones[2]).localRotation;
            quaternions[(int)SensorID.Ring2][1] = GetXChild(ringBones[2]).localRotation;

            // Pinky 1
            quaternions[(int)SensorID.Pinky1][0] = GetYChild(pinkyBones[1]).localRotation;
            quaternions[(int)SensorID.Pinky1][1] = GetXChild(pinkyBones[1]).localRotation;

            // Pinky 2
            quaternions[(int)SensorID.Pinky2][0] = GetYChild(pinkyBones[2]).localRotation;
            quaternions[(int)SensorID.Pinky2][1] = GetXChild(pinkyBones[2]).localRotation;
        }
        else
        {
            Debug.LogError("Bones matrix values not assigned");
        }

        return quaternions;
    }

    /// <summary>
    /// Returns the Y component of the Transform (Y child)
    /// </summary>
    /// <param name="tf">Transform to read</param>
    /// <returns>Y-child</returns>
    private Transform GetYChild(Transform tf)
    {
        return tf.GetChild(0);
    }

    /// <summary>
    /// Returns the X component of the Transform (X child)
    /// </summary>
    /// <param name="tf">Transform to read</param>
    /// <returns>X-child</returns>
    private Transform GetXChild(Transform tf)
    {
        return tf.GetChild(0).GetChild(0);
    }
}
