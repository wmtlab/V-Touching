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

// <summary>
// Get the position of the fingers over the object and detects if it is suitable to perform the grabbing action
// </summary>
public class GrabbingPoseTest : MonoBehaviour
{
    // The grab pose detector
    GrabbingPoseDetector poseDetector;
    // Center of the palm where we are going to set the center of the object
    public Transform palmCenter;
    // Colliders that are in contact with the hand
    private Collider[] cols;
    // Collider layer that is going to be checked
    public LayerMask colliderLayer;
    // Object that has been catched through this script
    private GameObject objectCatched;
    // When the object is released, applies this impulse
    public float impulse;
    // Time (in seconds) that colliders are disabled after the object has been released
    public float timeDisabled;

    private ObjectMovement objMov;

    void Start()
    {
        // Gets the GrabbingPoseDetector component
        poseDetector = GetComponent<GrabbingPoseDetector>();
    }

    void FixedUpdate()
    {
        // Gets all the colliders in contact with the hand
        cols = Physics.OverlapSphere(palmCenter.position, 0.04f, colliderLayer.value);

        if (cols.Length >= 1)
        {
            // Gets the objectCatched
            objectCatched = cols[0].gameObject;


            if (poseDetector.HandIsGrabbing)
            {
                if (objectCatched.GetComponent<ObjectMovement>() == null)
                {
                    // Adds ObjectMovement to calculate the object launching direction and velocity
                    objMov = objectCatched.AddComponent<ObjectMovement>();
                }

                // Performs the grabbing action
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

    // Disables the handColliders for a moment and enables them again to allow the user perform the object launching correctly
    IEnumerator DisableEnableColliders()
    {
        // Gets the hand colliders
        Collider[] handCols = poseDetector.GetComponentsInChildren<Collider>();
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
