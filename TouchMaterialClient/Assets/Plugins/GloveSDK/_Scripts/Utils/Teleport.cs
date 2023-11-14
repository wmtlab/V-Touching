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

using System.Collections.Generic;
using UnityEngine;
using UnityDLL.Haptic;
using UnityDLL.Contacts;
using NDAPIWrapperSpace;

/// <summary>
/// This class allows the user to teleport objects performing a gesture.
/// When the user is performing the teleport action, a curve will be shown with a marker at the point where the user is aiming.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Teleport : MonoBehaviour
{
    public enum TeleportGestures
    {
        IndexPinch,
        MiddlePinch,
        ThreeFingersPinch,
        Ok,
        Gun,
        PinchFist
    }

    /// <summary>
    /// The gesture that activates the teleport when performed.
    /// </summary>
    public TeleportGestures teleportGesture;
    /// <summary>
    /// Curvature applied to the teleport line.
    /// </summary>
    public AnimationCurve curvature;
    /// <summary>
    /// Curve smoothness.
    /// </summary>
    [Range(0, 1)]
    public float smoothness;
    /// <summary>
    /// Max distance to teleport.
    /// </summary>
    public float teleportDistance;
    /// <summary>
    /// HandModelController that can perform the teleport action.
    /// </summary>
    public HandModelController hmc;
    /// <summary>
    /// The teleport curve origin.
    /// </summary>
    public Transform curveOrigin;
    /// <summary>
    /// A marker that shows where the object will be teleported.
    /// </summary>
    public GameObject destinyMarker;
    /// <summary>
    /// The object that will be teleported.
    /// </summary>
    public GameObject objectToTeleport;
    /// <summary>
    /// Layer used to know where the object can be teleported.
    /// </summary>
    public LayerMask teleportLayer;
    /// <summary>
    /// Layer used to know where the object can not be teleported but the effect is shown.
    /// </summary>
    public LayerMask teleportSupportLayer;
    /// <summary>
    /// This line renderer will be shown when the user is aiming to a point out of the teleport range or the raycast can not detect
    /// an object with the layers supported.
    /// </summary>
    public LineRenderer auxLineRenderer;
    /// <summary>
    /// If this is true, the object teleported will rotate to match its transform.up with the normal of the teleport point surface.
    /// </summary>
    public bool allowRotation;
    /// <summary>
    /// If this is true, the teleported object is placed in the center of the destiny teleport region
    /// </summary>
    public bool forceCenter;
    /// <summary>
    /// Set a teleport event to get notifications of teleport actions to an object surface
    /// </summary>
    public PC_TeleportListener notifyOnTeleportEvent;
    /// <summary>
    /// Allows performing teleport.
    /// </summary>
    public bool enableTeleport;

    /// <summary>
    /// Line start color when the teleport point is valid.
    /// </summary>
    [Header("Curve and marker colors")]
    public Color curveStartColorEnabled = Color.white;
    /// <summary>
    /// Line end color when the teleport point is valid.
    /// </summary>
    public Color curveEndColorEnabled = Color.white;
    /// <summary>
    /// Line start color when the teleport point is not valid.
    /// </summary>
    public Color curveStartColorDisabled = Color.white;
    /// <summary>
    /// Line end color when the teleport point is not valid.
    /// </summary>
    public Color curveEndColorDisabled = Color.white;
    /// <summary>
    /// Marker color when the teleport point is valid.
    /// </summary>
    public Color markerColorEnabled = Color.white;
    /// <summary>
    /// Marker color when the teleport point is not valid.
    /// </summary>
    public Color markerColorDisabled = Color.white;

    /// <summary>
    /// Max number of LineRenderer vertices.
    /// </summary>
    private const int MAX_NUM_OF_VERTICES = 50;
    /// <summary>
    /// Min number of LineRenderer vertices.
    /// </summary>
    private const int MIN_NUM_OF_VERTICES = 2;
    /// <summary>
    /// LineRenderer attached to this object. This line will be the teleport curve visual effect.
    /// </summary>
    private LineRenderer lineRenderer;
    /// <summary>
    /// Curve points.
    /// </summary>
    private List<Vector3> points;
    /// <summary>
    /// LayerMask used for raycast.
    /// </summary>
    private LayerMask raycastLayer;
    /// <summary>
    /// RaycastHit that stores the raycast info.
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// Is the user performing the teleport action?
    /// </summary>
    private bool isPerformingTeleport;
    /// <summary>
    /// Can the user teleport an object to the point aimed?
    /// </summary>
    private bool canTeleport;
    /// <summary>
    /// The teleport point where the player is aiming.
    /// </summary>
    private Vector3 teleportPoint;
    /// <summary>
    /// The normal of the teleport point surface.
    /// </summary>
    private Vector3 teleportNormal;

    /// <summary>
    /// Last teleport object touched by the teleport destiny point
    /// </summary>
    private GameObject destinyObject;

    void Start()
    {
        // Disables the marker
        destinyMarker.SetActive(false);

        // Gets the LineRenderer and disables it
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        // Disables the auxLineRenderer
        auxLineRenderer.enabled = false;

        // The player is not performing teleport when the scene starts
        isPerformingTeleport = false;

        // Gets the raycastLayer. This will ignores all layers except the teleportLayer and teleportSupportLayer
        raycastLayer = teleportLayer | teleportSupportLayer;
    }

    void Update()
    {
        // If the HandModelController is not null and it has a device assigned, allows performing the teleport action
        if (hmc != null && hmc.device != null && enableTeleport)
        {
            // Initializes points List
            points = new List<Vector3>();
            // Sets this object position as the curveOrigin position
            transform.position = curveOrigin.position;

            bool isPerformingGesture = false;

            // Checks if the player is performing the correct gesture
            switch (teleportGesture)
            {
                case (TeleportGestures.IndexPinch):
                    if (ContactsSystem.AreContactsJoined(Contact.CONT_INDEX, Contact.CONT_THUMB, hmc.handLocation, hmc.user) &&
                        !ContactsSystem.AreContactsJoined(Contact.CONT_MIDDLE, Contact.CONT_ANY, hmc.handLocation, hmc.user))
                    {
                        isPerformingGesture = true;
                    }
                    break;
                case (TeleportGestures.MiddlePinch):
                    if (ContactsSystem.AreContactsJoined(Contact.CONT_MIDDLE, Contact.CONT_THUMB, hmc.handLocation, hmc.user) &&
                        !ContactsSystem.AreContactsJoined(Contact.CONT_INDEX, Contact.CONT_ANY, hmc.handLocation, hmc.user))
                    {
                        isPerformingGesture = true;
                    }
                    break;
                case (TeleportGestures.ThreeFingersPinch):
                    if (ContactsSystem.AreContactsJoined(Contact.CONT_MIDDLE, Contact.CONT_THUMB, hmc.handLocation, hmc.user) &&
                        ContactsSystem.AreContactsJoined(Contact.CONT_INDEX, Contact.CONT_THUMB, hmc.handLocation, hmc.user))
                    {
                        isPerformingGesture = true;
                    }
                    break;
                case (TeleportGestures.Ok):
                    if (ContactsSystem.AreContactsJoined(Contact.CONT_INDEX, Contact.CONT_PALM, hmc.handLocation, hmc.user) &&
                        ContactsSystem.AreContactsJoined(Contact.CONT_MIDDLE, Contact.CONT_PALM, hmc.handLocation, hmc.user))
                    {
                        isPerformingGesture = true;
                    }
                    break;
                case (TeleportGestures.Gun):
                    if (ContactsSystem.AreContactsJoined(Contact.CONT_MIDDLE, Contact.CONT_PALM, hmc.handLocation, hmc.user) &&
                        !ContactsSystem.AreContactsJoined(Contact.CONT_INDEX, Contact.CONT_ANY, hmc.handLocation, hmc.user))
                    {
                        isPerformingGesture = true;
                    }
                    break;
                case (TeleportGestures.PinchFist):
                    if (ContactsSystem.AreContactsJoined(Contact.CONT_INDEX, Contact.CONT_THUMB, hmc.handLocation, hmc.user) &&
                        ContactsSystem.AreContactsJoined(Contact.CONT_MIDDLE, Contact.CONT_PALM, hmc.handLocation, hmc.user))
                    {
                        isPerformingGesture = true;
                    }
                    break;
            }

            // If the user is performing the teleportGesture, check if teleport is allowed
            if (isPerformingGesture)
            {
                // If the user wasn't teleporting in the last frame, plays a pulse to give haptic feedback and sets isPerformingTeleport to true
                if (!isPerformingTeleport)
                {
                    HapticSystem.PlayPulse(0.2f, 100, hmc.handLocation, hmc.user);
                    isPerformingTeleport = true;
                }

                // Check if the player is aiming a point in range and the layer is the correct one
                if (Physics.Raycast(transform.position, curveOrigin.forward, out hit, teleportDistance, raycastLayer))
                {
                    // Sets the vertex count by default as the minimum
                    int vertexCount = MIN_NUM_OF_VERTICES;
                    // Gets the teleport direction
                    Vector3 direction = hit.point - transform.position;

                    // If the marker is not null, interpolates the teleport point to makes the visual effect smoother
                    if (destinyMarker != null)
                    {
                        // If the marker wasn't active in the last frame sets the teleport point as the RaycastHit point.
                        // Otherwise, interpolates between the last point and the new one
                        if (!destinyMarker.activeSelf)
                        {
                            // Enables the marker
                            destinyMarker.SetActive(true);
                            // Sets the teleport point as the RaycastHit point
                            teleportPoint = hit.point;
                            // Rotates this object to look at the teleport point
                            transform.rotation = Quaternion.LookRotation(direction);
                        }
                        else
                        {
                            // Interpolates between the last teleport point and the new one
                            teleportPoint = Vector3.Lerp(teleportPoint, hit.point, 15f * Time.deltaTime);
                            // Interpolates between this object last rotation and the new one
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 15f * Time.deltaTime);
                        }

                        // Sets the marker rotation and position
                        destinyMarker.transform.up = hit.normal;
                        destinyMarker.transform.position = teleportPoint;
                    }
                    else
                    {
                        // Sets the teleport point as the RaycastHit point
                        teleportPoint = hit.point;
                        // Rotates this object to look at the teleport point
                        transform.rotation = Quaternion.LookRotation(direction);
                    }

                    // Gets the normal of the teleport point surface
                    teleportNormal = hit.normal;

                    // If the object hit has the teleportLayer assigned, the user can teleport
                    if (LayerMask.GetMask(LayerMask.LayerToName(hit.collider.gameObject.layer)) == teleportLayer.value)
                    {
                        //Sets the last teleport destiny touched
                        destinyObject = hit.collider.gameObject;

                        // Sets the start and end colors of the curve
                        lineRenderer.startColor = curveStartColorEnabled;
                        lineRenderer.endColor = curveEndColorEnabled;

                        // If the marker is not null, changes its color
                        if (destinyMarker != null)
                            destinyMarker.GetComponent<Renderer>().material.color = markerColorEnabled;

                        // Sets canTeleport to true
                        canTeleport = true;
                    }
                    else
                    {
                        // Sets the start and end colors of the curve
                        lineRenderer.startColor = curveStartColorDisabled;
                        lineRenderer.endColor = curveEndColorDisabled;

                        // If the marker is not null, changes its color
                        if (destinyMarker != null)
                            destinyMarker.GetComponent<Renderer>().material.color = markerColorDisabled;

                        // Sets canTeleport to false
                        canTeleport = false;
                    }

                    // Gets the vertex count depending on smoothness value
                    vertexCount = (int)Mathf.Clamp(MAX_NUM_OF_VERTICES * smoothness, 2, MAX_NUM_OF_VERTICES);

                    // Draws the curve
                    for (float i = 0f; i <= 1f; i += 1f / vertexCount)
                    {
                        Vector3 point = new Vector3(0f, curvature.Evaluate(i), i * direction.magnitude);
                        points.Add(point);
                    }

                    // Sets the lineRenderer new points
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPositions(points.ToArray());

                    // Enables the lineRenderer
                    lineRenderer.enabled = true;

                    // Disables the auxLineRenderer
                    auxLineRenderer.enabled = false;
                }
                else
                {
                    // Disables the lineRenderer
                    lineRenderer.positionCount = 0;
                    lineRenderer.enabled = false;

                    // Enables the auxLineRenderer
                    Vector3 aux = curveOrigin.forward * teleportDistance;
                    transform.rotation = Quaternion.LookRotation(aux);
                    auxLineRenderer.enabled = true;

                    // Sets canTeleport to false
                    canTeleport = false;
                    // If the marker is not null, disables it
                    if (destinyMarker != null)
                        destinyMarker.SetActive(false);
                }
            }
            else
            {
                // Disables the lineRenderer
                lineRenderer.positionCount = 0;
                lineRenderer.enabled = false;

                // Disables the auxLineRenderer
                auxLineRenderer.enabled = false;

                // If the marker is not null, disables it
                if (destinyMarker != null)
                    destinyMarker.SetActive(false);

                // If the user was performing the teleport action and it is allowed, teleports the object
                if (isPerformingTeleport && canTeleport)
                {
                    // Sets the objectToTeleport new position
                    if (forceCenter)
                        objectToTeleport.transform.position = destinyObject.transform.position;
                    else
                        objectToTeleport.transform.position = teleportPoint;

                    // Notify we telerported to this object surface
                    if (notifyOnTeleportEvent != null) notifyOnTeleportEvent.TeleportToObject(destinyObject);

                    // If allowRotation is true, matches the objectToTeleport transform.up with the normal of the teleport point surface
                    if (allowRotation)
                        objectToTeleport.transform.up = teleportNormal;

                    // Sets canTeleport to false
                    canTeleport = false;
                }

                // Sets isPerformingTeleport to false
                isPerformingTeleport = false;
            }
        }
    }
}
