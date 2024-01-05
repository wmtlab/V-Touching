using System.Collections.Generic;
using UnityEngine;

namespace TouchMaterial.Server
{
    public class TactileDetector : MonoBehaviour
    {
        private List<TactileObject> _touchedObjects = new List<TactileObject>();
        private static readonly float[] EmptyTactile = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

        // -1f to +1f, 0f for not collided
        public float[] GetTactile()
        {
            float[] intensity = _touchedObjects.Count > 0 ?
                _touchedObjects[0].GetTactile(transform.position, transform.up) : EmptyTactile;

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
