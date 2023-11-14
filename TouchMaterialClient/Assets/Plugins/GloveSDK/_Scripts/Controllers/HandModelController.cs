// * 注释掉了void CreateHandMotionSensor()中Calibration一行，使虚拟手随tracker旋转

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
using NDAPIWrapperSpace;
using UnityDLL.Core;
using UnityDLL.Motion;
using UnityEngine;

/// <summary>
/// This script controls model bones, from fingers to upper body parts. It has to be attached to the hand model.
/// </summary>
[RequireComponent(typeof(MUXDataProvider_Hand))]
public class HandModelController : MonoBehaviour, IComparable<HandModelController>
{
    /// <summary>
    /// Hand location, left or right
    /// </summary>
    public Location handLocation;
    /// <summary>
    /// UserID
    /// </summary>
    public int user;
    /// <summary>
    /// NDDevice attached. This device acts as bridge from glove to model
    /// </summary>
    [HideInInspector]
    public NDDevice device;
    /// <summary>
    /// If this property is true, the model is ready to get data from glove.
    /// </summary>
    private bool canGetData;
    /// <summary>
    /// Controls rotation of the hand
    /// </summary>
    private MotionSensor handMotionSensor;
    /// <summary>
    /// Transform of hand bone
    /// </summary>
    private Transform handBone;
    /// <summary>
    /// Through this NDModelAxesChanger, it can be obtained the correct Transform where to apply rotations
    /// </summary>
    private NDModelAxesChanger axesChanger;
    /// <summary>
    /// Array where are stored the FingerModelController of each finger. With this objects, rotations are
    /// applied to fingers
    /// </summary>
    private FingerModelController[] fingers;
    /// <summary>
    /// Array where are stored the UpperModelController of each part of upper body. Only necessary for AvatarVR with TB2/TB3
    /// </summary>
    private UpperModelController[] upperParts;
    /// <summary>
    /// Structure that contains rotations and hand position that is going to be applied to the model. This structure has
    /// the result of collecting information from the inner mux
    /// </summary>
    private MuxData dataToApply;
    /// <summary>
    /// Mux that keeps sources data. Data from glove and gestures are stored as common values. If rotation is going to be retrieved
    /// from other source, it needs to store information as new registry in this mux, and then apply to model
    /// </summary>
    private Dictionary<string, MuxData> sourceDict;
    /// <summary>
    /// Array of bones' Transform. Data collected from sourceDict is stored at dataToApply, and then it is applied to each
    /// of this Transforms list.
    /// </summary>
    private Transform[] tfList;

    // Gestures
    /// <summary>
    /// Index pinch prefab gesture
    /// </summary>
    public HandGesture indexPinchGesture;
    /// <summary>
    /// Middle pinch prefab gesture
    /// </summary>
    public HandGesture middlePinchGesture;
    /// <summary>
    /// Three pinch prefab gesture
    /// </summary>
    public HandGesture threePinchGesture;
    /// <summary>
    /// Gun prefab gesture
    /// </summary>
    public HandGesture gunGesture;
    /// <summary>
    /// OK prefab gesture
    /// </summary>
    public HandGesture okGesture;
    /// <summary>
    /// Pinch fist prefab gesture
    /// </summary>
    public HandGesture pinchFistGesture;
    /// <summary>
    /// Determines if gestures are going to be recognized
    /// </summary>
    public bool useGestures;
    /// <summary>
    /// True if a gesture is done
    /// </summary>
    private bool isGesture;
    /// <summary>
    /// True if index pinch gesture is being done, false otherwise
    /// </summary>
    private bool isIndexPinchGesture;
    /// <summary>
    /// True if middle pinch gesture is being done, false otherwise
    /// </summary>
    private bool isMiddlePinchGesture;
    /// <summary>
    /// True if three pinch gesture is being done, false otherwise
    /// </summary>
    private bool isThreePinchGesture;
    /// <summary>
    /// True if gun gesture is being done, false otherwise
    /// </summary>
    private bool isGunGesture;
    /// <summary>
    /// True if OK gesture is being done, false otherwise
    /// </summary>
    private bool isOkGesture;
    /// <summary>
    /// True if pinch fist gesture is being done, false otherwise
    /// </summary>
    private bool isPinchFistGesture;
    /// <summary>
    /// Counter that determines from 0 to 1 the gesture lerp
    /// </summary>
    private float accumTimeToGesture;
    /// <summary>
    /// With this value, we can establish the speed of gesture lerp
    /// </summary>
    private float factorTimeToGesture = 2f;
    /// <summary>
    /// Inverse of FactorTimeToGesture
    /// </summary>
    private float multTimeGesture;
    /// <summary>
    /// Performing a gesture, with this property we check if a previous gesture has been done
    /// </summary>
    private bool isGestureDone;
    /// <summary>
    /// Previous gesture
    /// </summary>
    private Gesture previousGesture = Gesture.None;
    /// <summary>
    /// Current gesture
    /// </summary>
    private Gesture newGesture = Gesture.None;
    /// <summary>
    /// Initial rotation of the gesture lerp
    /// </summary>
    private Quaternion[][] initialRot;
    /// <summary>
    /// Auxiliar Quaternion[][] where data is stored from mux to DataToApply
    /// </summary>
    private Quaternion[][] dataToApplyAux;
    /// <summary>
    /// Auxiliar MuxData where data is stored from mux to DataToApply
    /// </summary>
    private MuxData sourceData;
    /// <summary>
    /// Number of bones that each finger has
    /// </summary>
    private int numberOfBones;
    /// <summary>
    /// Tag array that determines the rotation data source of each bone
    /// </summary>
    private string[] tagControllingRotationSource;
    /// <summary>
    /// Tag variable that determines the position data source of the hand
    /// </summary>
    private string tagControllingPositionSource;
    /// <summary>
    /// Type of rotation of each bone (local, global, local offset, global offset)
    /// </summary>
    private Type_MUXValueType[] typeRotationSource;
    /// <summary>
    /// Type of hand position (local, global, local offset, global offset)
    /// </summary>
    private Type_MUXValueType typePositionSource;
    /// <summary>
    /// MuxData for gestures
    /// </summary>
    private MuxData gesturesMuxData;
    /// <summary>
    /// MuxDataProvider of the hand. This MuxDataProvider acts as main source
    /// </summary>
    private PCI_MUXDataProvider handMuxDataProvider;
    /// <summary>
    /// Hand parent transform
    /// </summary>
    private Transform tfHandParent;

    private string previousTagControllingPositionSource;
    private bool lerpHandPosition;
    private bool isAssignedInitialPosition;
    private Vector3 newPosition = Vector3.zero;

    private float maxTimeForCalibration = 1f;
    private float startCalibrationTime, endCalibrationTime;
    private bool isCheckingRecalibration;

