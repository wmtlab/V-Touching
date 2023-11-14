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

/// <summary>
/// Play an animation previously recorded with NDAnimationRecorder.
/// </summary>
[RequireComponent(typeof(Animation))]
public class NDAnimationPlayer : MonoBehaviour
{
    /// <summary>
    /// Animation where it is going to be stored
    /// </summary>
    public Animation anim;

    // Time in seconds since the animation starts playing
    private float currentTime;

    public delegate void PlayAnimationHandler();
    public event PlayAnimationHandler PlayAnimationEvent;
    public delegate void StopAnimationHandler();
    public event StopAnimationHandler StopAnimationEvent;

    void Start()
    {
        currentTime = 0f;
    }

    void Update()
    {
        // If the animation clip is playing, increases currentTime
        if (anim != null && anim.isPlaying)
            currentTime += Time.deltaTime;

        // Checks if the animation is over
        IsAnimationFinished();
    }

    /// <summary>
    /// Destroy all unnecessary components attached to this game object and its children
    /// </summary>
    public void CleanGameObjects()
    {
        Component[] components = GetComponentsInChildren<Component>();
        List<Component> secondCleaning = new List<Component>();

        foreach (Component component in components)
        {
            if (component.GetType() != typeof(Transform) && component.GetType() != typeof(SkinnedMeshRenderer) &&
                component.GetType() != typeof(NDAnimationPlayer) && component.GetType() != typeof(Animation))
            {
                DestroyImmediate(component);
                if (component != null)
                {
                    secondCleaning.Add(component);
                }
            }
        }

        // Second cleaning
        foreach (Component component in secondCleaning)
        {
            if (component.GetType() != typeof(Transform) && component.GetType() != typeof(SkinnedMeshRenderer) &&
                component.GetType() != typeof(NDAnimationPlayer) && component.GetType() != typeof(Animation))
            {
                Debug.Log(component + " added for the second cleaning. Trying to delete it again.");
                DestroyImmediate(component);
            }
        }
    }

    /// <summary>
    /// This method searchs for all the NDModelAxesChanger to create the hierarchy
    /// </summary>
    public void SetUpBones()
    {
        foreach (NDModelAxesChanger axesChanger in GetComponentsInChildren<NDModelAxesChanger>())
        {
            axesChanger.HierarchySetup();

            // Try to get the HandModelController attached
            HandModelController hmc = axesChanger.GetComponent<HandModelController>();
            // If the object has a HandModelController attached, changes the game object name depending on HandModelController user
            if (hmc)
            {
                hmc.name = hmc.name + "_P" + hmc.user;
            }
        }
    }

    /// <summary>
    /// Starts playing the animation
    /// </summary>
    public void PlayAnimation()
    {
        if (anim == null)
            return;

        if (!anim.isPlaying)
        {
            anim.Play();
            currentTime = 0f;
            if (PlayAnimationEvent != null)
                PlayAnimationEvent();
        }
    }

    /// <summary>
    /// Stops playing the animation
    /// </summary>
    public void StopAnimation()
    {
        if (anim == null)
            return;

        if (anim.isPlaying)
        {
            anim.Stop();
            if (StopAnimationEvent != null)
                StopAnimationEvent();
        }
    }

    /// <summary>
    /// Determines if the animation clip is over
    /// </summary>
    /// <returns>True if the animation is over. False if the animation wrapMode is not Once or Default or the animation is not over.</returns>
    public bool IsAnimationFinished()
    {
        if (anim == null)
            return false;

        if (anim.wrapMode == WrapMode.Once || anim.wrapMode == WrapMode.Default)
        {
            if (anim.clip != null && anim.clip.length <= currentTime)
            {
                if (StopAnimationEvent != null)
                    StopAnimationEvent();
                return true;
            }
        }

        return false;
    }
}
