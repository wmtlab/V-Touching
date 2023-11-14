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
/// Controls model upper parts. This class has references to bone and NDModelAxesChanger of the model.
/// </summary>
public class UpperModelController
{
    /// <summary>
    /// This property handles the rotation of the bone
    /// </summary>
    public MotionSensor motionSensor;
    /// <summary>
    /// Through this property, we can access to the correct orientation of the bone
    /// </summary>
    public NDModelAxesChanger AxesChanger { get; set; }
    /// <summary>
    /// Bone Transform
    /// </summary>
    public Transform bone;

    /// <summary>
    /// Initializes properties
    /// </summary>
    /// <param name="bone">Bone that is going to be controlled</param>
    /// <returns>True if it has been created successfully; false otherwise</returns>
    public bool CreateMotionSensor(Transform bone)
    {
        if (bone)
        {
            this.bone = bone;

            AxesChanger = bone.GetComponentInChildren<NDModelAxesChanger>();

            motionSensor = new MotionSensor();
            motionSensor.SetBone(bone.gameObject);

            motionSensor.SetCalibrationPose(bone.rotation, AxesChanger.GetAlignerRotation());

            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Sets the new calibration pose depending on the hand/body orientation
    /// </summary>
    /// <param name="newYRotation">Rotation (in Euler angles) from the starting orientation to the current one</param>
    public void ResetCalibrationPose(Vector3 newRotation)
    {
        motionSensor.UpdateCalibrationPose(newRotation);
    }

    /// <summary>
    /// If TB3 is being used with a left AvatarVR, it is needed to call this method in order
    /// to change the chest orientation
    /// </summary>
    public void UpdateAlignerRotation()
    {
        AxesChanger.CorrectLeftChestAligner();

        motionSensor.SetCalibrationPose(bone.rotation, AxesChanger.GetAlignerRotation());
    }

    /// <summary>
    /// Calibrates the bone according to the initial rotation
    /// </summary>
    /// <param name="qSensor">Current rotation, needed to get the inverse</param>
    public void Calibrate(Quaternion qSensor)
    {
        motionSensor.Calibrate(qSensor);
    }

    /// <summary>
    /// Given the raw rotation of the sensor, perform the rotation of the bone
    /// </summary>
    /// <param name="rawRotation">Sensor raw rotation</param>
    /// <returns>Y and X components of the rotation to apply</returns>
    public Quaternion[] RotateBone(Quaternion rawRotation)
    {
        return motionSensor.RotateBone(rawRotation);
    }


    public Quaternion[] RotateBoneChest(Quaternion rawRotation)
    {
        return motionSensor.RotateBone(rawRotation);
    }
}