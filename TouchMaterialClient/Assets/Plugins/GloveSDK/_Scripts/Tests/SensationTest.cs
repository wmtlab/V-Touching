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
using UnityDLL.Haptic;

// This class shows how a sensation can be played in Unity.
// Once you have a sensation created (.hsen file), it has to be placed
// in the /StreamingAssets/Sensations folder.
// When the scene starts, it reads automatically all sensations placed there.
// If you want to play any of these sensations, you just need to call the method
// with the same name of the file.
public class SensationTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            HapticSystem.PlaySensation("SDKTest", 1);
        }
    }
}
