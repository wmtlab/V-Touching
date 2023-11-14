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
using System.Collections.Generic;
using UnityDLL.Haptic;
using UnityEngine;

/// <summary>
/// Performing the gestures sequencse established in this script, the device is recalibrated
/// Soft calibration recalibrates the device towards its local Z+
/// Hard calibration recalibrates the device towards global Z+
/// </summary>
public class InGameCalibration : MonoBehaviour
{
    // Timing
    /// <summary>
    /// Time (in seconds) needed to perform the gestures
    /// </summary>
    public float timeToPerformGestures;
    /// <summary>
    /// Time (in seconds) to wait since the last gesture has been done to perform the calibration
    /// </summary>
    public float timeToWaitAfterGestures;
    /// <summary>
    /// Inner counter
    /// </summary>
    private float currentTimeToPerformGestures;

    // Gestures sequence
    /// <summary>
    /// Soft calibration can be performed
    /// </summary>
    public bool enableSoftCalibration;
    /// <summary>
    /// List of gestures that have to be done in order to calibrate (soft calibration)
    /// </summary>
    public Gesture[] softCalibrationGesturesSequence;
    /// <summary>
    /// Hard calibration can be performed
    /// </summary>
    public bool enableHardCalibration;
    /// <summary>
    /// List of gestures that have to be done in order to calibrate (hard calibration)
    /// </summary>
    public Gesture[] hardCalibrationGesturesSequence;
    /// <summary>
    /// Gesture that is being performed
    /// </summary>
    private Gesture currentGesture;
    /// <summary>
    /// Previous gesture done
    /// </summary>
    private Gesture previousGesture;
    /// <summary>
    /// True if the window gesture is opened
    /// </summary>
    private bool isCheckingGesture;
    /// <summary>
    /// Current soft index of the gestures array
    /// </summary>
    private int currentSoftIndex;
    /// <summary>
    /// Current hard index of the gestures array
    /// </summary>
    private int currentHardIndex;
    /// <summary>
    /// True if all the gestures have been done on time (soft calibration)
    /// </summary>
    private bool softSequenceCompleted;
    /// <summary>
    /// True if all the gestures have been done on time (hard calibration)
    /// </summary>
    private bool hardSequenceCompleted;

    // Haptic
    private float factor;
    /// <summary>
    /// Intensity of the haptic feedback
    /// </summary>
    private float intensity;

    // Gestures 
    /// <summary>
    /// True if a gesture is recognized
    /// </summary>
    private bool isGesture;
    private bool isIndexPinchGesture;
    private bool isMiddlePinchGesture;
    private bool isGunGesture;
    private bool isOkGesture;
    private bool isThreePinchGesture;
    private bool isPinchFistGesture;

    /// <summary>
    /// HandModelController needed to perform the recalibration
    /// </summary>
    private HandModelController hmc;
    private IEnumerator coroutine;

    private void Awake()
    {
        hmc = GetComponent<HandModelController>();
        isGesture = false;
        currentSoftIndex = 0;
        softSequenceCompleted = false;
        factor = 1 / timeToWaitAfterGestures;
        intensity = 0;
        currentTimeToPerformGestures = timeToPerformGestures;
    }

    void Start()
    {
        coroutine = GetDevice();
        StartCoroutine(coroutine);

        // Clear empty gesture
        List<Gesture> gesturesList = new List<Gesture>();
        for (int i = 0; i < softCalibrationGesturesSequence.Length; i++)
        {
            if (softCalibrationGesturesSequence[i] != Gesture.None)
            {
                gesturesList.Add(softCalibrationGesturesSequence[i]);
            }
        }
        softCalibrationGesturesSequence = gesturesList.ToArray();

        // Clear empty gesture
        gesturesList = new List<Gesture>();
        for (int i = 0; i < hardCalibrationGesturesSequence.Length; i++)
        {
            if (hardCalibrationGesturesSequence[i] != Gesture.None)
            {
                gesturesList.Add(hardCalibrationGesturesSequence[i]);
            }
        }
        hardCalibrationGesturesSequence = gesturesList.ToArray();

        if (softCalibrationGesturesSequence.Length == 0)
        {
            Debug.LogWarning("Disabled InGameCalibration. Gestures array must not be empty");
            enableSoftCalibration = false;
            this.enabled = false;
        }

        if (hardCalibrationGesturesSequence.Length == 0)
        {
            enableHardCalibration = false;
        }
    }

    /// <summary>
    /// Gets the NDDevice attached to the associated HandModelController and subscribes gestures methods
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetDevice()
    {
        while (hmc.device == null)
        {
            yield return new WaitForSeconds(0.5f);
            if (hmc.device != null)
            {
                hmc.device.isPinchGestureEvent += new NDDevice.isPinchGestureHandler(IndexPinchGesture);
                hmc.device.isMiddlePinchGestureEvent += new NDDevice.isMiddlePinchGestureHandler(MiddlePinchGesture);
                hmc.device.isGunGestureEvent += new NDDevice.isGunGestureHandler(GunGesture);
                hmc.device.isOkGestureEvent += new NDDevice.isOkGestureHandler(OkGesture);
                hmc.device.isThreePinchGestureEvent += new NDDevice.isThreePinchGestureHandler(ThreePinchGesture);
                hmc.device.isPinchFistGestureEvent += new NDDevice.isPinchFistGestureHandler(PinchFistGesture);
            }
        }
    }

