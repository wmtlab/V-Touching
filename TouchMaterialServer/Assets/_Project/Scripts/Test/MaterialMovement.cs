using UnityEngine;

namespace TouchMaterial.Server.Test
{
    public class MaterialMovement : MonoBehaviour
    {
        private Vector3 _startPosition;

        void Start()
        {
            _startPosition = transform.position;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = _startPosition;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += Time.deltaTime * 0.1f * Vector3.forward;
            }
        }
    }
}
