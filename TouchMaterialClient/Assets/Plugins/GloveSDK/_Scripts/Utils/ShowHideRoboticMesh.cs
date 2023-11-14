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
using IK;
using NDAPIUnity;
using NDAPIWrapperSpace;

/// <summary>
/// Show/Hide mesh. Depending on the device connected, it is going to choose what will
/// be visible. Meshes has to be assigned in Editor.
/// Z key: Show/hide all meshes
/// X key: Show/hide current hand
/// C key: Show/hide chest mesh
/// </summary>
public class ShowHideRoboticMesh : MonoBehaviour
{
    /// <summary>
    /// Right arm mesh.
    /// </summary>
    public SkinnedMeshRenderer rightArmMesh;
    /// <summary>
    /// Left arm mesh.
    /// </summary>
    public SkinnedMeshRenderer leftArmMesh;
    /// <summary>
    /// Right IK arm mesh.
    /// </summary>
    public SkinnedMeshRenderer rightIKArmMesh;
    /// <summary>
    /// Left IK arm mesh.
    /// </summary>
    public SkinnedMeshRenderer leftIKArmMesh;
    /// <summary>
    /// Chest mesh.
    /// </summary>
    public SkinnedMeshRenderer chest;
    /// <summary>
    /// Makes the chest mesh visible.
    /// </summary>
    public bool showChest;
    /// <summary>
    /// Makes the active arm meshes visible.
    /// </summary>
    public bool showActiveArms;
    /// <summary>
    /// Starts the scene with meshes visible.
    /// </summary>
    public bool showActiveHands;
    /// <summary>
    /// The right hand mesh.
    /// </summary>
    public SkinnedMeshRenderer rightHandMesh;
    /// <summary>
    /// The left hand mesh.
    /// </summary>
    public SkinnedMeshRenderer leftHandMesh;
    /// <summary>
    /// Left hand Transform.
    /// </summary>
    public Transform leftHand;
    /// <summary>
    /// Right hand Transform.
    /// </summary>
    public Transform rightHand;

    private bool isRight, isLeft;

