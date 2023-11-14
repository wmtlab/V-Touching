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

namespace NDTest
{
    public class TextureChanger : MonoBehaviour
    {
        /// <summary>
        /// Materials with different textures.
        /// </summary>
        public Material[] materials;
        /// <summary>
        /// The texture plane renderer.
        /// </summary>
        private Renderer rend;
        /// <summary>
        /// Represents the active texture.
        /// </summary>
        private int index = 0;

        // Use this for initialization
        void Start()
        {
            // Gets the renderer component
            rend = GetComponent<Renderer>();
            // Sets the initial material
            rend.material = materials[index];
        }

        // Update is called once per frame
        void Update()
        {
            // Changes the texture material when the user presses the up or down arrow
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                index = (index + 1) % materials.Length;
                rend.material = materials[index];
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                index--;
                if (index < 0)
                    index = materials.Length - 1;

                rend.material = materials[index];
            }
        }
    }
}