using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace TouchMaterial.Client
{
    public class MPEGDecodeHelper : IDecodeHelper
    {
        private bool _useCompression;
        private float[][] _tactileCaches;   // [actuator][frame]
        private int _actuatorCount;
        private int _frameCount;
        private int _taxelFixedSize;


        public MPEGDecodeHelper(int actuatorCount, int frameCount, int taxelFixedSize, bool useCompression)
        {
            _useCompression = useCompression;
            _actuatorCount = actuatorCount;
            _frameCount = frameCount;
            _taxelFixedSize = taxelFixedSize;
            _tactileCaches = new float[_actuatorCount][];
            for (int i = 0; i < _actuatorCount; i++)
            {
                _tactileCaches[i] = new float[_frameCount * _taxelFixedSize];
                for (int j = 0; j < _tactileCaches[i].Length; j++)
                {
                    _tactileCaches[i][j] = 0f;
                }
            }
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public async Task Decode(byte[] data, Action<float[][]> onFinished = null)
        {
            if (data == null)
            {
                return;
            }
            byte[] rawData = data;
            if (_useCompression)
            {
                int index = 0;
                index = data.ReadInt(index, out int actuatorCount);
                int[] lengths = new int[actuatorCount];
                for (int i = 0; i < actuatorCount; i++)
                {
                    index = data.ReadInt(index, out lengths[i]);
                }
                byte[][] bodies = new byte[actuatorCount][];
                for (int i = 0; i < actuatorCount; i++)
                {
                    bodies[i] = new byte[lengths[i]];
                    Array.Copy(data, index, bodies[i], 0, lengths[i]);
                    index += lengths[i];
                }

                for (int i = 0; i < actuatorCount; i++)
                {
                    await Task.Run(() =>
                    {
                        int outputSize = 0;
                        IntPtr output = IntPtr.Zero;
                        Decode(bodies[i], bodies[i].Length, ref output, ref outputSize);
                        float[] signals = new float[outputSize];
                        Marshal.Copy(output, signals, 0, outputSize);
                        Marshal.FreeHGlobal(output);
                        Array.Copy(signals, 0, _tactileCaches[i], 0, outputSize);
                    });
                }
                onFinished?.Invoke(_tactileCaches);
            }
            else
            {
                Deserialize(rawData);
                onFinished?.Invoke(_tactileCaches);
            }

        }

        private void Deserialize(byte[] buffer)
        {
            if (buffer == null)
            {
                return;
            }
            int index = 0;

            try
            {
                for (int i = 0; i < _tactileCaches.Length; i++)
                {
                    for (int j = 0; j < _tactileCaches[i].Length; j++)
                    {
                        index = buffer.ReadFloat(index, out var signal);
                        _tactileCaches[i][j] = signal;
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.LogWarning(ex.ToString());
            }
        }

        [DllImport("HapticCodec")]
        private static extern int Decode(byte[] input, int inputSize, ref IntPtr output, ref int outputSize);

    }
}
