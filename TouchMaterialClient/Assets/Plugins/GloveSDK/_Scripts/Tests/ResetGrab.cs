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

public class ResetGrabTest : MonoBehaviour
{
    // The initial sphere position
    private Vector3 initPos;
    // Rigidbody attached to the sphere
    private Rigidbody rb;
    // Collider attached to the sphere
    private Collider[] cols;

    void Start()
    {
        // Gets the initial pos
        initPos = transform.position;
        // Gets the rigidbody
        rb = GetComponent<Rigidbody>();
        // Gets the collider
        cols = GetComponents<Collider>();
    }

    void Update()
    {
        // When pressing return takes the sphere back to the table and resets it
        if (Input.GetKeyDown(KeyCode.Return))
            ResetSphere();
    }

    private void ResetSphere()
    {
        // Disable the sphere for grabbing
        foreach (Collider col in cols)
        {
            col.enabled = false;
        }
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        // Resets sphere position
        transform.position = initPos;
        // Allows grabbing the sphere
        foreach (Collider col in cols)
        {
            col.enabled = true;
        }
        rb.isKinematic = false;
    }
}
