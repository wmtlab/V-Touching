using System.Collections.Generic;
using UnityEngine;

namespace TouchMaterial.Server
{
    public class TactileDetector : MonoBehaviour
    {
        private List<TactileObject> _touchedObjects = new List<TactileObject>();

        // 0f to +1f, 0f for not collided
        public float GetTactileUnsigned()
        {
            float intensity = _touchedObjects.Count > 0 ? _touchedObjects[0].GetTactileUnsigned(transform.position) : 0f;

            return intensity;
        }

        // -1f to +1f, 0f for not collided
        public float GetTactileSigned()
        {
            float intensity = _touchedObjects.Count > 0 ? _touchedObjects[0].GetTactileSigned(transform.position) : 0f;

            return intensity;
        }

        void OnTriggerEnter(Collider other)
        {
            TactileObject tactileObject = other.GetComponent<TactileObject>();
            if (tactileObject == null)
            {
                return;
            }

            _touchedObjects.Add(tactileObject);
        }

        void OnTriggerExit(Collider other)
        {
            TactileObject tactileObject = other.GetComponent<TactileObject>();
            if (tactileObject == null)
            {
                return;
            }
            _touchedObjects.Remove(tactileObject);
        }

    }
}
