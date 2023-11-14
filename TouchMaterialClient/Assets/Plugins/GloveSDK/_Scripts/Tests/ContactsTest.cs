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
using UnityDLL.Contacts;

// <summary>
// Simple contacts test performing gestures
// </summary>
public class ContactsTest : MonoBehaviour
{
    void Update()
    {
        // If any hand has the index and thumb contacts joined, shows a message
        if (ContactsSystem.AreContactsJoined(NDAPIWrapperSpace.Contact.CONT_THUMB, NDAPIWrapperSpace.Contact.CONT_INDEX))
        {
            Debug.Log("Thumb and index joined :)");
        }

        // If all hands has the index and thumb contacts joined, shows a message
        if (ContactsSystem.AreContactsJoinedAll(NDAPIWrapperSpace.Contact.CONT_THUMB, NDAPIWrapperSpace.Contact.CONT_INDEX))
        {
            Debug.Log("All gloves has thumb and index joined :)");
        }
    }
}