    /// <summary>
    /// If true, every reset action will set the torso orientation looking at global forward;
    /// Otherwise, the torso orientation will be set according to the hand orientation
    /// </summary>
    public bool forceInitialPoseCalibration = true;
    /// <summary>
    /// This variable determines the orientation of the body. Every reset action will recalibrate
    /// according to this orientation
    /// </summary>
    public PlayerForwardCalculator playerForwardCalculator;

    /// <summary>
    /// If true, the forearm calibration will be performed applying Z orientation;
    /// Otherwise, the forearm calibration will be performed using the xForced orientation that applies no Z rotation
    /// </summary>
    public bool calibrateForearmUsingZCorrection = true;

    /// <summary>
    /// Degrees to rotate the hand when the palm faces upside on calibration
    /// </summary>
    private float calibrationHandDegreesOnUp = 180f;
    /// <summary>
    /// Degrees to rotate the forearm when the palm faces upside on calibration
    /// </summary>
    private float calibrationForearmDegreesOnUp = 160f;

    /// <summary>
    /// When the device is reseted, trigger this ScriptableObject
    /// </summary>
    public SO_TriggerEvent avatarResetTrigger;


    /// <summary>
    /// Ensure that there is going to be only one instance of this HandModelController between scenes without a static variable
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);

        handMuxDataProvider = GetComponent<MUXDataProvider_Hand>();

        HandModelController[] hmcs = GameObject.FindObjectsOfType<HandModelController>();
        int counter = 0;
        foreach (HandModelController hmc in hmcs)
        {
            if (hmc.name.Contains(transform.name) && hmc.user == user)
                counter++;
        }

        if (counter > 1)
            Destroy(transform.root.gameObject);
        else
            Init();
    }

    /// <summary>
    /// Initialization of variables
    /// </summary>
    private void Init()
    {
        dataToApply = new MuxData(null);

        numberOfBones = Enum.GetNames(typeof(SensorID)).Length;


        initialRot = new Quaternion[numberOfBones][];
        dataToApplyAux = new Quaternion[numberOfBones][];
        for (int i = 0; i < numberOfBones; i++)
        {
            initialRot[i] = new Quaternion[2];
            dataToApplyAux[i] = new Quaternion[2];
        }

        canGetData = false;

        axesChanger = GetComponent<NDModelAxesChanger>();
        if (axesChanger)
        {
            handBone = axesChanger.GetDataTrasform();
            tfHandParent = handBone.parent;

        }
        fingers = new FingerModelController[10]; //TODO

        gesturesMuxData = new MuxData(null);

        tagControllingRotationSource = new string[Enum.GetNames(typeof(SensorID)).Length];
        typeRotationSource = new Type_MUXValueType[Enum.GetNames(typeof(SensorID)).Length];

        for (int i = 0; i < tagControllingRotationSource.Length; i++)
        {
            tagControllingRotationSource[i] = handMuxDataProvider.sourceId;
            typeRotationSource[i] = handMuxDataProvider.bonesToControlRotationType[i];
        }

        tagControllingPositionSource = handMuxDataProvider.sourceId;
        typePositionSource = handMuxDataProvider.bonesToControlPositionType[0];


        tfList = new Transform[Enum.GetNames(typeof(SensorID)).Length];

        multTimeGesture = 1 / factorTimeToGesture;
    }

    /// <summary>
    /// Create a registry of the specified source in MuxData dictionary
    /// </summary>
    /// <param name="sourceMuxDataProvider">PCI_MuxDataProvider that acts as data source</param>
    /// <returns>MuxData created</returns>
    public MuxData CreateSourceInDictionary(PCI_MUXDataProvider sourceMuxDataProvider)
    {
        MuxData auxModelData = new MuxData(sourceMuxDataProvider);

        if (sourceDict == null)
            sourceDict = new Dictionary<string, MuxData>();

        sourceDict.Add(sourceMuxDataProvider.sourceId, auxModelData);

        return auxModelData;
    }


    /// <summary>
    /// Creates the MotionSensor attached to the hand
    /// </summary>
    void CreateHandMotionSensor()
    {
        // Creates the hand bone controller
        handMotionSensor = new MotionSensor();
        handMotionSensor.SetBone(handBone.gameObject);

        // Set initial position and calibration pose
        //handMotionSensor.SetCalibrationPose(handBone.rotation, axesChanger.GetAlignerRotation());

        //Create a world angle calculator in the hand to set the calibration direction if an external one was not assigned
        if (playerForwardCalculator == null)
            playerForwardCalculator = handBone.gameObject.AddComponent<PlayerForwardCalculator>();

        tfList[(int)SensorID.Hand] = handBone;
    }

    /// <summary>
    /// Gets bone Transform of each finger and its phalanges
    /// </summary>
    private void GetModelFingers()
    {
        // Get FingerModelController of hand and fingers
        FingerModelController[] fingersAux = transform.GetComponentsInChildren<FingerModelController>();

        foreach (FingerModelController finger in fingersAux)
        {
            fingers[(int)finger.AxesChanger.part] = finger;

            Transform[] bones = finger.CreateFingerMotionSensor(handMotionSensor);

            if (bones != null)
            {
                switch (finger.AxesChanger.part)
                {
                    case NDModelAxesChanger.Parts.thumb0:
                        tfList[(int)SensorID.Thumb0] = bones[0];
                        tfList[(int)SensorID.Thumb1] = bones[1];
                        tfList[(int)SensorID.Thumb2] = bones[2];
                        break;
                    case NDModelAxesChanger.Parts.index:
                        tfList[(int)SensorID.Index] = bones[0];
                        tfList[(int)SensorID.Index1] = bones[1];
                        tfList[(int)SensorID.Index2] = bones[2];
                        break;
                    case NDModelAxesChanger.Parts.middle:
                        tfList[(int)SensorID.Middle] = bones[0];
                        tfList[(int)SensorID.Middle1] = bones[1];
                        tfList[(int)SensorID.Middle2] = bones[2];
                        break;
                    case NDModelAxesChanger.Parts.ring:
                        tfList[(int)SensorID.Ring] = bones[0];
                        tfList[(int)SensorID.Ring1] = bones[1];
                        tfList[(int)SensorID.Ring2] = bones[2];
                        break;
                    case NDModelAxesChanger.Parts.pinky:
                        tfList[(int)SensorID.Pinky] = bones[0];
                        tfList[(int)SensorID.Pinky1] = bones[1];
                        tfList[(int)SensorID.Pinky2] = bones[2];
                        break;
                }

            }
            else
            {
                Debug.LogWarning("Finger " + finger.AxesChanger.part + " not assigned");
            }
        }
    }

    /// <summary>
    /// If a TrackBand is plugged, get data from upper body
    /// </summary>
    private void GetUpperModel()
    {
        if (device.trackband == NDDevice.Trackband.TB2)
        {
            upperParts = new UpperModelController[2];
            PopulateUpperParts();
        }
        else if (device.trackband == NDDevice.Trackband.TB3)
        {
            upperParts = new UpperModelController[3];
            PopulateUpperParts();
        }

        if (CheckUpperPartsNull())
        {
            upperParts = null;
        }
    }

    /// <summary>
    /// Determines if there is any upper part stored in upperParts array
    /// </summary>
    /// <returns>True if upperParts array is empty</returns>
    private bool CheckUpperPartsNull()
    {
        if (upperParts == null)
            return true;

        for (int i = 0; i < upperParts.Length; i++)
        {
            if (upperParts[i] != null)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Getting data from parents' NDModelAxesChangers, upperParts array is populated
    /// </summary>
    private void PopulateUpperParts()
    {
        if (upperParts == null)
            return;

        NDModelAxesChanger[] axesChanger = GetComponentsInParent<NDModelAxesChanger>();

        // if there are more AxesChanger in parents... the hand has attached an arm
        if (axesChanger != null && axesChanger.Length > 1)
        {
            int maxIndex = 4;
            if (device.trackband == NDDevice.Trackband.TB2)
            {
                maxIndex = 3;
            }

            for (int i = 1; i < maxIndex; i++)
            {
                UpperModelController modelController = new UpperModelController();

                switch (axesChanger[i].part)
                {
                    case NDModelAxesChanger.Parts.forearm:
                        modelController.CreateMotionSensor(axesChanger[i].GetDataTrasform());
                        upperParts[i - 1] = modelController;

                        tfList[(int)SensorID.Forearm] = axesChanger[i].GetDataTrasform();

                        break;
                    case NDModelAxesChanger.Parts.arm:
                        modelController.CreateMotionSensor(axesChanger[i].GetDataTrasform());
                        upperParts[i - 1] = modelController;

                        tfList[(int)SensorID.Arm] = axesChanger[i].GetDataTrasform();

                        break;
                    case NDModelAxesChanger.Parts.chest:
                        modelController.CreateMotionSensor(axesChanger[i].GetDataTrasform());

                        if (!modelController.AxesChanger.IsChestAssigned())
                        {
                            modelController.AxesChanger.SetChest(true);
                            if (handLocation == Location.LOC_LEFT_HAND)
                            {
                                modelController.UpdateAlignerRotation();
                            }
                        }
                        else
                        {
                            device.trackband = NDDevice.Trackband.TB2;
                        }

                        upperParts[i - 1] = modelController;

                        tfList[(int)SensorID.Chest] = axesChanger[i].GetDataTrasform();

                        break;
                }
            }
        }
    }

    /// <summary>
    /// Starts model elements and communication with glove
    /// </summary>
    /// <param name="device">NDDevice from where data is going to be read</param>
    public void StartGlove(NDDevice device)
    {
        if (device != null)
            this.device = device;

        GetUpperModel();
        CreateHandMotionSensor();
        GetModelFingers();
        EnableContacts();
        SetInitialCalibrationPoseRotations();


        // Add a listener to NDDevice depending on the type of glove
        if (device.device == NDDevice.TypeOfDevice.AvatarVR)
            device.AddAvatarHMCListener(AddAvatarSensorListener);
        else
            device.AddGloveoneHMCListener(AddGloveoneSensorListener);


    }


    /// <summary>
    /// Update source mux with data given by AvatarVR
    /// </summary>
    /// <param name="quat">values read from sensors, stored in Quaternion[]</param>
    private void AddAvatarSensorListener(Quaternion[] quat)
    {
        UpdateSourceDict(NDDevice.RAW_SOURCE_TAG, CalculatePhalangesLimits(quat));
        canGetData = true;
    }

    /// <summary>
    /// Update source mux with data given by Gloveone
    /// </summary>
    /// <param name="handRotation">Rotation of the hand</param>
    /// <param name="flexValues">values read from fingers sensors, stored in Float[]</param>
    private void AddGloveoneSensorListener(Quaternion handRotation, float[] flexValues)
    {
        UpdateSourceDict(NDDevice.RAW_SOURCE_TAG, CalculatePhalangesLimits(handRotation, flexValues));
        canGetData = true;
    }

    /// <summary>
    /// Update source mux with data given by an external source
    /// </summary>
    /// <param name="source">Source name</param>
    /// <param name="quatList">Array of Quaternions that represents the rotation of each bone</param>
    public void UpdateExternalSensorData(string source, Quaternion[] quatList)
    {
        UpdateSourceDict(source, CalculatePhalangesLimits(quatList));
    }

    /// <summary>
    /// Given the sourceID, boneID and its value, update this specific bone value
    /// </summary>
    /// <param name="source">SourceID to get from the MuxData dictionary</param>
    /// <param name="boneID">BoneID to get from the specified source</param>
    /// <param name="quat">Value to update</param>
    public void UpdateExternalSensorData(string source, int boneID, Quaternion quat)
    {
        MuxData temp;
        sourceDict.TryGetValue(source, out temp);
        if (temp != null)
        {
            temp.SetData(boneID, new Quaternion[] { quat, Quaternion.identity });
            sourceDict[source] = temp;
        }
    }

    /// <summary>
    /// Given the source ID and data, store this values in the correct registry of the mux
    /// </summary>
    /// <param name="source">Source ID</param>
    /// <param name="quatValues">Matrix of Quaternions, where: Quaternion[i][j], "i" represents the bone,
    /// and "j" the component Y or X</param>
    public void UpdateSourceDict(string source, Quaternion[][] quatValues)
    {
        MuxData temp;
        sourceDict.TryGetValue(source, out temp);
        if (temp != null)
        {
            temp.FillSensorsData(quatValues);
            sourceDict[source] = temp;
        }
    }

    /// <summary>
    /// Given the source ID and data, store this values in the correct registry of the mux
    /// </summary>
    /// <param name="source">Source ID</param>
    /// <param name="modelData">MuxData to overwrite</param>
    public void UpdateSourceDict(string source, MuxData modelData)
    {
        MuxData temp;
        sourceDict.TryGetValue(source, out temp);
        if (temp != null)
        {
            temp.FillSensorsData(modelData);
            sourceDict[source] = temp;
        }
    }

    /// <summary>
    /// Given the source ID and data, store this values in the correct registry of the mux
    /// </summary>
    /// <param name="source">Source ID</param>
    /// <param name="globalHandPosition">Global position of the hand</param>
    public void UpdateSourceDict(string source, Vector3 globalHandPosition)
    {
        MuxData temp;
        sourceDict.TryGetValue(source, out temp);
        if (temp != null)
        {
            temp.UpdateHandPosition(globalHandPosition);
            sourceDict[source] = temp;
        }
    }

    /// <summary>
    /// Given the source ID and data, store this values in the correct registry of the mux
    /// </summary>
    /// <param name="source">Source ID</param>
    /// <param name="quatValue">Components X and Y of the hand</param>
    /// <param name="flexValues">Value of each finger, from 0 to 1</param>
    private void UpdateSourceDict(string source, Quaternion[] quatValue, float[] flexValues)
    {
        MuxData temp;
        sourceDict.TryGetValue(source, out temp);
        if (temp != null)
        {
            temp.FillSensorsData(quatValue, flexValues);
            sourceDict[source] = temp;
        }
    }

    /// <summary>
    /// Change the current source to the new specified
    /// </summary>
    /// <param name="selectedSource">New current source</param>
    public void UpdateSourcesWithNewPriorities()
    {
        int[] highestRotationPriorityFound = new int[tagControllingRotationSource.Length];
        int highestPositionPriorityFound = -1;

        for (int i = 0; i < tagControllingRotationSource.Length; i++)
        {
            tagControllingRotationSource[i] = handMuxDataProvider.sourceId;
            typeRotationSource[i] = handMuxDataProvider.bonesToControlRotationType[i];
        }

        tagControllingPositionSource = handMuxDataProvider.sourceId;
        typePositionSource = handMuxDataProvider.bonesToControlPositionType[0];

        int currentMuxProviderPriority = -1;
        PCI_MUXDataProvider tempMuxDataProvider = null;



        foreach (KeyValuePair<string, MuxData> singleSourceMuxData in sourceDict)
        {
            currentMuxProviderPriority = singleSourceMuxData.Value.GetMuxSourceDataProvider().GetPriority();
            // Rotation
            for (int i = 0; i < tagControllingRotationSource.Length; i++)
            {
                tempMuxDataProvider = singleSourceMuxData.Value.GetMuxSourceDataProvider();
                if (tempMuxDataProvider.MustControlBoneRotation(i))
                {
                    if (currentMuxProviderPriority > highestRotationPriorityFound[i])
                    {
                        highestRotationPriorityFound[i] = currentMuxProviderPriority;
                        tagControllingRotationSource[i] = singleSourceMuxData.Key;
                        typeRotationSource[i] = tempMuxDataProvider.GetBoneRotationType(i);
                    }
                }
                else
                {
                    if (highestRotationPriorityFound[i] == 0 && currentMuxProviderPriority == 0)
                    {
                        highestRotationPriorityFound[i] = -1;
                        tagControllingRotationSource[i] = null;
                    }
                }
            }

            // Position
            if (singleSourceMuxData.Value.GetMuxSourceDataProvider().MustControlBonePosition(0))
            {
                if (currentMuxProviderPriority > highestPositionPriorityFound)
                {
                    highestPositionPriorityFound = currentMuxProviderPriority;
                    tagControllingPositionSource = singleSourceMuxData.Key;
                    typePositionSource = tempMuxDataProvider.GetBonePositionType(0);


                    if (tagControllingPositionSource != previousTagControllingPositionSource)
                    {
                        //if (tagControllingPositionSource != NDDevice.RAW_SOURCE_TAG)
                        //{
                        lerpHandPosition = true;
                        isAssignedInitialPosition = false;
                        //}
                    }
                    else
                    {
                        lerpHandPosition = false;
                    }

                    previousTagControllingPositionSource = tagControllingPositionSource;
                }
            }
        }
    }

    /// <summary>
    /// Given rotation from hand and values from fingers, converts input rotations to
    /// rotations that can be applied to bones, with restricted rotations applied.
    /// Gloveone only
    /// </summary>
    /// <param name="handBoneQuaternion">Rotation of the hand</param>
    /// <param name="flexValues">Values from fingers, from 0 to 1</param>
    /// <returns>Matrix of Quaternions, where: Quaternion[i][j],
    /// "i" represents the bone, and "j" the component Y or X</returns>
    private Quaternion[][] CalculatePhalangesLimits(Quaternion handBoneQuaternion, float[] flexValues)
    {
        Quaternion[][] qAux = new Quaternion[numberOfBones][];
        for (int i = 0; i < qAux.Length; i++)
        {
            qAux[i] = new Quaternion[2];
        }

        // Hand
        qAux[0] = handMotionSensor.RotateBone(handBoneQuaternion);

        for (int i = 0; i < fingers.Length; i++)
        {
            Quaternion[][] q;
            if (fingers[i] != null)
            {
                switch (fingers[i].AxesChanger.part)
                {
                    case NDModelAxesChanger.Parts.thumb0:
                        q = fingers[i].RotateBone(flexValues[(int)SensorID.Thumb0]);
                        qAux[(int)SensorID.Thumb0] = q[0];
                        qAux[(int)SensorID.Thumb1] = q[1];
                        qAux[(int)SensorID.Thumb2] = q[2];
                        break;
                    case NDModelAxesChanger.Parts.index:
                        q = fingers[i].RotateBone(flexValues[(int)SensorID.Index]);
                        qAux[(int)SensorID.Index] = q[0];
                        qAux[(int)SensorID.Index1] = q[1];
                        qAux[(int)SensorID.Index2] = q[2];
                        break;
                    case NDModelAxesChanger.Parts.middle:
                        q = fingers[i].RotateBone(flexValues[(int)SensorID.Middle]);
                        qAux[(int)SensorID.Middle] = q[0];
                        qAux[(int)SensorID.Middle1] = q[1];
                        qAux[(int)SensorID.Middle2] = q[2];
                        break;
                    case NDModelAxesChanger.Parts.ring:
                        q = fingers[i].RotateBone(flexValues[(int)SensorID.Ring]);
                        qAux[(int)SensorID.Ring] = q[0];
                        qAux[(int)SensorID.Ring1] = q[1];
                        qAux[(int)SensorID.Ring2] = q[2];
                        break;
                    case NDModelAxesChanger.Parts.pinky:
                        q = fingers[i].RotateBone(flexValues[(int)SensorID.Pinky]);
                        qAux[(int)SensorID.Pinky] = q[0];
                        qAux[(int)SensorID.Pinky1] = q[1];
                        qAux[(int)SensorID.Pinky2] = q[2];
                        break;
                }
            }
        }

        return qAux;
    }

    /// <summary>
    /// Given rotation from upper parts, hand and fingers, converts input rotations to
    /// rotations that can be applied to bones, with restricted rotations applied.
    /// AvatarVR only
    /// </summary>
    /// <param name="qIn">Array of sensors rotation</param>
    /// <returns>Matrix of Quaternions, where: Quaternion[i][j],
    /// "i" represents the bone, and "j" the component Y or X</returns>
    private Quaternion[][] CalculatePhalangesLimits(Quaternion[] qIn)
    {
        Quaternion[][] qAux = new Quaternion[numberOfBones][];
        for (int i = 0; i < qAux.Length; i++)
        {
            qAux[i] = new Quaternion[2];
        }

        // Hand
        qAux[0] = handMotionSensor.RotateBone(qIn[0], true);

        // Fingers
        for (int i = 0; i < fingers.Length; i++)
        {
            Quaternion[][] q;
            if (fingers[i] != null)
            {
                switch (fingers[i].AxesChanger.part)
                {
                    case NDModelAxesChanger.Parts.thumb0:
                        q = fingers[i].RotateBone(qIn[i], qIn[i + 1]);
                        qAux[(int)SensorID.Thumb0] = q[0];
                        qAux[(int)SensorID.Thumb1] = q[1];
                        qAux[(int)SensorID.Thumb2] = q[2];
                        break;
                    case NDModelAxesChanger.Parts.index:
                        q = fingers[i].RotateBone(qIn[i], Quaternion.identity);
                        qAux[(int)SensorID.Index] = q[0];
                        qAux[(int)SensorID.Index1] = q[1];
                        qAux[(int)SensorID.Index2] = q[2];
                        break;
                    case NDModelAxesChanger.Parts.middle:
                        q = fingers[i].RotateBone(qIn[i], Quaternion.identity);
                        qAux[(int)SensorID.Middle] = q[0];
                        qAux[(int)SensorID.Middle1] = q[1];
                        qAux[(int)SensorID.Middle2] = q[2];
                        break;
                    case NDModelAxesChanger.Parts.ring:
                        q = fingers[i].RotateBone(qIn[i], Quaternion.identity);
                        qAux[(int)SensorID.Ring] = q[0];
                        qAux[(int)SensorID.Ring1] = q[1];
                        qAux[(int)SensorID.Ring2] = q[2];
                        break;
                    case NDModelAxesChanger.Parts.pinky:
                        q = fingers[i].RotateBone(qIn[i], Quaternion.identity);
                        qAux[(int)SensorID.Pinky] = q[0];
                        qAux[(int)SensorID.Pinky1] = q[1];
                        qAux[(int)SensorID.Pinky2] = q[2];
                        break;
                }
            }
        }

        // TrackBand
        if (device.trackband != NDDevice.Trackband.None)
        {
            if (upperParts != null)
            {
                for (int i = 0; i < upperParts.Length; i++)
                {
                    if (upperParts[i] != null)
                    {
                        switch (upperParts[i].AxesChanger.part)
                        {
                            case NDModelAxesChanger.Parts.forearm:
                                qAux[(int)SensorID.Forearm] = upperParts[i].RotateBone(qIn[(int)SensorID.Forearm]);
                                break;
                            case NDModelAxesChanger.Parts.arm:
                                qAux[(int)SensorID.Arm] = upperParts[i].RotateBone(qIn[(int)SensorID.Arm]);
                                break;
                            case NDModelAxesChanger.Parts.chest:
                                qAux[(int)SensorID.Chest] = upperParts[i].RotateBone(qIn[(int)SensorID.Chest]);
                                break;
                        }
                    }
                }
            }
        }

        return qAux;
    }

    /// <summary>
    /// Recalibrate the device.
    /// </summary>
    public void Recalibrate(bool forceInitialPose)
    {
        isCheckingRecalibration = false;

        if (forceInitialPose)
            SetNewCalibrationPoseWorldAngle(Vector3.zero, Vector3.zero, Vector3.zero);
        else
            SetNewCalibrationPoseWorldAngle(playerForwardCalculator.GetWorldAngle(), playerForwardCalculator.GetWorldAngle(device.GetAcceleration(), handLocation == Location.LOC_RIGHT_HAND, calibrationForearmDegreesOnUp), playerForwardCalculator.GetWorldAngle(device.GetAcceleration(), handLocation == Location.LOC_RIGHT_HAND, calibrationHandDegreesOnUp));
    }

    /// <summary>
    /// Recalibrate Gloveone finger sensors.
    /// </summary>
    public void RecalibrateFlex()
    {
        if (device.device == NDDevice.TypeOfDevice.Gloveone)
        {
            device.ResetFlex();
        }
    }

    void Update()
    {
        if (!canGetData)
            return;

        // Check how much time is the Recalibration key pressed, depending on the pressed time...
        // One shot: recalibration where the hand is pointing at
        // Pressed for 1 second: initial pose recalibration
        float pressedTime = endCalibrationTime - startCalibrationTime;
        if (Input.GetKeyDown(NDService.Instance.resetKey))
        {
            startCalibrationTime = Time.time;
            isCheckingRecalibration = true;
        }
        else if (Input.GetKeyUp(NDService.Instance.resetKey))
        {
            endCalibrationTime = Time.time;
            if (isCheckingRecalibration)
            {
                if (pressedTime <= maxTimeForCalibration)
                    Recalibrate(forceInitialPoseCalibration);
                else
                    Recalibrate(true);
            }
        }
        else if (Input.GetKey(NDService.Instance.resetKey))
        {
            endCalibrationTime = Time.time;
            if (pressedTime >= maxTimeForCalibration && isCheckingRecalibration)
            {
                Recalibrate(true);
            }
        }

        if (Input.GetKeyDown(NDService.Instance.resetFlexKey))
        {
            RecalibrateFlex();
        }

        if (useGestures)
        {
            isGesture = false;

            isGesture = isIndexPinchGesture || isMiddlePinchGesture || isThreePinchGesture || isOkGesture || isGunGesture || isPinchFistGesture;

            isIndexPinchGesture = false;
            isMiddlePinchGesture = false;
            isThreePinchGesture = false;
            isGunGesture = false;
            isOkGesture = false;
            isPinchFistGesture = false;

            if (!isGesture)
                ResetGesture();
        }
    }

    private void LateUpdate()
    {
        if (!canGetData)
            return;

        // After Update() data treatment, apply data to bones
        ApplyDataToBones();
    }

    /// <summary>
    /// This method collects data from Mux, fills the MuxData to apply and then puts rotation and position
    /// to each bone
    /// </summary>
    private void ApplyDataToBones()
    {
        for (int i = 0; i < tagControllingRotationSource.Length; i++)
        {
            if (tagControllingRotationSource[i] != null)
            {
                sourceDict.TryGetValue(tagControllingRotationSource[i], out sourceData);
                dataToApply.SetData(i, sourceData.GetSensorData(i));
            }
        }

        sourceDict.TryGetValue(tagControllingPositionSource, out sourceData);

        dataToApply.UpdateHandPosition(sourceData.GetHandPosition());
        //Vector3 newPosition = sourceData.GetHandPosition();

        if (lerpHandPosition)
        {
            if (!isAssignedInitialPosition)
            {
                if (tagControllingPositionSource != NDDevice.RAW_SOURCE_TAG)
                {
                    switch (typePositionSource)
                    {

                        case Type_MUXValueType.localSpace:
                        case Type_MUXValueType.worldSpace:
                            newPosition = handBone.position;

                            break;
                        case Type_MUXValueType.deltaLocalSpace:
                        case Type_MUXValueType.deltaWorldSpace:
                            newPosition = Vector3.zero;
                            break;
                    }

                    isAssignedInitialPosition = true;
                }
                else
                {
                    newPosition = tfHandParent.InverseTransformPoint(handBone.position);
                    isAssignedInitialPosition = true;

                    //if (handBone.parent != null)
                    //{
                    //    isAssignedInitialPosition = true;
                    //    newPosition = handBone.TransformDirection(newPosition);
                    //}
                    //else
                    //    newPosition = previousHandPosition;
                }

            }
            if (isAssignedInitialPosition)
                newPosition = Vector3.Lerp(newPosition, dataToApply.GetHandPosition(), Time.deltaTime * 30f);
            //else
            //    newPosition = previousHandPosition;

            if (Vector3.Distance(newPosition, dataToApply.GetHandPosition()) < 0.01f)
                lerpHandPosition = false;
        }
        else
        {
            newPosition = dataToApply.GetHandPosition();
        }

        switch (typePositionSource)
        {
            case Type_MUXValueType.localSpace:
                tfList[(int)SensorID.Hand].parent = tfHandParent;
                tfList[(int)SensorID.Hand].localPosition = newPosition;
                break;
            case Type_MUXValueType.worldSpace:
                tfList[(int)SensorID.Hand].parent = null;
                tfList[(int)SensorID.Hand].position = newPosition;
                break;
            case Type_MUXValueType.deltaLocalSpace:
                tfList[(int)SensorID.Hand].parent = tfHandParent;
                tfList[(int)SensorID.Hand].localPosition += newPosition;
                break;
            case Type_MUXValueType.deltaWorldSpace:
                //tfList[(int)SensorID.Hand].parent = null;
                tfList[(int)SensorID.Hand].parent = tfHandParent;
                tfList[(int)SensorID.Hand].position += newPosition;
                break;
        }

        if (useGestures && isGesture)
            CheckGesture(ref dataToApply);

        switch (typeRotationSource[0])
        {
            case Type_MUXValueType.localSpace:
                tfList[(int)SensorID.Hand].localRotation = dataToApply.GetSensorData((int)SensorID.Hand)[0];
                break;
            case Type_MUXValueType.worldSpace:
                tfList[(int)SensorID.Hand].rotation = dataToApply.GetSensorData((int)SensorID.Hand)[0];
                break;
            case Type_MUXValueType.deltaLocalSpace:
                tfList[(int)SensorID.Hand].Rotate(dataToApply.GetSensorData((int)SensorID.Hand)[0].eulerAngles, Space.Self);
                break;
            case Type_MUXValueType.deltaWorldSpace:
                tfList[(int)SensorID.Hand].Rotate(dataToApply.GetSensorData((int)SensorID.Hand)[0].eulerAngles, Space.World);
                break;
        }

        foreach (FingerModelController finger in fingers)
        {
            if (!finger)
            {
                continue;
            }

            switch (finger.AxesChanger.part)
            {
                case NDModelAxesChanger.Parts.thumb0:
                    SetBoneRotation((int)SensorID.Thumb0, (int)SensorID.Thumb1, (int)SensorID.Thumb2);
                    break;
                case NDModelAxesChanger.Parts.index:
                    if (tagControllingRotationSource[(int)SensorID.Index] != null)
                        SetBoneRotation((int)SensorID.Index, (int)SensorID.Index1, (int)SensorID.Index2);
                    break;
                case NDModelAxesChanger.Parts.middle:
                    if (tagControllingRotationSource[(int)SensorID.Middle] != null)
                        SetBoneRotation((int)SensorID.Middle, (int)SensorID.Middle1, (int)SensorID.Middle2);
                    break;
                case NDModelAxesChanger.Parts.ring:
                    if (tagControllingRotationSource[(int)SensorID.Ring] != null)
                        SetBoneRotation((int)SensorID.Ring, (int)SensorID.Ring1, (int)SensorID.Ring2);
                    break;
                case NDModelAxesChanger.Parts.pinky:
                    if (tagControllingRotationSource[(int)SensorID.Pinky] != null)
                        SetBoneRotation((int)SensorID.Pinky, (int)SensorID.Pinky1, (int)SensorID.Pinky2);
                    break;
                default:
                    break;
            }
        }

        if (device.trackband != NDDevice.Trackband.None)
        {
            if (upperParts != null)
            {
                foreach (UpperModelController upperModel in upperParts)
                {
                    switch (upperModel.AxesChanger.part)
                    {
                        case NDModelAxesChanger.Parts.forearm:
                            if (tagControllingRotationSource[(int)SensorID.Forearm] != null)
                                tfList[(int)SensorID.Forearm].rotation = dataToApply.GetSensorData((int)SensorID.Forearm)[0];
                            break;
                        case NDModelAxesChanger.Parts.arm:
                            if (tagControllingRotationSource[(int)SensorID.Arm] != null)
                                tfList[(int)SensorID.Arm].rotation = dataToApply.GetSensorData((int)SensorID.Arm)[0];
                            break;
                        case NDModelAxesChanger.Parts.chest:
                            if (tagControllingRotationSource[(int)SensorID.Chest] != null)
                                tfList[(int)SensorID.Chest].rotation = dataToApply.GetSensorData((int)SensorID.Chest)[0];
                            break;
                    }
                }
            }
        }
    }

    private void SetBoneRotation(int index0, int index1, int index2)
    {
        GetYChild(tfList[index0]).localRotation = dataToApply.GetSensorData(index0)[0];
        GetXChild(tfList[index0]).localRotation = dataToApply.GetSensorData(index0)[1];

        GetYChild(tfList[index1]).localRotation = dataToApply.GetSensorData(index1)[0];
        GetXChild(tfList[index1]).localRotation = dataToApply.GetSensorData(index1)[1];

        GetYChild(tfList[index2]).localRotation = dataToApply.GetSensorData(index2)[0];
        GetXChild(tfList[index2]).localRotation = dataToApply.GetSensorData(index2)[1];
    }

    /// <summary>
    /// Checks if gesture is being done. Gets data from gesture and apply them to fingers.
    /// If there are fingers that are not part of a gesture, apply data from dataToApply.
    /// </summary>
    /// <param name="dataToApply">MuxData where rotations are going to be stored</param>
    private void CheckGesture(ref MuxData dataToApply)
    {
        // Checks if current gesture is different from the previous one
        if (previousGesture != newGesture)
        {
            isGestureDone = false;
            previousGesture = newGesture;
        }

        // Gets initial rotation for the lerp
        for (int i = 0; i < numberOfBones; i++)
        {
            initialRot[i] = dataToApply.GetSensorData(i);
        }

        List<bool> gestureList = gesturesMuxData.GetUseThis();

        for (int i = 0; i < gestureList.Count; i++)
        {
            // Gestures lerp
            Quaternion[] q = new Quaternion[2];
            for (int j = 0; j < 2; j++)
            {
                if (gestureList[i] == true)
                {
                    if (!isGestureDone)
                    {
                        accumTimeToGesture += Time.deltaTime * multTimeGesture;
                        q[j] = Quaternion.Slerp(initialRot[i][j], gesturesMuxData.GetSensorData(i)[j], accumTimeToGesture);
                        if (accumTimeToGesture >= 1)
                        {
                            accumTimeToGesture = 0;
                            isGestureDone = true;
                        }
                    }
                    else
                    {
                        q[j] = gesturesMuxData.GetSensorData(i)[j];
                    }

                    dataToApply.SetData(i, q);
                }
            }
        }
    }

    /// <summary>
    /// Returns the X component of the Transform (X child)
    /// </summary>
    /// <param name="t">Transform to read</param>
    /// <returns>X-child</returns>
    private Transform GetXChild(Transform t)
    {
        return t.GetChild(0).GetChild(0);
    }

    /// <summary>
    /// Returns the Y component of the Transform (Y child)
    /// </summary>
    /// <param name="t">Transform to read</param>
    /// <returns>Y-child</returns>
    private Transform GetYChild(Transform t)
    {
        return t.GetChild(0);
    }

    /// <summary>
    /// Recalibrate upper parts, hand model and fingers
    /// </summary>
    public void SetInitialCalibrationPoseRotations()
    {
        device.RetrieveSensorsData();
        //handMotionSensor.Calibrate(device.rawHandData.transform.rotation);

        //ROT
        handMotionSensor.Calibrate(device.rawHandData.transform.rotation);

        foreach (FingerModelController finger in fingers)
        {
            if (!finger)
                continue;

            switch (finger.AxesChanger.part)
            {
                case NDModelAxesChanger.Parts.thumb0:
                    finger.Recalibrate(device.rawThumb0Data.transform.rotation, device.rawThumb1Data.transform.rotation);
                    break;
                case NDModelAxesChanger.Parts.index:
                    finger.Recalibrate(device.rawIndexData.transform.rotation, Quaternion.identity);
                    break;
                case NDModelAxesChanger.Parts.middle:
                    finger.Recalibrate(device.rawMiddleData.transform.rotation, Quaternion.identity);
                    break;
                case NDModelAxesChanger.Parts.ring:
                    finger.Recalibrate(device.rawRingData.transform.rotation, Quaternion.identity);
                    break;
                case NDModelAxesChanger.Parts.pinky:
                    finger.Recalibrate(device.rawPinkyData.transform.rotation, Quaternion.identity);
                    break;
                default:
                    break;
            }
        }

        if (device.trackband != NDDevice.Trackband.None)
        {
            if (upperParts != null && upperParts.Length > 0)
            {
                foreach (UpperModelController upperPart in upperParts)
                {
                    switch (upperPart.AxesChanger.part)
                    {
                        case NDModelAxesChanger.Parts.forearm:
                            upperPart.Calibrate(device.rawForearmData.transform.rotation);
                            break;
                        case NDModelAxesChanger.Parts.arm:
                            upperPart.Calibrate(device.rawArmData.transform.rotation);
                            break;
                        case NDModelAxesChanger.Parts.chest:
                            upperPart.Calibrate(device.rawChestData.transform.rotation);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// With the orientation of the body, calculate the new calibration pose
    /// </summary>
    /// <param name="poseWorldAngle"></param>
    public void SetNewCalibrationPoseWorldAngle(Vector3 poseWorldAngleArm, Vector3 poseWorldAngleForeArm, Vector3 poseWorldAngleHand)
    {
        device.isInitialValueSet = false;
        device.RetrieveSensorsData();
        //handMotionSensor.Calibrate(device.rawHandData.transform.rotation);

        //ROT
        handMotionSensor.UpdateCalibrationPose(poseWorldAngleHand);
        handMotionSensor.Calibrate(device.rawHandData.transform.rotation);

        foreach (FingerModelController finger in fingers)
        {
            if (!finger)
                continue;

            finger.ResetCalibrationPose(poseWorldAngleHand);
            switch (finger.AxesChanger.part)
            {
                case NDModelAxesChanger.Parts.thumb0:
                    finger.Recalibrate(device.rawThumb0Data.transform.rotation, device.rawThumb1Data.transform.rotation);
                    break;
                case NDModelAxesChanger.Parts.index:
                    finger.Recalibrate(device.rawIndexData.transform.rotation, Quaternion.identity);
                    break;
                case NDModelAxesChanger.Parts.middle:
                    finger.Recalibrate(device.rawMiddleData.transform.rotation, Quaternion.identity);
                    break;
                case NDModelAxesChanger.Parts.ring:
                    finger.Recalibrate(device.rawRingData.transform.rotation, Quaternion.identity);
                    break;
                case NDModelAxesChanger.Parts.pinky:
                    finger.Recalibrate(device.rawPinkyData.transform.rotation, Quaternion.identity);
                    break;
                default:
                    break;
            }
        }

        if (device.trackband != NDDevice.Trackband.None)
        {
            if (upperParts != null && upperParts.Length > 0)
            {
                foreach (UpperModelController upperPart in upperParts)
                {

                    switch (upperPart.AxesChanger.part)
                    {
                        case NDModelAxesChanger.Parts.forearm:
                            if (calibrateForearmUsingZCorrection)
                                upperPart.ResetCalibrationPose(poseWorldAngleForeArm);
                            else
                                upperPart.ResetCalibrationPose(poseWorldAngleArm);

                            upperPart.Calibrate(device.rawForearmData.transform.rotation);
                            break;
                        case NDModelAxesChanger.Parts.arm:
                            upperPart.ResetCalibrationPose(poseWorldAngleArm);
                            upperPart.Calibrate(device.rawArmData.transform.rotation);
                            break;
                        case NDModelAxesChanger.Parts.chest:
                            upperPart.ResetCalibrationPose(poseWorldAngleArm);
                            upperPart.Calibrate(device.rawChestData.transform.rotation);
                            break;
                    }
                }
            }
        }

        // Trigger reset notifications
        if (avatarResetTrigger)
            avatarResetTrigger.TriggerEvent();
    }

    public int CompareTo(HandModelController other)
    {
        if (this.user > other.user)
        {
            return 1;
        }
        else if (this.user < other.user)
        {
            return -1;
        }
        else
        {
            if (this.handLocation == Location.LOC_RIGHT_HAND && other.handLocation == Location.LOC_LEFT_HAND)
            {
                return -1;
            }
            else if (this.handLocation == Location.LOC_LEFT_HAND && other.handLocation == Location.LOC_RIGHT_HAND)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    #region Contacts and gestures

    /// <summary>
    /// Updates mux gesture registry with index pinch gesture
    /// </summary>
    public void IndexPinch()
    {
        if (useGestures)
        {
            if (handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index2] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb0] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb1] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb2])
            {
                gesturesMuxData.SetFingersMask((int)SensorID.Index, (int)SensorID.Index1, (int)SensorID.Index2,
                                (int)SensorID.Thumb0, (int)SensorID.Thumb1, (int)SensorID.Thumb2);
            }

            gesturesMuxData.FillSensorsData(indexPinchGesture.GetQuaternions());
        }
    }

    /// <summary>
    /// Updates mux gesture registry with middle pinch gesture
    /// </summary>
    public void MiddlePinch()
    {
        if (useGestures)
        {
            if (handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle2] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb0] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb1] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb2])
            {
                gesturesMuxData.SetFingersMask((int)SensorID.Middle, (int)SensorID.Middle1, (int)SensorID.Middle2,
                            (int)SensorID.Thumb0, (int)SensorID.Thumb1, (int)SensorID.Thumb2);
            }

            gesturesMuxData.FillSensorsData(middlePinchGesture.GetQuaternions());
        }
    }

    /// <summary>
    /// Updates mux gesture registry with three pinch gesture
    /// </summary>
    public void ThreePinch()
    {
        if (useGestures)
        {
            if (handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index2] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle1] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle2] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb0] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb2])
            {
                gesturesMuxData.SetFingersMask((int)SensorID.Index, (int)SensorID.Index1, (int)SensorID.Index2,
                                                (int)SensorID.Middle, (int)SensorID.Middle1, (int)SensorID.Middle2,
                                                (int)SensorID.Thumb0, (int)SensorID.Thumb1, (int)SensorID.Thumb2);
            }

            gesturesMuxData.FillSensorsData(threePinchGesture.GetQuaternions());
        }
    }

    /// Updates mux gesture registry with gun gesture
    public void Gun()
    {
        if (useGestures)
        {
            if (handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle2] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Ring] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Ring1] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Ring2] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Pinky] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Pinky1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Pinky2])
            {
                gesturesMuxData.SetFingersMask((int)SensorID.Middle, (int)SensorID.Middle1, (int)SensorID.Middle2,
                                                (int)SensorID.Ring, (int)SensorID.Ring1, (int)SensorID.Ring2,
                                                (int)SensorID.Pinky, (int)SensorID.Pinky1, (int)SensorID.Pinky2);
            }

            gesturesMuxData.FillSensorsData(gunGesture.GetQuaternions());
        }
    }

    /// Updates mux gesture registry with OK gesture
    public void Ok()
    {
        if (useGestures)
        {
            if (handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index2] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle1] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle2] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Ring] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Ring1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Ring2] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Pinky] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Pinky1] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Pinky2])
            {
                gesturesMuxData.SetFingersMask((int)SensorID.Index, (int)SensorID.Index1, (int)SensorID.Index2,
                                                (int)SensorID.Middle, (int)SensorID.Middle1, (int)SensorID.Middle2,
                                                (int)SensorID.Ring, (int)SensorID.Ring1, (int)SensorID.Ring2,
                                                (int)SensorID.Pinky, (int)SensorID.Pinky1, (int)SensorID.Pinky2);
            }

            gesturesMuxData.FillSensorsData(okGesture.GetQuaternions());
        }
    }

    /// Updates mux gesture registry with pinch fist gesture
    public void PinchFist()
    {
        if (useGestures)
        {
            if (handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Index2] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle1] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Middle2] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Ring] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Ring1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Ring2] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Pinky] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Pinky1] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Pinky2] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb0] && handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb1] &&
                handMuxDataProvider.bonesToControlRotation[(int)SensorID.Thumb2])
            {
                gesturesMuxData.SetFingersMask((int)SensorID.Thumb0, (int)SensorID.Thumb1, (int)SensorID.Thumb2,
                                                (int)SensorID.Index, (int)SensorID.Index1, (int)SensorID.Index2,
                                                (int)SensorID.Middle, (int)SensorID.Middle1, (int)SensorID.Middle2,
                                                (int)SensorID.Ring, (int)SensorID.Ring1, (int)SensorID.Ring2,
                                                (int)SensorID.Pinky, (int)SensorID.Pinky1, (int)SensorID.Pinky2);
            }

            gesturesMuxData.FillSensorsData(pinchFistGesture.GetQuaternions());
        }
    }

    /// <summary>
    /// Reset gesture to none
    /// </summary>
    public void ResetGesture()
    {
        gesturesMuxData.ResetFingersMask();
        isGestureDone = false;
        newGesture = Gesture.None;
    }

    /// <summary>
    /// Enables contacts and events
    /// </summary>
    private void EnableContacts()
    {
        device.isPinchGestureEvent += new NDDevice.isPinchGestureHandler(IndexPinchGesture);
        device.isMiddlePinchGestureEvent += new NDDevice.isMiddlePinchGestureHandler(MiddlePinchGesture);
        device.isThreePinchGestureEvent += new NDDevice.isThreePinchGestureHandler(ThreePinchGesture);
        device.isGunGestureEvent += new NDDevice.isGunGestureHandler(GunGesture);
        device.isOkGestureEvent += new NDDevice.isOkGestureHandler(OKGesture);
        device.isPinchFistGestureEvent += new NDDevice.isPinchFistGestureHandler(PinchFistGesture);
    }

    /// <summary>
    /// Establish index pinch gesture as current gesture
    /// </summary>
    public void IndexPinchGesture()
    {
        isIndexPinchGesture = true;

        gesturesMuxData.ResetFingersMask();
        newGesture = Gesture.IndexPinchGesture;
        IndexPinch();
    }

    /// <summary>
    /// Establish middle pinch gesture as current gesture
    /// </summary>
    public void MiddlePinchGesture()
    {
        isMiddlePinchGesture = true;

        gesturesMuxData.ResetFingersMask();
        newGesture = Gesture.MiddlePinchGesture;
        MiddlePinch();
    }

    /// <summary>
    /// Establish three pinch gesture as current gesture
    /// </summary>
    public void ThreePinchGesture()
    {
        isThreePinchGesture = true;

        gesturesMuxData.ResetFingersMask();
        newGesture = Gesture.ThreePinchGesture;
        ThreePinch();
    }

    /// <summary>
    /// Establish gun gesture as current gesture
    /// </summary>
    public void GunGesture()
    {
        isGunGesture = true;

        gesturesMuxData.ResetFingersMask();
        newGesture = Gesture.GunGesture;
        Gun();
    }

    /// <summary>
    /// Establish OK gesture as current gesture
    /// </summary>
    public void OKGesture()
    {
        isOkGesture = true;

        gesturesMuxData.ResetFingersMask();
        newGesture = Gesture.OkGesture;
        Ok();
    }

    /// <summary>
    /// Establish pinch fist gesture as current gesture
    /// </summary>
    public void PinchFistGesture()
    {
        isPinchFistGesture = true;

        gesturesMuxData.ResetFingersMask();
        newGesture = Gesture.PinchFistGesture;
        PinchFist();
    }

    #endregion
}