    /// <summary>
    /// Establish index pinch gesture as current gesture
    /// </summary>
    public void IndexPinchGesture()
    {
        currentGesture = Gesture.IndexPinchGesture;
        isIndexPinchGesture = true;
    }

    /// <summary>
    /// Establish middle pinch gesture as current gesture
    /// </summary>
    public void MiddlePinchGesture()
    {
        currentGesture = Gesture.MiddlePinchGesture;
        isMiddlePinchGesture = true;
    }

    /// <summary>
    /// Establish gun gesture as current gesture
    /// </summary>
    public void GunGesture()
    {
        currentGesture = Gesture.GunGesture;
        isGunGesture = true;
    }

    /// <summary>
    /// Establish ok gesture as current gesture
    /// </summary>
    public void OkGesture()
    {
        currentGesture = Gesture.OkGesture;
        isOkGesture = true;
    }

    /// <summary>
    /// Establish three pinch gesture as current gesture
    /// </summary>
    public void ThreePinchGesture()
    {
        currentGesture = Gesture.ThreePinchGesture;
        isThreePinchGesture = true;
    }

    /// <summary>
    /// Establish pinch fist gesture as current gesture
    /// </summary>
    public void PinchFistGesture()
    {
        currentGesture = Gesture.PinchFistGesture;
        isPinchFistGesture = true;
    }

    /// <summary>
    /// Resets current gesture
    /// </summary>
    private void ResetGesture()
    {
        currentGesture = Gesture.None;
    }

    void Update()
    {
        if (!enableSoftCalibration && !enableHardCalibration)
            return;

        isGesture = false;
        isGesture = isIndexPinchGesture || isMiddlePinchGesture || isGunGesture || isOkGesture || isThreePinchGesture || isPinchFistGesture;

        isIndexPinchGesture = false;
        isMiddlePinchGesture = false;
        isGunGesture = false;
        isOkGesture = false;
        isThreePinchGesture = false;
        isPinchFistGesture = false;

        if (softSequenceCompleted || hardSequenceCompleted)
        {
            intensity += Time.deltaTime * factor;
            HapticSystem.PlayValue(intensity, hmc.handLocation, hmc.user);
        }

        // Opened sequence window
        if (isCheckingGesture)
        {
            // Update remaining time
            currentTimeToPerformGestures -= Time.deltaTime;

            if (enableHardCalibration)
            {
                // Check hard calibration
                if (currentHardIndex < hardCalibrationGesturesSequence.Length)
                {
                    if (previousGesture != currentGesture && currentGesture == hardCalibrationGesturesSequence[currentHardIndex])
                    {
                        currentHardIndex++;
                    }
                }
                else
                {
                    ResetWindow();

                    // Sequence done successfully
                    hardSequenceCompleted = true;
                    softSequenceCompleted = false;
                }
            }

            if (enableSoftCalibration)
            {
                // Check soft calibration
                if (currentSoftIndex < softCalibrationGesturesSequence.Length)
                {
                    if (previousGesture != currentGesture && currentGesture == softCalibrationGesturesSequence[currentSoftIndex])
                    {
                        currentSoftIndex++;
                    }
                }
                else
                {
                    ResetWindow();

                    // Sequence done successfully
                    softSequenceCompleted = true;
                }
            }

            if (hardSequenceCompleted || softSequenceCompleted)
            {
                Invoke("Recalibrate", timeToWaitAfterGestures);
                return;
            }

            if (currentTimeToPerformGestures <= 0)
            {
                ResetWindow();
            }
        }

        // If there is not any gesture, reset
        if (!isGesture)
        {
            ResetGesture();
        }
        else
        {
            // If there is any gesture, checks if window can be opened
            if (!isCheckingGesture)
            {
                if (enableSoftCalibration && (previousGesture != currentGesture && (currentGesture == softCalibrationGesturesSequence[currentSoftIndex])))
                {
                    currentSoftIndex++;
                    isCheckingGesture = true;
                }

                if (enableHardCalibration && (previousGesture != currentGesture && (currentGesture == hardCalibrationGesturesSequence[currentHardIndex])))
                {
                    currentHardIndex++;
                    isCheckingGesture = true;
                }
            }
        }
        previousGesture = currentGesture;
    }

    /// <summary>
    /// Calls the Recalibrate method from HandModelController
    /// </summary>
    private void Recalibrate()
    {
        intensity = 0f;
        HapticSystem.StopActuators(hmc.user);

        if (hardSequenceCompleted)
        {
            hardSequenceCompleted = false;
            hmc.Recalibrate(true);
        }
        else if (softSequenceCompleted)
        {
            softSequenceCompleted = false;
            hmc.Recalibrate(hmc.forceInitialPoseCalibration);
        }
    }

    /// <summary>
    /// Resets gestures window
    /// </summary>
    private void ResetWindow()
    {
        currentTimeToPerformGestures = timeToPerformGestures;
        isCheckingGesture = false;
        currentSoftIndex = 0;
        currentHardIndex = 0;
        softSequenceCompleted = false;
        hardSequenceCompleted = false;
    }
}
