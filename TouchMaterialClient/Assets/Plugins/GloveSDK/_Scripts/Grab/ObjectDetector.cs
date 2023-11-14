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

using System.Collections;
using UnityEngine;

/// <summary>
/// Object detector class. This script has to be placed in the hand GameObject. If you want to grab an object in a natural way, this script allows you to perform that.
/// </summary>
public class ObjectDetector : MonoBehaviour
{
    /// <summary>
    /// Center of the palm where we are going to set the center of the object
    /// </summary>
    public Transform palmCenter;
    /// <summary>
    /// Radius of detection
    /// </summary>
    public float radius;
    /// <summary>
    /// Colliders that are in contact with the hand
    /// </summary>
    private Collider[] cols;
    /// <summary>
    /// Collider layer that is going to be checked
    /// </summary>
    public LayerMask colliderLayer;
    private bool thumb, ring;
    /// <summary>
    /// Object that has been catched through this script
    /// </summary>
    private GameObject objectCatched;
    /// <summary>
    /// When the object is released, applies this impulse.
    /// </summary>
    public float impulse;
    /// <summary>
    /// Time (in seconds) that colliders are disabled after the object has been released
    /// </summary>
    public float timeDisabled;
    private HandModelController hmc;
    private ObjectMovement objMov;

    void Start()
    {
        hmc = GetComponentInParent<HandModelController>();
    }

    /// <summary>
    /// This sets ring and thumb values, used to know if the fingers are in contact with any object
    /// </summary>
    /// <param name="The finger name"></param>
    /// <param name="The value to set. True or false"></param>
    public void SetFinger(string name, bool value)
    {
        if (name == "thumb")
            thumb = value;
        else
            ring = value;
    }

    void FixedUpdate()
    {
        // Gets the colliders in contact with the palm
        cols = Physics.OverlapSphere(palmCenter.position, radius, colliderLayer.value);

        // If there is any collider in range, checks if fingers are in contact with the collider too
        if (cols.Length >= 1)
        {
            // Gets the objectCatched
            objectCatched = cols[0].gameObject;
            if (objectCatched.GetComponent<ObjectMovement>() == null)
            {
                // Adds ObjectMovement to calculate the object launching direction and velocity
                objMov = objectCatched.AddComponent<ObjectMovement>();
            }

            // If the thum and the ring are in contact with the object, perform the grabbing action
            if (thumb && ring)
            {
                objectCatched.transform.SetParent(palmCenter);
                objectCatched.GetComponent<Rigidbody>().isKinematic = true;
                objectCatched.transform.localPosition = Vector3.Lerp(objectCatched.transform.localPosition, new Vector3(0f, 0f, -cols[0].bounds.extents.z), Time.deltaTime * 5f);
            }
            else
            {
                // Allows the object using gravity again
                objectCatched.GetComponent<Rigidbody>().isKinematic = false;
                objectCatched.GetComponent<Rigidbody>().useGravity = true;
                objectCatched.transform.SetParent(null);

                if (objMov != null)
                {
                    // Disables the colliders to launch the object correctly
                    StartCoroutine(DisableEnableColliders());
                    // Launchs the object
                    objectCatched.GetComponent<Rigidbody>().AddForce(impulse * objMov.objectSmoothVelocity, ForceMode.Impulse);
                    // Destroys the ObjectMovement script attached to the object to stop calculating the speed
                    Destroy(objMov);
                }
                // Sets objectCatched to null
                objectCatched = null;
            }
        }
    }

    /// <summary>
    /// Disables the handColliders for a moment and enables them again to allow the user perform the object launching correctly
    /// </summary>
    /// <returns></returns>
    IEnumerator DisableEnableColliders()
    {
        // Gets the hand colliders
        Collider[] handCols = hmc.GetComponentsInChildren<Collider>();
        // Disables the colliders
        foreach (Collider col in handCols)
        {
            if (LayerMask.GetMask(LayerMask.LayerToName(col.gameObject.layer)) != colliderLayer.value)
            {
                col.enabled = false;
            }
        }
        yield return new WaitForSecondsRealtime(timeDisabled);
        // Enables them again
        foreach (Collider col in handCols)
        {
            if (LayerMask.GetMask(LayerMask.LayerToName(col.gameObject.layer)) != colliderLayer.value)
            {
                col.enabled = true;
            }
        }
    }
}