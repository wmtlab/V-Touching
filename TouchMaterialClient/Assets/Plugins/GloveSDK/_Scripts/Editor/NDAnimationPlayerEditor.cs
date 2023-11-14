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

[CustomEditor(typeof(NDAnimationPlayer))]
public class NDAnimationPlayerEditor : Editor
{
    private NDAnimationPlayer Target { get { return (NDAnimationPlayer)target; } }

    private void OnEnable()
    {
        if (Target)
        {
            Target.PlayAnimationEvent += Repaint;
            Target.StopAnimationEvent += Repaint;
        }
    }

    private void OnDisable()
    {
        if (Target)
        {
            Target.PlayAnimationEvent -= Repaint;
            Target.StopAnimationEvent -= Repaint;
        }
    }

    private void OnDestroy()
    {
        if (Target)
        {
            Target.PlayAnimationEvent -= Repaint;
            Target.StopAnimationEvent -= Repaint;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        NDAnimationPlayer animPlayer = (NDAnimationPlayer)target;

        GUILayout.Space(15);

        if (GUILayout.Button("Set up game objects"))
        {
            animPlayer.SetUpBones();
            animPlayer.CleanGameObjects();
        }

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Play"))
        {
            if (EditorApplication.isPlaying)
                animPlayer.PlayAnimation();
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Stop"))
        {
            if (EditorApplication.isPlaying)
                animPlayer.StopAnimation();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        if (animPlayer.anim)
        {
            if (animPlayer.anim.isPlaying)
                EditorGUILayout.HelpBox("PLAYING", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("STOPPED", MessageType.Info);
        }
        else
        {
            animPlayer.anim = animPlayer.gameObject.GetComponent<Animation>();
            EditorGUILayout.HelpBox("STOPPED", MessageType.Info);
        }
    }
}
