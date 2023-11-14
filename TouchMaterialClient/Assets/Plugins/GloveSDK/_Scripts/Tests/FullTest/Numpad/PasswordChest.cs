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
using UnityEngine.UI;

namespace NDTest
{
    // <summary>
    // Test chest that is opened or closed, depending on the numpad code
    // </summary>
    public class PasswordChest : MonoBehaviour
    {
        /// <summary>
        /// Visible code to open the chest
        /// </summary>
        public Text password;
        /// <summary>
        /// Chest cover
        /// </summary>
        public Transform cover;
        public GameObject chestLight;
        private Material lightMaterial;

        public Vector3 openLocalRot;
        public Vector3 closedLocalRot;
        public Color openColor = Color.green;
        public Color closedColor = Color.red;

        void Start()
        {
            lightMaterial = chestLight.GetComponent<Renderer>().material;
            cover.localEulerAngles = closedLocalRot;
            lightMaterial.SetColor("_EmissionColor", closedColor);
        }

        /// <summary>
        /// Open the chest if the password matches with the code shown
        /// </summary>
        /// <param name="password">Password entered in the numpad</param>
        public void Open(string password)
        {
            if (this.password.text.Equals(password))
            {
                cover.localEulerAngles = openLocalRot;
                lightMaterial.SetColor("_EmissionColor", openColor);
            }
            else
            {
                cover.localEulerAngles = closedLocalRot;
                lightMaterial.SetColor("_EmissionColor", closedColor);
            }
        }
    }
}