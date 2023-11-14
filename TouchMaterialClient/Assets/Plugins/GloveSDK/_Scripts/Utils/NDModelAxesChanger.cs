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
/// Create the needed hierarchy to change the orientation of the Transform. Our devices need to have the Z-axis looking forward. If the Transform
/// has any other axis looking forward, this script creates the necessary Transforms and rotations in order to have a Transform with the Z-axis looking forward.
/// </summary>
public class NDModelAxesChanger : MonoBehaviour
{
    /// <summary>
    /// Enumeration of possible axes.
    /// </summary>
    public enum Axes
    {
        posX,
        negX,
        posY,
        negY,
        posZ,
        negZ
    }

    /// <summary>
    /// Parts that are prone to be changed.
    /// </summary>
    public enum Parts
    {
        chest = 7,
        arm = 8,
        forearm = 9,
        hand = 0,
        thumb0 = 1,
        thumb1 = 2,
        index = 3,
        middle = 4,
        ring = 5,
        pinky = 6
    }

    public enum Phalanges
    {
        none = -1,
        proximal = 0,
        intermediate = 1,
        distal = 2
    }

    [Tooltip("This axis is located at the right side of the chest. Using TB3 (if applicable) at the right hand")]
    /// <summary>
    /// The forward and up axes of the IMU. Check online documentation for further information about these properties.
    /// </summary>
    public Axes forward, up;
    /// <summary>
    /// Part of the body.
    /// </summary>
    public Parts part;
    /// <summary>
    /// Aligner GameObject. This GO has the desired orientation after the correction and being child of the Y and X components
    /// </summary>
    [HideInInspector]
    public GameObject goAligner;
    /// <summary>
    /// Corrector GameObject. This GO has the desired orientation and is the parent of all the new hierarchy
    /// </summary>
    private GameObject goCorrector;
    private GameObject goImu;
    private GameObject goImuX;
    private GameObject goImuY;
    /// <summary>
    /// Determines if Chest has been assigned
    /// </summary>
    private bool isChestSet = false;
    /// <summary>
    /// If true, the hierarchy of sensor Y and X components are going to be created
    /// </summary>
    [HideInInspector]
    public bool createIMUTransforms;

    /// <summary>
    /// Bone index. This is needed for phalanges. (0 = proximal, 1 = intermediate, 2 = distal)
    /// </summary>
    public Phalanges boneIndex;

    /// <summary>
    /// Set to <c>False</c> if it is going to be stored the gesture; <c>True</c> for a normal use, setting aligners.
    /// </summary>
    public bool setAligner;

    /// <summary>
    /// Create GameObjects that change the orientation of the bone. These new GameObjects are parents of the processed bone (Y and X)
    /// </summary>
    void Awake()
    {
        createIMUTransforms = true;

        CreateAlignerGizmo();

        // Corrector creation
        goCorrector = new GameObject("Corrector_" + this.name);
        goCorrector.transform.SetParent(goAligner.transform);
        goCorrector.transform.localPosition = Vector3.zero;
        goCorrector.transform.localRotation = Quaternion.identity;

        if (createIMUTransforms)
        {

            // IMU creation
            goImu = new GameObject("IMU_" + this.name);
            goImu.transform.SetParent(goCorrector.transform);
            goImu.transform.localPosition = Vector3.zero;
            goImu.transform.localRotation = Quaternion.identity;

            // Corrector as script's child
            goCorrector.transform.SetParent(this.transform.parent);

            // Y IMU component
            goImuY = new GameObject("IMUY_" + this.name);
            goImuY.transform.SetParent(goImu.transform);
            goImuY.transform.localPosition = Vector3.zero;
            goImuY.transform.localRotation = Quaternion.identity;

            // X IMU component
            goImuX = new GameObject("IMUX_" + this.name);
            goImuX.transform.SetParent(goImuY.transform);
            goImuX.transform.localPosition = Vector3.zero;
            goImuX.transform.localRotation = Quaternion.identity;

            // Bone as IMU's child
            this.transform.SetParent(goImuX.transform);
        }
        else
        {
            // Corrector as script's child
            goCorrector.transform.SetParent(this.transform.parent);
            this.transform.SetParent(goCorrector.transform);
        }

        if (!setAligner)
        {
            GameObject.Destroy(goAligner);
        }
    }

