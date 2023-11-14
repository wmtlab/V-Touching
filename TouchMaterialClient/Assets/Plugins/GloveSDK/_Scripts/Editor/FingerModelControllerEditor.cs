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

[CustomEditor(typeof(FingerModelController))]
public class FingerModelControllerEditor : Editor
{
    FingerModelController fmc;
    SerializedProperty _applyLimits;
    SerializedProperty _phalangesSimulation;
    SerializedProperty lockX;
    SerializedProperty lockY;

    override public void OnInspectorGUI()
    {
        fmc = (FingerModelController)target;

        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        _phalangesSimulation = serializedObject.FindProperty("enablePhalangesClosingSimulation");
        _phalangesSimulation.boolValue = EditorGUILayout.Toggle("Phalanges closing simulation", _phalangesSimulation.boolValue);
        if (_phalangesSimulation.boolValue)
        {
            lockX = serializedObject.FindProperty("lockX");
            lockX.boolValue = EditorGUILayout.Toggle("Lock X", lockX.boolValue);
            lockY = serializedObject.FindProperty("lockY");
            lockY.boolValue = EditorGUILayout.Toggle("Lock Y", lockY.boolValue);

            SerializedProperty degreesToTurnYToZero = serializedObject.FindProperty("degreesToTurnYToZero");
            degreesToTurnYToZero.floatValue = EditorGUILayout.FloatField("Degrees to turn Y to 0", degreesToTurnYToZero.floatValue);

            SerializedProperty desiredDegreesForP1OnClosing = serializedObject.FindProperty("desiredDegreesForP1OnClosing");
            desiredDegreesForP1OnClosing.floatValue = EditorGUILayout.FloatField("Desired degrees for P1 on closing", desiredDegreesForP1OnClosing.floatValue);
            SerializedProperty animCurveCloseSensitivityP1 = serializedObject.FindProperty("animCurveCloseSensitivityP1");
            animCurveCloseSensitivityP1.animationCurveValue = EditorGUILayout.CurveField("Anim curve close sensitivity P1", animCurveCloseSensitivityP1.animationCurveValue);
            SerializedProperty degreesToTurnP1ToPreviousValue = serializedObject.FindProperty("degreesToTurnP1ToPreviousValue");
            degreesToTurnP1ToPreviousValue.floatValue = EditorGUILayout.FloatField("Degrees to turn P1 to previous value", degreesToTurnP1ToPreviousValue.floatValue);

            SerializedProperty desiredDegreesForP2OnClosing = serializedObject.FindProperty("desiredDegreesForP2OnClosing");
            desiredDegreesForP2OnClosing.floatValue = EditorGUILayout.FloatField("Desired degrees for P2 on closing", desiredDegreesForP2OnClosing.floatValue);
            SerializedProperty animCurveCloseSensitivityP2 = serializedObject.FindProperty("animCurveCloseSensitivityP2");
            animCurveCloseSensitivityP2.animationCurveValue = EditorGUILayout.CurveField("Anim curve close sensitivity P2", animCurveCloseSensitivityP2.animationCurveValue);
            SerializedProperty degreesToTurnP2ToPreviousValue = serializedObject.FindProperty("degreesToTurnP2ToPreviousValue");
            degreesToTurnP2ToPreviousValue.floatValue = EditorGUILayout.FloatField("Degrees to turn P2 to previous value", degreesToTurnP2ToPreviousValue.floatValue);

            //fmc.desiredDegreesForP1OnClosing = EditorGUILayout.FloatField("Desired degrees for P1 on closing", fmc.desiredDegreesForP1OnClosing);
            //fmc.animCurveCloseSensitivityP1 = EditorGUILayout.CurveField("Anim curve close sensitivity P1", fmc.animCurveCloseSensitivityP1);
            //fmc.degreesToTurnP1ToPreviousValue = EditorGUILayout.FloatField("Degrees to turn P1 to previous value", fmc.degreesToTurnP1ToPreviousValue);

            //fmc.desiredDegreesForP2OnClosing = EditorGUILayout.FloatField("Desired degrees for P2 on closing", fmc.desiredDegreesForP2OnClosing);
            //fmc.animCurveCloseSensitivityP2 = EditorGUILayout.CurveField("Anim curve close sensitivity P2", fmc.animCurveCloseSensitivityP2);
            //fmc.degreesToTurnP2ToPreviousValue = EditorGUILayout.FloatField("Degrees to turn P2 to previous value", fmc.degreesToTurnP2ToPreviousValue);
        }

        _applyLimits = serializedObject.FindProperty("applyLimits");
        _applyLimits.boolValue = EditorGUILayout.Toggle("Apply limits", _applyLimits.boolValue);
        if (_applyLimits.boolValue)
        {
            SerializedProperty maxXDegrees = serializedObject.FindProperty("maxXDegrees");
            maxXDegrees.floatValue = EditorGUILayout.FloatField("Max X degrees", maxXDegrees.floatValue);
            SerializedProperty minXDegrees = serializedObject.FindProperty("minXDegrees");
            minXDegrees.floatValue = EditorGUILayout.FloatField("Min X degrees", minXDegrees.floatValue);
            SerializedProperty maxYDegrees = serializedObject.FindProperty("maxYDegrees");
            maxYDegrees.floatValue = EditorGUILayout.FloatField("Max Y degrees", maxYDegrees.floatValue);
            SerializedProperty minYDegrees = serializedObject.FindProperty("minYDegrees");
            minYDegrees.floatValue = EditorGUILayout.FloatField("Min Y degrees", minYDegrees.floatValue);
        }

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
