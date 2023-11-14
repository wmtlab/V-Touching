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

    // <summary>
    // Control the behaviour of the dimmer test
    // </summary>
    public class DimmerBehaviour : MonoBehaviour
    {
        private Transform objectTfColliding;
        private Vector3 initialContact;
        private Vector3 initialRotation;
        public bool isColliding;
        public GameObject objectGOColliding;
        public float currentValue;

        public GameObject lightGO;
        private Renderer lightRenderer;
        private Material matLight;
        private Color baseColor = Color.yellow;
        public LayerMask colliderLayer;

        private void Start()
        {
            lightRenderer = lightGO.GetComponent<Renderer>();
            matLight = lightRenderer.material;
            currentValue = transform.localEulerAngles.y / 360f;

            Color finalColor = baseColor * currentValue;
            matLight.SetColor("_EmissionColor", finalColor);
        }

        void Update()
        {
            if (objectGOColliding)
            {
                Vector3 inicio = new Vector3(initialContact.x - transform.position.x, 0f, initialContact.z - transform.position.z);
                Vector3 fin = new Vector3(objectTfColliding.position.x - transform.position.x, 0f, objectTfColliding.position.z - transform.position.z);

                float angle = Vector3.Angle(inicio, fin);
                Vector3 vCross = Vector3.Cross(inicio, fin);

                if (vCross.y < 0)
                    angle = -angle;

                Vector3 localAngle = new Vector3(0f, angle, 0f);

                transform.localEulerAngles = initialRotation + localAngle;
                currentValue = transform.localEulerAngles.y / 360f;

                Color finalColor = baseColor * currentValue;
                matLight.SetColor("_EmissionColor", finalColor);

                // Haptic
                ActuatorInfo actInfo = objectGOColliding.GetComponent<ActuatorInfo>();
                if (actInfo)
                    HapticSystem.PlayPulse(currentValue / 2f, 80, actInfo.userIndex, actInfo.actuator);
            }
        }

        private void OnTriggerStay(Collider collision)
        {
            if (!objectGOColliding)
            {
                if (LayerMask.GetMask(LayerMask.LayerToName(collision.gameObject.layer)) == colliderLayer.value)
                {
                    objectGOColliding = collision.gameObject;
                    objectTfColliding = collision.transform;
                    initialContact = objectTfColliding.position;
                    initialRotation = transform.localEulerAngles;
                    isColliding = true;
                }
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (LayerMask.GetMask(LayerMask.LayerToName(collision.gameObject.layer)) == colliderLayer.value)
            {
                if (!isColliding)
                {
                    objectGOColliding = collision.gameObject;
                    objectTfColliding = collision.transform;
                    initialContact = objectTfColliding.position;
                    initialRotation = transform.localEulerAngles;
                    isColliding = true;
                }
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject == objectGOColliding)
            {
                objectGOColliding = null;
                objectTfColliding = null;
                isColliding = false;
            }
        }
    }
}