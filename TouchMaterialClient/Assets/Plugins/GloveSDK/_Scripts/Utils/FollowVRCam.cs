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
/// Translate the avatar according to the position of the object to track. This script has been made in order to
/// use it with VR headsets.
/// </summary>
public class FollowVRCam : MonoBehaviour
{
    /// <summary>
    /// Checks if the avatar is ready to move following the camera
    /// </summary>
    public bool readyToMove;
    /// <summary>
    /// This Transform is the object to follow
    /// </summary>
    public Transform pointToFollow;
    /// <summary>
    /// Local offset to apply
    /// </summary>
    public Vector3 offset;

    private float filterPosition = 100f;
    private Vector3 targetPosition;
    private bool isTB3Active;
    private bool isReady;
    private HandModelController[] hmcs;

    private Quaternion quatTrackY;
    private Quaternion initialRotationOffset;

    private void Awake()
    {
        isReady = false;

        hmcs = GameObject.FindObjectsOfType<HandModelController>();

        initialRotationOffset = Quaternion.Inverse(pointToFollow.transform.rotation) * transform.rotation;
    }

    /// <summary>
    /// Determine if the device has a TrackBand3 plugged or not
    /// </summary>
    private void GetTBFromHMC()
    {
        if (hmcs != null)
        {
            for (int i = 0; i < hmcs.Length; i++)
                if (hmcs[i].device != null)
                {
                    isReady = true;
                    if (hmcs[i].device.trackband == NDDevice.Trackband.TB3)
                    {
                        isTB3Active = true;
                        break;
                    }
                }
        }
    }

    void Update()
    {
        if (!isReady)
        {
            GetTBFromHMC();
            return;
        }

        if (!readyToMove)
            return;

        if (!isTB3Active)
        {
            // Position
            this.transform.position = pointToFollow.position + offset;

            //// Rotation
            quatTrackY = pointToFollow.rotation * initialRotationOffset;
            quatTrackY = new Quaternion(0, quatTrackY.y, 0, quatTrackY.w);
            transform.rotation = quatTrackY;
        }
        else
        {
            // Position
            targetPosition = this.transform.position + ((pointToFollow.position - transform.position) + offset);
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, Time.deltaTime * filterPosition);
        }
    }
}
