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

namespace NDTest
{
    public class ScreenPaint : MonoBehaviour
    {
        public Texture2D texture;
        public LayerMask layer;
        public LayerMask layerSupport;
        public float radius;
        public int brushSize;
        public float intensity;
        public Color[] initialColors;
        private ActuatorInfo actuatorInfo;
        private RaycastHit hit;

        // Use this for initialization
        void Start()
        {
            actuatorInfo = GetComponentInParent<ActuatorInfo>();

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, Color.black);
                }
            }
            initialColors = texture.GetPixels();
            texture.Apply();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (brushSize <= 0)
                brushSize = 1;

            Collider[] cols = Physics.OverlapSphere(this.transform.position, radius, layerSupport);
            if (cols != null && cols.Length != 0)
            {
                Vector3 rayOrigin = cols[0].ClosestPoint(transform.position) + cols[0].transform.up;

                Debug.DrawRay(rayOrigin, -cols[0].transform.up, Color.magenta);

                if (Physics.Raycast(rayOrigin, -cols[0].transform.up, out hit, Mathf.Infinity, layer))
                {
                    Vector3 pixelUV = hit.textureCoord;

                    pixelUV.x *= texture.width;
                    pixelUV.y *= texture.height;

                    for (int i = -brushSize; i <= brushSize; i++)
                    {
                        if ((int)pixelUV.x + i >= 0 && (int)pixelUV.x + i <= texture.width - 1)
                        {
                            texture.SetPixel((int)pixelUV.x + i, (int)pixelUV.y, Color.white);
                            for (int j = -brushSize; j <= brushSize; j++)
                            {
                                if ((int)pixelUV.y + j >= 0 && (int)pixelUV.y + j <= texture.height - 1)
                                {
                                    texture.SetPixel((int)pixelUV.x + i, (int)pixelUV.y + j, Color.white);
                                }
                            }
                        }
                    }

                    HapticSystem.PlayPulse(intensity, 100, actuatorInfo.location, actuatorInfo.userIndex, actuatorInfo.actuator);
                    texture.Apply();
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position, radius);
        }
    }
}