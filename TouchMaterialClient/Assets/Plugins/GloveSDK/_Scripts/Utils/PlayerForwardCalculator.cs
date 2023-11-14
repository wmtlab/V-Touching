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

/// <summary>
/// Determines the orientation of the body, needed to recalibrate the device
/// </summary>
public class PlayerForwardCalculator : MonoBehaviour
{
    /// <summary>
    /// Angle (in degrees) of the hand forward and the global Z+
    /// </summary>
    public Vector3 worldAngle;
    private Vector3 diff;
    /// <summary>
    /// If true; the calibration stays the current orientation of the hand, and just calibrates the fingers
    /// </summary>
    public bool forceZeroX = false;

    private Vector3 previousWorldAngle;

    /// <summary>
    /// Calculates the Vector3 of the new orientation
    /// </summary>
    /// <param name="acceleration">Hand acceleration. With this parameter, it is possible to know where is the palm facing</param>
    /// <param name="isRight">True if it is right; False if it is left</param>
    /// <param name="maxZRotation">Maximum Z-axis rotation</param>
    /// <returns>Orientation for the new recalibration</returns>
    public Vector3 GetWorldAngle(Vector3 acceleration, bool isRight, float maxZRotation = 180)
    {
        diff = transform.TransformPoint(Vector3.forward * 1000f).normalized;
        if (forceZeroX)
        {
            diff = new Vector3(diff.x, 0, diff.z);
            worldAngle = Quaternion.LookRotation(diff.normalized, Vector3.up).eulerAngles;
        }
        else
        {
            diff = new Vector3(diff.x, 0, diff.z);
            Vector3 tempWorldAngle = Quaternion.LookRotation(diff.normalized, Vector3.up).eulerAngles;

            Quaternion quatFinal;
            Quaternion quatHandY = new Quaternion();
            quatHandY.eulerAngles = tempWorldAngle;
            Quaternion quatHandLocalZ = new Quaternion();
            quatHandLocalZ.eulerAngles = new Vector3(0, 0, Mathf.Lerp(isRight ? -maxZRotation : maxZRotation, 0, Mathf.Clamp((1 - acceleration.y) * 0.5f, -1, 1)));
            quatFinal = quatHandY * quatHandLocalZ;

            worldAngle = quatFinal.eulerAngles;

        }

        if (Mathf.Abs(previousWorldAngle.y - worldAngle.y) <= 0.5f)
        {
            worldAngle = previousWorldAngle;
        }

        previousWorldAngle = worldAngle;

        return worldAngle;
    }

    /// <summary>
    /// Calculates the new orientation without any condition
    /// </summary>
    /// <returns>Orientation of the new calibration just for one bone</returns>
    public Vector3 GetWorldAngle()
    {
        diff = transform.TransformPoint(Vector3.forward * 1000f).normalized;
        diff = new Vector3(diff.x, 0, diff.z);
        worldAngle = Quaternion.LookRotation(diff.normalized, Vector3.up).eulerAngles;
        return worldAngle;
    }
}
