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
using NDAPIWrapperSpace;

[CustomEditor(typeof(MUXDataProvider_External))]
public class MUXDataProvider_ExternalEditor : Editor
{
    private static GUIStyle ToggleButtonStyleNormal = null;
    private static GUIStyle ToggleButtonStyleToggled = null;
    protected bool isActive = true;
    SerializedProperty _positionSource;
    SerializedProperty _rotationSource;

    MUXDataProvider_External muxDataProvider;

    public override void OnInspectorGUI()
    {
        muxDataProvider = (MUXDataProvider_External)target;

        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        muxDataProvider.sourceId = EditorGUILayout.TextField("Source ID", muxDataProvider.sourceId);
        muxDataProvider.userId = EditorGUILayout.IntField("User ID", muxDataProvider.userId);
        muxDataProvider.handLocation = (Location)EditorGUILayout.EnumPopup("Hand location", muxDataProvider.handLocation);

        if (muxDataProvider.positionSource == null)
            muxDataProvider.positionSource = muxDataProvider.transform;

        if (muxDataProvider.rotationSource == null)
            muxDataProvider.rotationSource = muxDataProvider.transform;

        if (muxDataProvider.priorityPositiveValue < 0)
            muxDataProvider.priorityPositiveValue = 0;

        muxDataProvider.priorityPositiveValue = EditorGUILayout.IntField("Priority Value", muxDataProvider.priorityPositiveValue);

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Positions", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        muxDataProvider.bonesToControlPosition[0] = EditorGUILayout.Toggle(SensorID.Hand.ToString(), muxDataProvider.bonesToControlPosition[0]);

        muxDataProvider.bonesToControlPositionType[0] = (Type_MUXValueType)EditorGUILayout.EnumPopup(muxDataProvider.bonesToControlPositionType[0]);

        _positionSource = serializedObject.FindProperty("positionSource");
        EditorGUILayout.PropertyField(_positionSource, GUIContent.none);

        GUILayout.EndHorizontal();

        muxDataProvider.positionOffset = EditorGUILayout.Vector3Field("Position offset", muxDataProvider.positionOffset);

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Rotations", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        SensorID enumDisplayStatus = SensorID.Hand;
        muxDataProvider.bonesToControlRotation[(int)SensorID.Hand] = EditorGUILayout.Toggle(enumDisplayStatus.ToString(), muxDataProvider.bonesToControlRotation[(int)SensorID.Hand]);

        muxDataProvider.bonesToControlRotationType[(int)SensorID.Hand] = (Type_MUXValueType)EditorGUILayout.EnumPopup(muxDataProvider.bonesToControlRotationType[(int)SensorID.Hand]);

        _rotationSource = serializedObject.FindProperty("rotationSource");
        EditorGUILayout.PropertyField(_rotationSource, GUIContent.none);

        GUILayout.EndHorizontal();

        muxDataProvider.rotationOffset = EditorGUILayout.Vector3Field("Rotation offset", muxDataProvider.rotationOffset);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        if (Application.isPlaying)
        {
            if (ToggleButtonStyleNormal == null)
            {
                ToggleButtonStyleNormal = "Button";
                ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
            }

            if (GUILayout.Button("Update values"))
            {
                muxDataProvider.SetActive(isActive);
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Enabled", isActive ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
            {
                isActive = true;
                muxDataProvider.SetActive(isActive);
            }

            if (GUILayout.Button("Disabled", !isActive ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
            {
                isActive = false;
                muxDataProvider.SetActive(isActive);
            }

            GUILayout.EndHorizontal();
        }
    }
}