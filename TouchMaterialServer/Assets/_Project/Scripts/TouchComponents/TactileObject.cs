using UnityEngine;

namespace TouchMaterial.Server
{
    public class TactileObject : MonoBehaviour
    {
        [SerializeField]
        private Texture2D _referenceMap;
        [SerializeField, Range(0f, 1f)]
        private float _tactileMultiplier = 0.5f;
        private int _layerMask;


        void Start()
        {
            _layerMask = LayerMask.GetMask("TactileObject");
        }

        // 0f to +1f, 0f for not collided
        public float GetTactileUnsigned(Vector3 position)
        {
            Vector3 lossyScale = transform.lossyScale;
            float longestSide = Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
            float rayLength = longestSide * 2f;

            Vector3 origin = position + transform.up * rayLength;
            if (Physics.Raycast(origin, -transform.up, out var hit, rayLength, _layerMask))
            {
                var texCoord = hit.textureCoord;

                Color color = _referenceMap.GetPixelBilinear(texCoord.x, texCoord.y);
                float baseValue = color.r;

                float depth = Vector3.Distance(hit.point, position);
                float modifier = Mathf.Clamp(depth * 10f, 0f, 0.5f);
                float ret = Mathf.Clamp01(baseValue * (1f + modifier)) * _tactileMultiplier;
                return ret;
            }
            return 0f;
        }

        // -1f to +1f, 0f for not collided
        public float GetTactileSigned(Vector3 position)
        {
            Vector3 lossyScale = transform.lossyScale;
            float longestSide = Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
            float rayLength = longestSide * 2f;

            Vector3 origin = position + transform.up * rayLength;
            if (Physics.Raycast(origin, -transform.up, out var hit, rayLength, _layerMask))
            {
                var texCoord = hit.textureCoord;

                Color color = _referenceMap.GetPixelBilinear(texCoord.x, texCoord.y);
                float baseValue = (color.r - 0.5f) * 2f;

                float depth = Vector3.Distance(hit.point, position);
                float modifier = Mathf.Clamp(depth * 10f, 0f, 0.5f);
                float ret = Mathf.Clamp(baseValue * (1f + modifier), -1f, 1f);
                return ret;
            }
            return 0f;
        }

    }
}