    /// <summary>
    /// Returns the aligner rotation of the bone if it has; Quaternion.identity if not
    /// </summary>
    /// <returns>Aligner rotation</returns>
    public Quaternion GetAlignerRotation()
    {
        if (setAligner)
        {
            return goAligner.transform.rotation;
        }
        else
            return Quaternion.identity;
    }

    /// <summary>
    /// Calling this method, chest is marked as set
    /// </summary>
    /// <param name="value">If true, chest is marked as set</param>
    public void SetChest(bool value)
    {
        isChestSet = value;
    }

    /// <summary>
    /// Checks if chest has been assigned
    /// </summary>
    /// <returns>True if chest has been assiged; false otherwise</returns>
    public bool IsChestAssigned()
    {
        return isChestSet;
    }

    /// <summary>
    /// Chest aligner is placed initially at the right side; calling this method, if the user
    /// has a left AvatarVR with TB3 plugged, is needed to change the orientation of this aligner
    /// </summary>
    public void CorrectLeftChestAligner()
    {
        goAligner.transform.Rotate(goAligner.transform.right, 180f);
    }

    /// <summary>
    /// Returns the associated Transform to the bone
    /// </summary>
    /// <returns>Transform where sensor data is going to be applied</returns>
    public Transform GetDataTrasform()
    {
        if (createIMUTransforms)
            return goImu.transform;
        else
            return transform;
    }

    private void CreateAlignerGizmo()
    {
        Quaternion q = Quaternion.LookRotation(GetAxis(forward, this.transform), GetAxis(up, this.transform));
        goAligner = new GameObject("Aligner" + this.name);

        goAligner.transform.SetParent(this.transform);
        goAligner.transform.localPosition = Vector3.zero;
        goAligner.transform.rotation = q;
    }

    private Vector3 GetAxis(Axes param, Transform obj)
    {
        switch (param)
        {
            case Axes.negX:
                return -obj.right;
            case Axes.posX:
                return obj.right;
            case Axes.negY:
                return -obj.up;
            case Axes.posY:
                return obj.up;
            case Axes.negZ:
                return -obj.forward;
            case Axes.posZ:
                return obj.forward;
            default:
                return Vector3.zero;
        }
    }

    private Axes GetOppositeAxis(Axes param)
    {
        switch (param)
        {
            case Axes.negX:
                return Axes.posX;
            case Axes.posX:
                return Axes.negX;
            case Axes.negY:
                return Axes.posY;
            case Axes.posY:
                return Axes.negY;
            case Axes.negZ:
                return Axes.posZ;
            case Axes.posZ:
                return Axes.negZ;
            default:
                return param;
        }
    }

    /// <summary>
    /// This method performs the necessary changes to bones in order to set up the model hierarchy correctly.
    /// </summary>
    /// <returns></returns>
    public Transform HierarchySetup()
    {
        setAligner = false;

        CreateAlignerGizmo();

        // Corrector creation
        goCorrector = new GameObject("Corrector_" + this.name);
        goCorrector.transform.SetParent(goAligner.transform);
        goCorrector.transform.localPosition = Vector3.zero;
        goCorrector.transform.localRotation = Quaternion.identity;

        // IMU creation
        goImu = new GameObject("IMU_" + this.name);
        goImu.transform.SetParent(goCorrector.transform);
        goImu.transform.localPosition = Vector3.zero;
        goImu.transform.localRotation = Quaternion.identity;

        Vector3 imuEuler = goImu.transform.localEulerAngles;

        // Y IMU component
        goImuY = new GameObject("IMUY_" + this.name);
        goImuY.transform.SetParent(goImu.transform);
        goImuY.transform.localPosition = Vector3.zero;
        goImuY.transform.localRotation = Quaternion.Euler(new Vector3(0f, imuEuler.y, 0f));

        // X IMU component
        goImuX = new GameObject("IMUX_" + this.name);
        goImuX.transform.SetParent(goImuY.transform);
        goImuX.transform.localPosition = Vector3.zero;
        goImuX.transform.localRotation = Quaternion.Euler(new Vector3(imuEuler.x, 0f, 0f));

        // Corrector as script's child
        goCorrector.transform.SetParent(this.transform.parent);

        // Bone as IMU's child
        this.transform.SetParent(goImuX.transform);

        return goImu.transform;
    }
}
