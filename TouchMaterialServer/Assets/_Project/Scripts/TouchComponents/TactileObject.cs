using System;
using System.IO;
using UnityEngine;

namespace TouchMaterial.Server
{
    public class TactileObject : MonoBehaviour
    {
        [SerializeField]
        private int _fixedSize = 8;
        [SerializeField, Range(0f, 1f)]
        private float _tactileMultiplier = 0.5f;
        private int _layerMask;
        private float[,][] _referenceData;
        private static readonly float[] EmptyTactile = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        private int _height;
        private int _width;

        void Start()
        {
            _layerMask = LayerMask.GetMask("TactileObject");
            if (!LoadReferenceData())
            {
                Debug.LogError($"Failed to load tactile data: {gameObject.name}");
            }
        }

        private bool LoadReferenceData()
        {
            string refDir = Path.Combine(Application.streamingAssetsPath, $"TactileData/{gameObject.name}");
            string headerPath = Path.Combine(refDir, "header.bin");
            string dataPath = Path.Combine(refDir, "data.bin");

            if (!File.Exists(headerPath) || !File.Exists(dataPath))
            {
                Debug.LogError($"Tactile data not found: {gameObject.name}");
                return false;
            }
            byte[] headerBytes = File.ReadAllBytes(headerPath);
            byte[] dataBytes = File.ReadAllBytes(dataPath);
            int height = BitConverter.ToInt32(headerBytes, 0);
            int width = BitConverter.ToInt32(headerBytes, sizeof(int));
            if (height <= 0 || width <= 0)
            {
                Debug.LogError($"Invalid tactile data: {gameObject.name}");
                return false;
            }
            if (headerBytes.Length != (height * width + 2) * sizeof(int))
            {
                Debug.LogError($"Invalid tactile data: {gameObject.name}");
                return false;
            }

            int[] starts = new int[height * width + 1];
            for (int i = 0; i < height * width; i++)
            {
                starts[i] = BitConverter.ToInt32(headerBytes, (i + 2) * sizeof(int));
            }
            starts[height * width] = dataBytes.Length / sizeof(float);

            float[,][] referenceData = new float[height, width][];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int start = starts[i * width + j];
                    int end = starts[i * width + j + 1];
                    int length = end - start;
                    float[] raw = new float[length];
                    for (int k = 0; k < length; k++)
                    {
                        raw[k] = BitConverter.ToSingle(dataBytes, (start + k) * sizeof(float));
                    }
                    referenceData[i, j] = RawToFixedArray(raw);
                }
            }

            _referenceData = referenceData;
            _height = height;
            _width = width;
            return true;
        }

        private float[] RawToFixedArray(float[] raw)
        {
            float[] ret = new float[_fixedSize];
            if (raw.Length == _fixedSize)
            {
                return raw;
            }
            else if (raw.Length > _fixedSize)
            {
                for (int i = 0; i < _fixedSize; i++)
                {
                    ret[i] = raw[(int)((float)i / _fixedSize * raw.Length)];
                }
            }
            else if (raw.Length == 0)
            {
                for (int i = 0; i < _fixedSize; i++)
                {
                    ret[i] = 0f;
                }
            }
            else
            {
                float step = (raw.Length - 1f) / (_fixedSize - 1f);
                for (int i = 0; i < _fixedSize; i++)
                {
                    float index = i * step;
                    int floor = (int)index;
                    if (floor == raw.Length - 1)
                    {
                        ret[i] = raw[floor];
                    }
                    else
                    {
                        ret[i] = Mathf.Lerp(raw[floor], raw[floor + 1], index - floor);
                    }
                }
            }
            return ret;
        }

        // uv: ↑y, →x
        // data source：↓r, →c
        // r = 1 - y, c = x

        // -1f to +1f, 0f for not collided
        public float[] GetTactile(Vector3 position, Vector3 normal)
        {
            float rayLength = 1f;

            Vector3 origin = position - normal * rayLength;
            if (Physics.Raycast(origin, normal, out var hit, rayLength, _layerMask))
            {
                var texCoord = hit.textureCoord;

                int r = (int)((1f - texCoord.y) * _height);
                int c = (int)(texCoord.x * _width);
                float[] baseValues;
                if (_referenceData[r, c] != null && _referenceData[r, c].Length > 0)
                {
                    baseValues = _referenceData[r, c];
                }
                else
                {
                    baseValues = EmptyTactile;
                }

                float depth = Vector3.Distance(hit.point, position);
                float modifier = Mathf.Clamp(depth * 10f, 0f, 0.5f);
                float[] ret = new float[_fixedSize];
                for (int i = 0; i < _fixedSize; i++)
                {
                    ret[i] = Mathf.Clamp(baseValues[i] * (1f + modifier), -1f, 1f) * _tactileMultiplier;
                }
                return ret;
            }
            return EmptyTactile;
        }

    }
}
