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
using UnityDLL.Motion;

/// <summary>
/// This class controls the behaviour of the finger. It gets phalanges and applies rotations to them.
/// </summary>
public class FingerModelController : MonoBehaviour
{
    /// <summary>
    /// Controls finger rotation
    /// </summary>
    FingerMotionSensor fingerMotionSensor;

    /// <summary>
    /// This property gives information about finger and its axes
    /// </summary>
    public NDModelAxesChanger AxesChanger { get; set; }

    /// <summary>
    /// Information about phalanges
    /// </summary>
    NDModelAxesChanger[] phalanges;

    /// <summary>
    /// Phalanges bones
    /// </summary>
    private Transform[] bones;

    /// <summary>
    /// Determines if this finger is thumb or not, in order to control phalanges rotations
    /// </summary>
    private bool isThumb;

    /// <summary>
    /// When the X rotation of the finger reachs this degree, it turns its Y rotation to 0
    /// </summary>
    public float degreesToTurnYToZero;

    /// <summary>
    /// If this property is checked, fingers can be closed
    /// </summary>
    public bool enablePhalangesClosingSimulation;

    /// <summary>
    /// Locks the X component
    /// </summary>
    public bool lockX;

    /// <summary>
    /// Locks the Y component
    /// </summary>
    public bool lockY;

    /// <summary>
    /// When the proximal phalanx reaches this degree, the intermediate phalanx closes
    /// </summary>
    public float desiredDegreesForP1OnClosing;
    /// <summary>
    /// AnimationCurve to determine how to close the intermediate phalanx
    /// </summary>
    public AnimationCurve animCurveCloseSensitivityP1;
    /// <summary>
    /// Degree of X component of the intermediate phalanx where it finishes the previous value
    /// </summary>
    public float degreesToTurnP1ToPreviousValue;

    /// <summary>
    /// When the proximal phalanx reaches this degree, the intermediate phalanx closes
    /// </summary>
    public float desiredDegreesForP2OnClosing;
    /// <summary>
    /// AnimationCurve to determine how to close the intermediate phalanx
    /// </summary>
    public AnimationCurve animCurveCloseSensitivityP2;
    /// <summary>
    /// Degree of X component of the intermediate phalanx where it finishes the previous value
    /// </summary>
    public float degreesToTurnP2ToPreviousValue;

    /// <summary>
    /// Max X degrees allowed
    /// </summary>
    public float maxXDegrees;
    /// <summary>
    /// Min X degrees allowed
    /// </summary>
    public float minXDegrees;
    /// <summary>
    /// Max Y degrees allowed
    /// </summary>
    public float maxYDegrees;
    /// <summary>
    /// Min Y degrees allowed
    /// </summary>
    public float minYDegrees;

    public bool applyLimits;

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Retrieves information about phalanges and bones
    /// </summary>
    private void Initialize()
    {
        AxesChanger = GetComponent<NDModelAxesChanger>();

        if (AxesChanger.part == NDModelAxesChanger.Parts.thumb0)
            isThumb = true;

        phalanges = transform.GetComponentsInChildren<NDModelAxesChanger>();
        if (phalanges.Length != 0)
        {
            bones = new Transform[phalanges.Length];
            foreach (NDModelAxesChanger phalanx in phalanges)
            {
                bones[(int)phalanx.boneIndex] = phalanx.GetDataTrasform();
            }
        }
    }

    /// <summary>
    /// Avatar VR method. Given two quaternions (proximal and distal phalanges), it rotates finger along X-axis.
    /// For index, middle, ring and pinky fingers, it is only necessary the proximal one.
    /// </summary>
    /// <param name="quatProximal">Proximal phalanx quaternion</param>
    /// <param name="quatDistal">Distal phalanx quaternion. It is necessary for thumbs. Quaternion.identity for the rest of the fingers</param>
    public Quaternion[][] RotateBone(Quaternion quatProximal, Quaternion quatDistal)
    {
        Quaternion[][] fingersRotations = fingerMotionSensor.RotateBonesWithoutRestrictions(quatProximal, quatDistal);

        if (isThumb)
        {
            //Limit X+ rotation of Thumb1
            if (fingersRotations[2][1].x < 0)
            {
                fingersRotations[2][1] = Quaternion.identity;
            }
            fingersRotations[2][0] = Quaternion.identity;
        }

        if (!isThumb)
            if (enablePhalangesClosingSimulation)
            {
                fingerMotionSensor.HandClosingSimulation(fingersRotations);
            }

        return fingersRotations;
    }

    /// <summary>
    /// Gloveone method. Finger closing is measured from 0 to 1.
    /// Interpolating this value, phalanges are going to be closed or opened
    /// </summary>
    /// <param name="fingerFlexRotation">Value from 0 to 1 read from finger sensor</param>
    /// <returns>Quaternion[i][j] where are going to be stored rotations of each finger, where "i" represents the phalanx,
    /// and "j" the component (Y or X)</returns>
    public Quaternion[][] RotateBone(float fingerFlexRotation)
    {
        return fingerMotionSensor.RotateBones(fingerFlexRotation);
    }

    /// <summary>
    /// Recalibrates the model using the current values from sensors
    /// </summary>
    /// <param name="quatProximal">Proximal phalanx quaternion</param>
    /// <param name="quatDistal">Distal phalanx quaternion. It is necessary for thumbs. Quaternion.identity for the rest of the fingers</param>
    public void Recalibrate(Quaternion quatProximal, Quaternion quatDistal)
    {
        fingerMotionSensor.SetLimits(minXDegrees, maxXDegrees, minYDegrees, maxYDegrees, degreesToTurnYToZero);
        fingerMotionSensor.Calibrate(quatProximal, quatDistal);

    }

    /// <summary>
    /// Given the Y angle, establishes a new calibration pose
    /// </summary>
    /// <param name="newYRotation">Global Y angle from the starting pose</param>
    public void ResetCalibrationPose(Vector3 newRotation)
    {
        fingerMotionSensor.UpdateCalibrationPose(newRotation);
    }

    /// <summary>
    /// Creates the necessary FingerMotionSensor, and assign properties to it
    /// </summary>
    /// <returns>Array of Transforms where are stored the phalanges of the finger</returns>
    public Transform[] CreateFingerMotionSensor(MotionSensor handMotionSensor)
    {
        if (bones.Length != 0)
        {
            if (AxesChanger.part == NDModelAxesChanger.Parts.thumb0)
                fingerMotionSensor = new FingerMotionSensor(true);
            else
                fingerMotionSensor = new FingerMotionSensor();

            fingerMotionSensor.SetHandClosingSimulationParameters(desiredDegreesForP1OnClosing, degreesToTurnP1ToPreviousValue, desiredDegreesForP2OnClosing, degreesToTurnP2ToPreviousValue, degreesToTurnYToZero, animCurveCloseSensitivityP1, animCurveCloseSensitivityP2, lockX, lockY);

            fingerMotionSensor.applyLimits = applyLimits;
            fingerMotionSensor.SetHandMotionSensor(handMotionSensor);

            // Set bones to FingerMotionSensor
            fingerMotionSensor.SetBones(bones);

            fingerMotionSensor.SetCalibrationPose(bones[0].rotation, bones[1].rotation, bones[2].rotation);

            return bones;
        }
        else
            return null;
    }
}
