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

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

/// <summary>
/// Record movements of the avatar and save it in a animation
/// </summary>
public class NDAnimationRecorder : MonoBehaviour
{
#if UNITY_EDITOR
    /// <summary>
    /// The animation clip where NDAnimationRecorder is going to save the animation
    /// </summary>
    private AnimationClip clip;
    /// <summary>
    /// The name for the recorded animation clip  
    /// </summary>
    public string clipName;

    private readonly string path = "Assets/_Animation/Records/";

    // The GameObjectRecorder will record all the changing properties and will save those changes into the specified animation clip
    private GameObjectRecorder recorder;
    // Is this NDRecorder recording?
    private bool recordClip;

    public delegate void StartRecordingHandler();
    public event StartRecordingHandler StartRecordingEvent;
    public delegate void StopRecordingHandler();
    public event StopRecordingHandler StopRecordingEvent;

    void Start()
    {
        // Initializes the recorder
        SetUpRecorder();
    }

    /// <summary>
    /// This method initializes the GameObjectRecorder
    /// </summary>
    void SetUpRecorder()
    {
        // Sets recordClip to false
        recordClip = false;
        // Creates a new GameObjectRecorder
        recorder = new GameObjectRecorder(gameObject);
        // The property we want to bind is the Transform from this game object and its children
        recorder.BindComponentsOfType<Transform>(gameObject, true);
        // Creates a new clip
        clip = new AnimationClip();
    }

    void LateUpdate()
    {
        if (clip == null)
            return;

        // If this is recording, the GameObjectRecorder stores the values of the Transform component
        if (recordClip)
        {
            recorder.TakeSnapshot(Time.deltaTime);
        }
    }

    /// <summary>
    /// Is the NDAnimationRecorder recording the animation?
    /// </summary>
    /// <returns></returns>
    public bool IsRecording()
    {
        return recordClip;
    }

    /// <summary>
    /// Starts recording the animation
    /// </summary>
    public void StartRecording()
    {
        if (clip == null)
            return;

        if (!recorder.isRecording)
        {
            // Initializes the recorder
            SetUpRecorder();
            // Sets recordClip to true to start recording
            recordClip = true;
            // Calls the StartRecordingEvent
            if (StartRecordingEvent != null)
                StartRecordingEvent();
        }
    }

    /// <summary>
    /// Stops recording the animation and saves it into the specified animation clip
    /// </summary>
    public void StopRecording()
    {
        if (clip == null)
            return;

        if (recorder.isRecording)
        {
            // Sets recordClip to false in order to stop recording
            recordClip = false;
            // Saves the animation clip with the stored values
            SaveClip();
            // Calls the StopRecordingEvent
            if (StopRecordingEvent != null)
                StopRecordingEvent();
        }
    }

    void OnDisable()
    {
        // Stops recording when this script is disabled
        if (recorder)
            StopRecording();
    }

    void OnDestroy()
    {
        // Stops recording when this script is destroyed
        if (recorder)
            StopRecording();
    }

    /// <summary>
    /// Saves the recorded animation into a clip with the specified name
    /// </summary>
    void SaveClip()
    {
        // Saves all the stored values into the animation clip
        recorder.SaveToClip(clip);
        // Resets the recorder stored values
        recorder.ResetRecording();
        // The clip will use the Legacy Animation system
        clip.legacy = true;
        // Creates a new folder if the Records folder does not exists
        if (!AssetDatabase.IsValidFolder("Assets/_Animation/Records"))
            AssetDatabase.CreateFolder("Assets/_Animation", "Records");
        // Creates an unique path for the animation clip
        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path + clipName + ".anim");
        // Creates the clip in the _Animation/Records folder
        AssetDatabase.CreateAsset(clip, uniquePath);
    }
#else
    void Start() {
        Destroy(this);
    }
#endif
}
