/******************************************************************************
* Copyright © NeuroDigital Technologies, S.L. 2018                            *
* Licensed under the Apache License, Version 2.0 (the "License");             *
* you may not use this file except in compliance with the License.            *
* You may obtain a copy of the License at                                     *
* http://www.apache.org/licenses/LICENSE-2.0                                  *
* Unless required by applicable law or agreed to in writing, software         *
* distributed under the License is distributed on an "AS IS" BASIS,           *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.    *
* See the License for the specific language governing permissions and         *
* limitations under the License.                                              *
*******************************************************************************/

using UnityEngine;
using System.Collections;
using NDAPIWrapperSpace;
using UnityDLL.Contacts;

/// <summary>
/// This class allows to grab objects performing the PinchGesture and MiddlePinchGesture.
/// The object must have a trigger collider, rigid body and the layer specified in the property.
/// </summary>
public class GrabThings : MonoBehaviour
{
    /// <summary>
    /// Collider layer that is going to be checked.
    /// </summary>
    public LayerMask colliderLayer;
    /// <summary>
    /// The time in seconds the hand colliders will be disabled to launch objects.
    /// </summary>
    public float timeDisabled;
    /// <summary>
    /// Factor applied in launching.
    /// </summary>
    public float impulse;
    /// <summary>
    /// Position where the grabbed object is going to be.
    /// </summary>
    private Transform grabZone;
    private GameObject objectCatched;
    private GameObject hand;
    private Collider[] cols;
    private ActuatorInfo finger;
    /// <summary>
    /// The component used to get the object acceleration
    /// </summary>
    private ObjectMovement objMov;

    // Use this for initialization
    void Start()
    {
        objectCatched = this.gameObject;
        finger = GetComponentInParent<ActuatorInfo>();
        grabZone = this.transform;

        hand = GetComponentInParent<HandModelController>().gameObject;
    }

    /// <summary>
    /// Checks every frame if the glove is performing the Pinch gesture (Thumb and index) or MiddlePinch gesture (middle finger and thumb), and if
    /// it is present some object with the specified tag near these fingers.
    /// </summary>
    void Update()
    {
        if (finger == null)
            return;

        // If this finger is the index and the user is performing the pinch gesture or this finger is the middle and the user is performing the pinch gesture,
        // try to grab objects. Otherwise, calls the UnCatched method and resets values
        if (finger.actuator == Actuator.ACT_INDEX && ContactsSystem.AreContactsJoined(Contact.CONT_THUMB, Contact.CONT_INDEX, finger.location, finger.userIndex) ||
            finger.actuator == Actuator.ACT_MIDDLE && ContactsSystem.AreContactsJoined(Contact.CONT_THUMB, Contact.CONT_MIDDLE, finger.location, finger.userIndex))
        {
            if (objectCatched == this.gameObject)
            {
                // Gets the colliders with the specified layer in contact with the grabZone
                cols = Physics.OverlapSphere(grabZone.transform.position, 0.1f, colliderLayer.value);
                if (cols.Length > 0)
                {
                    for (int i = 0; i < cols.Length; i++)
                    {
                        if ((objectCatched == this.gameObject))
                        {
                            // Catchs the object
                            Catched(cols[i].gameObject);
                            break;
                        }
                    }
                }
            }
            else
            {
                // Lerp the objectCatched position with the grabZone
                objectCatched.transform.position = Vector3.Lerp(objectCatched.transform.position, grabZone.position, Time.deltaTime * 5);
            }
        }
        else
        {
            if (objectCatched != this.gameObject)
            {
                UnCatched();
            }
        }
    }

    /// <summary>
    /// Once the object is catched, sets is as the object catched and sets this as its parent.
    /// </summary>
    /// <param name="objectToCach">Object to cach.</param>
    public void Catched(GameObject objectToCach)
    {
        objectCatched = objectToCach;
        objectCatched.GetComponent<Rigidbody>().isKinematic = true;
        objectCatched.transform.SetParent(this.transform);
        if (objMov == null)
            // Adds ObjectMovement to calculate the object launching direction and velocity
            objMov = objectCatched.AddComponent<ObjectMovement>();
    }

    /// <summary>
    /// Once we stop grabbing the object, launchs it and resets the values.
    /// </summary>
    public void UnCatched()
    {
        if (objectCatched != this.gameObject)
        {
            objectCatched.GetComponent<Rigidbody>().isKinematic = false;
            objectCatched.transform.parent = null;
            if (finger)
                StartCoroutine(DisableEnableColliders());
            if (objMov != null)
                objectCatched.GetComponent<Rigidbody>().AddForce(impulse * objMov.objectSmoothVelocity, ForceMode.Impulse);
            Destroy(objMov);
            objectCatched = this.gameObject;
        }
    }

    /// <summary>
    /// Disables the handColliders for a moment and enables them again to allow the user perform the object launching correctly
    /// </summary>
    /// <returns></returns>
    IEnumerator DisableEnableColliders()
    {
        // Gets the hand colliders
        Collider[] handCols = hand.GetComponentsInChildren<Collider>();
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
