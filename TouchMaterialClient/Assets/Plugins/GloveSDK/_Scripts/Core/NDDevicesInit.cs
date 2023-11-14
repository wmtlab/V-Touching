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

using System;
using System.Collections.Generic;
using NDAPIUnity;
using NDAPIWrapperSpace;
using UnityDLL.Core;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NDDevicesInit is the main script of the SDK. This script establishes relations between physycal devices and hands in scene.
/// It has to be attached in the root of the Torso.
/// </summary>
public class NDDevicesInit : MonoBehaviour
{
    private static NDDevicesInit instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.Init();
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Inner class to keep control of HMC and assignments
    /// </summary>
    public class Device
    {
        public HandModelController hmc;
        public bool isAssigned;
        public int deviceId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hmc">Attached HandModelController</param>
        /// <param name="isAssigned">True if this device is assigned</param>
        /// <param name="deviceId">Device ID</param>
        public Device(HandModelController hmc, bool isAssigned, int deviceId)
        {
            this.hmc = hmc;
            this.isAssigned = isAssigned;
            this.deviceId = deviceId;
        }

        /// <summary>
        /// Given a deviceID, create a NDDevice and assign it to that device
        /// </summary>
        /// <param name="deviceId"></param>
        public void AssignGlove(int deviceId)
        {
            this.isAssigned = true;
            this.deviceId = deviceId;
            NDDevice ndDevice = NDService.Instance.CreateDevice(deviceId, NDDevicesInit.currentUser);
            if (hmc != null)
                this.hmc.StartGlove(ndDevice);
        }

        /// <summary>
        /// Set the HandModelController given by parameter
        /// </summary>
        /// <param name="param">HandModelController to set</param>
        public void SetHMC(HandModelController param)
        {
            hmc = param;
        }

        /// <summary>
        /// Set the isAssigned variable
        /// </summary>
        /// <param name="param">True if this device is assigned</param>
        public void SetIsAssigned(bool param)
        {
            isAssigned = param;
        }

        /// <summary>
        /// Set the deviceID to this Device
        /// </summary>
        /// <param name="param">DeviceID in the system</param>
        public void SetDeviceId(int param)
        {
            deviceId = param;
        }
    }

    private HapticController[] controllers;
    private bool allControllersAssigned = false;
    private List<Device> devicesInScene;
    private int currentIndex;
    private Location currentGloveLocation;
    private string message;
    private int leftDevices, rightDevices;
    private int leftHmcs, rightHmcs;

    private string templateMessage = "Player <b>{1}</b>, perform the pinch gesture with your <b>{2} hand</b> to assign that device, or press <b>Esc</b> button to omit it";
    /// <summary>
    /// UI Text reference if we want to show a message
    /// </summary>
    public Text uiTextMessage;

    [HideInInspector]
    public int numberOfAxesChanged = 0;

    /// <summary>
    /// Current user that is assigning the device
    /// </summary>
    public static int currentUser;

    /// <summary>
    /// Populate arrays with physical devices and hands in scene.
    /// </summary>
    void Init()
    {
        devicesInScene = new List<Device>();

        // Get all connected devices (physical devices)
        NDController.GetHapticControllers(out controllers);
        // Count the number of controllers per device
        if (controllers != null)
        {
            foreach (HapticController hc in controllers)
            {
                if (hc.Location == Location.LOC_RIGHT_HAND)
                    rightDevices++;
                else if (hc.Location == Location.LOC_LEFT_HAND)
                    leftDevices++;
            }
        }

        if (rightDevices + leftDevices == 0)
        {
            allControllersAssigned = true;

            message = "Please, connect any suitable device (Gloveone or AvatarVR)";
            if (uiTextMessage != null)
                uiTextMessage.text = message;
            else
                Debug.LogWarning(message);

            return;
        }

        // Get all NDDevice in the scene (hands in scene) and sorts them
        List<HandModelController> hmcsSorted = new List<HandModelController>();
        hmcsSorted.AddRange(GameObject.FindObjectsOfType<HandModelController>());
        if (hmcsSorted.Count != 0)
        {
            hmcsSorted.Sort();

            // Populate the Devices array in the inverted order to show P1_right, P1_left, P2_right...
            foreach (HandModelController hmc in hmcsSorted)
            {
                devicesInScene.Add(new Device(hmc, false, -1));

                // Count the number of HMCs per location
                if (hmc.handLocation == Location.LOC_LEFT_HAND)
                {
                    leftHmcs++;
                    CheckGameObjectName(hmc.gameObject, hmc.user);
                }
                else if (hmc.handLocation == Location.LOC_RIGHT_HAND)
                {
                    rightHmcs++;
                    CheckGameObjectName(hmc.gameObject, hmc.user);
                }
            }
        }
        else
        {
            foreach (HapticController c in controllers)
            {
                devicesInScene.Add(new Device(null, false, -1));
                if (leftDevices > 0 && rightDevices > 0)
                    devicesInScene.Add(new Device(null, false, -1));
            }
        }

        WaitForDevice();
    }

    private void CheckGameObjectName(GameObject go, int user)
    {
        if (!go.name.Contains("_P") || !System.Char.IsDigit(go.name.ToCharArray()[go.name.Length - 1]))
        {
            go.name = go.name + "_P" + user;
        }
    }

