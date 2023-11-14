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
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MUXDataProvider_Hand))]
public class MUXDataProvider_HandEditor : Editor
{
    MUXDataProvider_Hand muxDataProvider;
    SerializedProperty _priorityPositiveValue;
    SerializedProperty[] _toggleValues = new SerializedProperty[10];

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        muxDataProvider = (MUXDataProvider_Hand)target;

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        if (muxDataProvider.priorityPositiveValue < 0)
            muxDataProvider.priorityPositiveValue = 0;

        _priorityPositiveValue = serializedObject.FindProperty("priorityPositiveValue");
        _priorityPositiveValue.intValue = EditorGUILayout.IntField("Priority Value", _priorityPositiveValue.intValue);

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Rotations", EditorStyles.boldLabel);

        for (int i = 0; i <= 9; i++)
        {
            SensorID enumDisplayStatus = (SensorID)i;
            _toggleValues[i] = serializedObject.FindProperty("bonesToControlRotation").GetArrayElementAtIndex(i);
            _toggleValues[i].boolValue = EditorGUILayout.Toggle(enumDisplayStatus.ToString(), _toggleValues[i].boolValue);
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Deselect all"))
        {
            SelectAllRotations(false);
        }

        if (GUILayout.Button("Select all"))
        {
            SelectAllRotations(true);
        }
        GUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            muxDataProvider.UpdateRotations();
        }
    }

    private void SelectAllRotations(bool selected)
    {
        for (int i = 0; i <= 9; i++)
        {
            SensorID enumDisplayStatus = (SensorID)i;
            _toggleValues[i] = serializedObject.FindProperty("bonesToControlRotation").GetArrayElementAtIndex(i);
            _toggleValues[i].boolValue = EditorGUILayout.Toggle(enumDisplayStatus.ToString(), selected);
        }
    }
}
