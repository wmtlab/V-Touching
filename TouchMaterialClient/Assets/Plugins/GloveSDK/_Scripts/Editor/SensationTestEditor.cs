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

[CustomEditor(typeof(SensationTest))]
public class SensationTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.HelpBox("This script shows how to play a sensation. Open \"SensationTest\" script" +
            " and check how it works. \n\nPress the \"S\" key to play a defined sensation.\n\n" +
            "In order to make Sensations works, please, be sure that \"Use Haptics\" checkbox is checked in " +
            "the NDInitializer GameObject", MessageType.Info);
    }
}