    /// <summary>
    /// Checks if the device is being used
    /// </summary>
    /// <returns><c>true</c>, if the glove is in use, <c>false</c> otherwise.</returns>
    /// <param name="id">Device ID to check</param>
    private bool IsAssigned(int id)
    {
        foreach (Device device in devicesInScene)
        {
            if (device.isAssigned && device.deviceId == id)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Change variables and wait for the next device to assign
    /// </summary>
    private void WaitForDevice()
    {
        if (currentIndex >= devicesInScene.Count)
        {
            allControllersAssigned = true;
            NDService.Instance.AllAssigned();
            if (uiTextMessage != null)
            {
                Invoke("HideUIMessage", 2f);

            }
            ShowUIMessage();
            return;
        }

        if (leftHmcs != 0 || rightHmcs != 0)
        {
            currentGloveLocation = devicesInScene[currentIndex].hmc.handLocation;
        }
        else
        {
            if (leftDevices == 0 && rightDevices > 0)
            {
                currentGloveLocation = Location.LOC_RIGHT_HAND;
            }
            else if (leftDevices > 0 && rightDevices == 0)
            {
                currentGloveLocation = Location.LOC_LEFT_HAND;
            }
            else
            {
                if ((currentIndex + 1) % 2 == 0)
                    currentGloveLocation = Location.LOC_LEFT_HAND;
                else
                    currentGloveLocation = Location.LOC_RIGHT_HAND;
            }
        }

        // If there is not any other glove of the given location in the scene, put that glove as assigned
        if ((currentGloveLocation == Location.LOC_LEFT_HAND && leftDevices <= 0) || (currentGloveLocation == Location.LOC_RIGHT_HAND && rightDevices <= 0))
        {
            devicesInScene[currentIndex].SetIsAssigned(true);
            currentIndex++;
            WaitForDevice();
        }

        ShowUIMessage();
    }

    private void HideUIMessage()
    {
        uiTextMessage.text = "";
    }

    private void ShowUIMessage()
    {
        if (!allControllersAssigned)
        {
            message = templateMessage;

            if (leftHmcs == 0 && rightHmcs == 0)
            {
                if (leftDevices == 0 && rightDevices > 0 || leftDevices > 0 && rightDevices == 0)
                {
                    currentUser = currentIndex + 1;
                }
                else
                {
                    currentUser = (int)Math.Round((currentIndex + 1) / 2f, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                currentUser = devicesInScene[currentIndex].hmc.user;
            }

            message = message.Replace("{1}", currentUser.ToString());
            if (currentGloveLocation == Location.LOC_RIGHT_HAND)
            {
                message = message.Replace("{2}", "right");
            }
            else
            {
                message = message.Replace("{2}", "left");
            }
        }
        else
        {
            message = "All controllers are assigned, have fun :)";
        }

        if (uiTextMessage != null)
            uiTextMessage.text = message;
        else
            Debug.Log(message);
    }

    void Update()
    {
        int assignedDeviceId = -1;
        bool found = false;

        if (!allControllersAssigned)
        {
            //Get all connected devices(physical devices)
            NDController.GetHapticControllers(out controllers);
            foreach (HapticController c in controllers)
            {
                // If the glove is not assigned yet and is performing the pinch gesture
                if (c != null && !IsAssigned(c.DeviceId) && c.AreContactsJoined(Contact.CONT_THUMB, Contact.CONT_INDEX) > 0)
                {
                    // If the glove location matches with the location we expect
                    if (c.Location == currentGloveLocation)
                    {
                        assignedDeviceId = c.DeviceId;
                        found = true;
                        break;
                    }
                }

                //If there is just one glove and one HandMotionController of that location in scene, connect it directly.
                // Same with just one or two gloves connected and zero HandModelControllers in scene
                if ((rightDevices == 1 && rightHmcs == 1 && leftDevices == 0 && leftHmcs == 1) || (leftDevices == 1 && leftHmcs == 1 && rightHmcs == 0 && rightDevices == 1) ||
                    (rightDevices == 1 && rightHmcs == 1 && leftDevices == 1 && leftHmcs == 0) || (leftDevices == 1 && leftHmcs == 1 && rightHmcs == 1 && rightDevices == 0) ||
                    (rightDevices == 1 && rightHmcs == 1 && leftDevices == 0 && leftHmcs == 0) || (leftDevices == 1 && leftHmcs == 1 && rightHmcs == 0 && rightDevices == 0) ||
                    (rightDevices == 1 && rightHmcs == 1 && leftDevices == 1 && leftHmcs == 1) || (leftDevices == 1 && leftHmcs == 0 && rightHmcs == 0 && rightDevices == 0) ||
                    (leftDevices == 0 && leftHmcs == 0 && rightHmcs == 0 && rightDevices == 1) || (leftDevices == 1 && leftHmcs == 0 && rightHmcs == 0 && rightDevices == 1))
                {
                    if (c.Location == currentGloveLocation)
                    {
                        assignedDeviceId = c.DeviceId;
                        found = true;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }

            if (found)
            {
                if (assignedDeviceId != -1)
                {
                    devicesInScene[currentIndex].AssignGlove(assignedDeviceId);
                }
                else
                {
                    devicesInScene[currentIndex].SetIsAssigned(true);
                }

                currentIndex++;
                WaitForDevice();
            }

            // We can omit the assignment with the Escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                devicesInScene[currentIndex].SetIsAssigned(true);

                currentIndex++;
                WaitForDevice();
            }
        }
    }
}