    void Start()
    {
        HapticController hc;
        NDController.GetController(NDAPIWrapperSpace.Location.LOC_LEFT_HAND, out hc);
        if (hc != null)
        {
            isLeft = true;
        }
        else
        {   //Disable all colliders of that hand
            Collider[] cols = leftHand.GetComponentsInChildren<Collider>();
            foreach (Collider col in cols)
            {
                col.enabled = false;
            }
        }

        NDController.GetController(NDAPIWrapperSpace.Location.LOC_RIGHT_HAND, out hc);
        if (hc != null)
            isRight = true;
        else
        {   //Disable all colliders of that hand
            Collider[] cols = rightHand.GetComponentsInChildren<Collider>();

            foreach (Collider col in cols)
            {
                col.enabled = false;
            }
        }

        // Check IK
        IKControl IKControl_inScene = GameObject.FindObjectOfType<IKControl>();
        if (IKControl_inScene)
        {
            leftArmMesh.gameObject.SetActive(false);
            rightArmMesh.gameObject.SetActive(false);
        }

        //initial chest config
        chest.enabled = showChest;

        IKController ik_controller = GameObject.FindObjectOfType<IKController>();

        if (!isLeft)
        {
            leftArmMesh.enabled = false;
            leftHandMesh.enabled = false;

            if (rightIKArmMesh)
                leftIKArmMesh.enabled = false;
        }

        if (!isRight)
        {
            rightArmMesh.enabled = false;
            rightHandMesh.enabled = false;

            if (rightIKArmMesh)
                rightIKArmMesh.enabled = false;
        }

        if (!showActiveHands)
        {
            leftHandMesh.enabled = false;
            rightHandMesh.enabled = false;
        }

        if (!showActiveArms)
        {
            leftArmMesh.enabled = false;
            rightArmMesh.enabled = false;

            if (leftIKArmMesh)
                leftIKArmMesh.enabled = false;
            if (rightIKArmMesh)
                rightIKArmMesh.enabled = false;
        }
        else
        {
            if ((ik_controller && ik_controller.useIK) && leftIKArmMesh && leftIKArmMesh.gameObject.activeSelf == false)
                leftIKArmMesh.gameObject.SetActive(true);
            if ((ik_controller && ik_controller.useIK) && rightIKArmMesh && rightIKArmMesh.gameObject.activeSelf == false)
                rightIKArmMesh.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // Z key is used to show/hide all hand/arm/chest meshes
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ShowHideAllMeshes();
        }
        // X key is used to show/hide current active hands
        if (Input.GetKeyDown(KeyCode.X))
        {
            ShowHideActiveHands();
        }
        // C key is used to show/hide chest mesh
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShowHideChestMesh();
        }
    }

    /// <summary>
    /// Given a location (left/right), show that hand mesh
    /// </summary>
    /// <param name="handLocation">Location (left/right)</param>
    public void ShowHandByLocation(Location handLocation)
    {
        if (handLocation == Location.LOC_RIGHT_HAND)
        {
            if (isRight)
                rightHandMesh.enabled = true;
        }
        else if (handLocation == Location.LOC_LEFT_HAND)
        {
            if (isLeft)
                leftHandMesh.enabled = true;
        }
    }

    /// <summary>
    /// Given a location (left/right), hide mesh of that hand
    /// </summary>
    /// <param name="handLocation">Location (left/right)</param>
    public void HideHandByLocation(Location handLocation)
    {
        if (handLocation == Location.LOC_RIGHT_HAND)
        {
            if (isRight)
                rightHandMesh.enabled = false;
        }
        else if (handLocation == Location.LOC_LEFT_HAND)
        {
            if (isLeft)
                leftHandMesh.enabled = false;
        }
    }

    /// <summary>
    /// Given a location (left/right), show mesh of that arm
    /// </summary>
    /// <param name="handLocation">Location (left/right)</param>
    public void ShowArmByLocation(Location handLocation)
    {
        if (handLocation == Location.LOC_RIGHT_HAND)
        {
            if (isRight)
            {
                rightArmMesh.enabled = true;
                if (rightIKArmMesh)
                    rightIKArmMesh.enabled = true;
            }
        }
        else if (handLocation == Location.LOC_LEFT_HAND)
        {
            if (isLeft)
            {
                leftArmMesh.enabled = true;
                if (leftIKArmMesh)
                    leftIKArmMesh.enabled = true;
            }
        }
    }

    /// <summary>
    /// Given a location (left/right), hide mesh of that arm
    /// </summary>
    /// <param name="handLocation">Location (left/right)</param>
    public void HideArmByLocation(Location handLocation)
    {
        if (handLocation == Location.LOC_RIGHT_HAND)
        {
            if (isRight)
            {
                rightArmMesh.enabled = false;
                if (rightIKArmMesh)
                    rightIKArmMesh.enabled = false;
            }
        }
        else if (handLocation == Location.LOC_LEFT_HAND)
        {
            if (isLeft)
            {
                leftArmMesh.enabled = false;
                if (leftIKArmMesh)
                    leftIKArmMesh.enabled = false;
            }
        }
    }

    /// <summary>
    /// Show/Hide chest mesh.
    /// </summary>
    public void ShowHideChestMesh()
    {
        showChest = !showChest;
        chest.enabled = showChest;
    }

    /// <summary>
    /// Show/Hide meshes of active arms
    /// </summary>
    public void ShowHideAllMeshes()
    {
        showActiveHands = !showActiveHands;

        if (isRight)
        {
            if (showActiveHands)
            {
                showActiveArms = true;
                showChest = true;
            }
            else
            {
                showActiveArms = false;
                showChest = false;
            }

            rightArmMesh.enabled = showActiveArms;
            rightHandMesh.enabled = showActiveHands;

            if (rightIKArmMesh)
                rightIKArmMesh.enabled = showActiveArms;
        }

        if (isLeft)
        {
            if (showActiveHands)
            {
                showActiveArms = true;
                showChest = true;
            }
            else
            {
                showActiveArms = false;
                showChest = false;
            }

            leftArmMesh.enabled = showActiveArms;
            leftHandMesh.enabled = showActiveHands;

            if (leftIKArmMesh)
                leftIKArmMesh.enabled = showActiveArms;
        }

        chest.enabled = showChest;
    }

    /// <summary>
    /// Show/Hide meshes of active hands.
    /// </summary>
    public void ShowHideActiveHands()
    {
        showActiveHands = !showActiveHands;

        if (isRight)
        {
            rightHandMesh.enabled = showActiveHands;
        }

        if (isLeft)
        {
            leftHandMesh.enabled = showActiveHands;
        }
    }

    /// <summary>
    /// Show chest mesh, if it is not visible
    /// </summary>
    public void ShowChest()
    {
        showChest = true;
        chest.enabled = showChest;
    }

    /// <summary>
    /// Hide chest mesh, if it is visible
    /// </summary>
    public void HideChest()
    {
        showChest = false;
        chest.enabled = showChest;
    }

    /// <summary>
    /// Show active arms meshes, if they are not visible
    /// </summary>
    public void ShowActiveArms()
    {
        showActiveArms = true;

        if (isRight)
        {
            rightArmMesh.enabled = showActiveArms;
            if (rightIKArmMesh)
                rightIKArmMesh.enabled = showActiveArms;
        }

        if (isLeft)
        {
            leftArmMesh.enabled = showActiveArms;
            if (leftIKArmMesh)
                leftIKArmMesh.enabled = showActiveArms;
        }
    }

    /// <summary>
    /// Hide active arms meshes, if they are visible
    /// </summary>
    public void HideActiveArms()
    {
        showActiveArms = false;

        if (isRight)
        {
            rightArmMesh.enabled = showActiveArms;
            if (rightIKArmMesh)
                rightIKArmMesh.enabled = showActiveArms;
        }

        if (isLeft)
        {
            leftArmMesh.enabled = showActiveArms;
            if (leftIKArmMesh)
                leftIKArmMesh.enabled = showActiveArms;

        }
    }

    /// <summary>
    /// Show active hands meshes, if they are not visible
    /// </summary>
    public void ShowActiveHands()
    {
        showActiveHands = true;

        if (isRight)
        {
            rightHandMesh.enabled = showActiveHands;
        }

        if (isLeft)
        {
            leftHandMesh.enabled = showActiveHands;
        }
    }

    /// <summary>
    /// Hide active hands meshes, if they are visible
    /// </summary>
    public void HideActiveHands()
    {
        showActiveHands = false;

        if (isRight)
        {
            rightHandMesh.enabled = showActiveHands;
        }

        if (isLeft)
        {
            leftHandMesh.enabled = showActiveHands;
        }
    }
}
