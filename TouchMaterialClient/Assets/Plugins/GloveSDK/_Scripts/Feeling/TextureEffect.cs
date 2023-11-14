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
#pragma warning disable 44
using System;
using UnityEngine;
using UnityDLL.Haptic;

public class TextureEffect : MonoBehaviour
{
    public LayerMask layerMask, layerMaskSupport;
    public float sensationFactor = 1f;
    public float radius;
    private ActuatorInfo actuator;
    private RaycastHit hit;

    // Use this for initialization
    void Start()
    {
        actuator = transform.GetComponentInParent<ActuatorInfo>();
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        float value = 0f;
        try
        {
            // Gets the colliders in range
            Collider[] cols = Physics.OverlapSphere(this.transform.position, radius, layerMaskSupport);
            if (cols != null && cols.Length != 0)
            {
                // Gets the origin of the ray depending on the closest point to the finger
                Vector3 rayOrigin = cols[0].ClosestPoint(transform.position) + cols[0].transform.up;

                // DEBUG
                Debug.DrawRay(rayOrigin, -cols[0].transform.up, Color.magenta);

                // Casts a ray
                if (Physics.Raycast(rayOrigin, -cols[0].transform.up, out hit, Mathf.Infinity, layerMask))
                {
                    Renderer renderer = hit.collider.GetComponent<Renderer>();
                    MeshCollider meshCollider = hit.collider as MeshCollider;
                    if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null)
                    {
                        return;
                    }
                    // Texture read from "Detail Mask".
                    Texture2D tex = (Texture2D)renderer.material.GetTexture("_DetailMask");
                    Vector2 pixelUV = hit.textureCoord;
                    pixelUV.x *= tex.width;
                    pixelUV.y *= tex.height;

                    // Value to send to Controller from grayscale
                    value = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y).grayscale;

                    if (actuator)
                    {
                        HapticSystem.PlayPulse(value * sensationFactor, 100, actuator.location, actuator.userIndex, actuator.actuator);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
}
