/********************************************************************************
* Copyright © NeuroDigital Technologies, S.L. 2018								*
* Licensed under the Apache License, Version 2.0 (the "License");				*
* you may not use this file except in compliance with the License.				*
* You may obtain a copy of the License at										*
* http://www.apache.org/licenses/LICENSE-2.0									*
* Unless required by applicable law or agreed to in writing, software			*
* distributed under the License is distributed on an "AS IS" BASIS,				*
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.		*
* See the License for the specific language governing permissions and			*
* limitations under the License.												*
********************************************************************************/

using UnityEngine;

/// <summary>
/// Object detector class. This script has to be placed in the Thumb and Ring GameObjects. If you want to grab an object in a natural way, this script allows you to perform that.
/// </summary>
public class ObjectDetectorFinger : MonoBehaviour
{
    /// <summary>
    /// Radius of detection
    /// </summary>
    public float radius;
    /// <summary>
    /// Colliders that are in contact with the attached collider
    /// </summary>
    private Collider[] cols;
    /// <summary>
    /// Collider layer that is going to be checked
    /// </summary>
    public LayerMask colliderLayer;
    private ObjectDetector od;
    private string finger;

    void Start()
    {
        // Gets the ObjectDetector component
        od = GetComponentInParent<ObjectDetector>();

        // Sets the finger value depending on this object name
        if (transform.name.Contains("ring"))
        {
            finger = "ring";
        }
        else
            finger = "thumb";
    }

    void FixedUpdate()
    {
        // Gets all the colliders with the specified layer in range
        cols = Physics.OverlapSphere(transform.position, radius, colliderLayer.value);

        // If there is any object in range, sets this finger value to true. Otherwise, sets it to false
        if (cols.Length >= 1)
        {
            od.SetFinger(finger, true);
        }
        else
            od.SetFinger(finger, false);
    }

    /// <summary>
    /// Shows a wire sphere that represents the object detection area
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}