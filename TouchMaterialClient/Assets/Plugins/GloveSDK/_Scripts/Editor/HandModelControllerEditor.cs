/******************************************************************************
* Copyright © NeuroDigital Technologies, S.L. 2018                            *
* Licensed under the Apache License, Version 2.0 (the "License");             *
* you may not use this file except in compliance with the License.            *
* You may obtain a copy of the License at                                     *
* http://www.apache.org/licenses/LICENSE-2.0                                  *
* Unless required by applicable law or agreed to in writing, software         *
* distributed under the License is distributed on an "AS IS" BASIS,           *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.    *
* See the License for the specific language governing permissions and         *
* limitations under the License.                                              *
*******************************************************************************/

using UnityEditor;

[CustomEditor(typeof(HandModelController))]
public class HandModelControllerEditor : Editor
{
    SerializedProperty _user;
    SerializedProperty _handLocation;
    SerializedProperty _useGestures;
    SerializedProperty _initialPoseCalibration;

    HandModelController hmc;

    override public void OnInspectorGUI()
    {
        hmc = (HandModelController)target;

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        _user = serializedObject.FindProperty("user");
        _user.intValue = EditorGUILayout.IntField("User ID", hmc.user);
        _handLocation = serializedObject.FindProperty("handLocation");
        EditorGUILayout.PropertyField(_handLocation, true);

        _useGestures = serializedObject.FindProperty("useGestures");
        _useGestures.boolValue = EditorGUILayout.Toggle("Use gestures", _useGestures.boolValue);
        if (_useGestures.boolValue)
        {
            SerializedProperty indexPinchGesture = serializedObject.FindProperty("indexPinchGesture");
            EditorGUILayout.PropertyField(indexPinchGesture, true);
            SerializedProperty middlePinchGesture = serializedObject.FindProperty("middlePinchGesture");
            EditorGUILayout.PropertyField(middlePinchGesture, true);
            SerializedProperty threePinchGesture = serializedObject.FindProperty("threePinchGesture");
            EditorGUILayout.PropertyField(threePinchGesture, true);
            SerializedProperty okGesture = serializedObject.FindProperty("okGesture");
            EditorGUILayout.PropertyField(okGesture, true);
            SerializedProperty gunGesture = serializedObject.FindProperty("gunGesture");
            EditorGUILayout.PropertyField(gunGesture, true);
            SerializedProperty pinchFistGesture = serializedObject.FindProperty("pinchFistGesture");
            EditorGUILayout.PropertyField(pinchFistGesture, true);
        }

        _initialPoseCalibration = serializedObject.FindProperty("forceInitialPoseCalibration");
        _initialPoseCalibration.boolValue = EditorGUILayout.Toggle("Force Initial Pose Calibration", _initialPoseCalibration.boolValue);
        if (!_initialPoseCalibration.boolValue)
        {
            SerializedProperty playerForwardCalculator = serializedObject.FindProperty("playerForwardCalculator");
            EditorGUILayout.PropertyField(playerForwardCalculator, true);
        }

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
