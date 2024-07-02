using UnityEngine;

namespace TouchMaterial.Client.Test
{
    public class TrackerSimulator : MonoBehaviour
    {
        private Vector3 _startPosition;

        void Start()
        {
            _startPosition = transform.position;
        }

        void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                transform.position = _startPosition;
            }
            transform.position += Time.deltaTime * 0.1f * movement;
        }
    }
}
