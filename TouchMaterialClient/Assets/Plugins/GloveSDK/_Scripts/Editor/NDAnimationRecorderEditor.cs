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
using UnityEditor;

[CustomEditor(typeof(NDAnimationRecorder))]
public class NDAnimationRecorderEditor : Editor
{
    private NDAnimationRecorder Target { get { return (NDAnimationRecorder)target; } }

    private void OnEnable()
    {
        if (Target)
        {
            Target.StartRecordingEvent += Repaint;
            Target.StopRecordingEvent += Repaint;
        }
    }

    private void OnDisable()
    {
        if (Target)
        {
            Target.StartRecordingEvent -= Repaint;
            Target.StopRecordingEvent -= Repaint;
        }
    }

    private void OnDestroy()
    {
        if (Target)
        {
            Target.StartRecordingEvent -= Repaint;
            Target.StopRecordingEvent -= Repaint;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        NDAnimationRecorder animRecorder = (NDAnimationRecorder)target;

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start recording"))
        {
            if (EditorApplication.isPlaying)
                animRecorder.StartRecording();
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Stop recording"))
        {
            if (EditorApplication.isPlaying)
                animRecorder.StopRecording();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        if (animRecorder.IsRecording())
            EditorGUILayout.HelpBox("RECORDING", MessageType.Warning);
        else
            EditorGUILayout.HelpBox("STOPPED", MessageType.Info);
    }
}
