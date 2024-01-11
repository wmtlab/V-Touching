using System.Collections.Generic;
using UnityEngine;

namespace TouchMaterial.Server
{
    public class TactileDetector : MonoBehaviour
    {
        private List<TactileObject> _touchedObjects = new List<TactileObject>();
        private float[] _emptyTactile;


        // -1f to +1f, 0f for not collided
        public float[] GetTactile()
        {
            float[] signals = _touchedObjects.Count > 0 ?
                _touchedObjects[0].GetTactile(transform.position, transform.up) : null;

            return signals;
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
