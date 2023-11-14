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

// <summary>
// Editor script of HandGesture
// </summary>
[CustomEditor(typeof(HandGesture))]
public class HandGestureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HandGesture hgc = (HandGesture)target;

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create hierarchy"))
        {
            hgc.SetUpBones();
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Apply Gesture"))
        {
            hgc.CleanGameObjects();

            hgc.CreatePrefab();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);
    }
}