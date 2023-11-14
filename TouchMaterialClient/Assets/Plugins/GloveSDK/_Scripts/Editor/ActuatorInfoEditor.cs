﻿/******************************************************************************
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

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActuatorInfo))]
public class ActuatorInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ActuatorInfo actuator = (ActuatorInfo)target;

        GUILayout.Space(15);

        if (GUILayout.Button("Search basic information"))
        {
            actuator.SetLocation();
            actuator.SetUserIndex();
            actuator.SetPhalanxIndex();
            actuator.SetActuator();
        }

        GUILayout.Space(15);
    }
}