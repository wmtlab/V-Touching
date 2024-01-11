using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TouchMaterial.Server
{
    public class MPEGEncodeHelper : IEncodeHelper
    {
        private bool _useCompression;

        public MPEGEncodeHelper(bool useCompression)
        {
            _useCompression = useCompression;
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public Task<(byte[], int)> EncodeAsync(float[][] tactileCaches)
        {
            if (_useCompression)
            {
                int actuatorCount = tactileCaches.Length;
                byte[][] outputs = new byte[actuatorCount][];
                for (int i = 0; i < actuatorCount; i++)
                {
                    var input = tactileCaches[i].Clone() as float[];
                    IntPtr output = IntPtr.Zero;
                    int outputSize = 0;
                    Encode(input, input.Length, ref output, ref outputSize);
                    outputs[i] = new byte[outputSize];
                    Marshal.Copy(output, outputs[i], 0, outputSize);
                    Marshal.FreeHGlobal(output);
                }
                int bodySize = 0;
                foreach (var output in outputs)
                {
                    bodySize += output.Length;
                }
                int headerSize = (1 + actuatorCount) * sizeof(int);
                int totalSize = headerSize + bodySize;
                byte[] result = new byte[totalSize];
                int index = 0;
                index = result.WriteInt(index, actuatorCount);
                foreach (var output in outputs)
                {
                    index = result.WriteInt(index, output.Length);
                }
                int offset = headerSize;
                foreach (var output in outputs)
                {
                    Array.Copy(output, 0, result, offset, output.Length);
                    offset += output.Length;
                }
                return Task.FromResult((result, totalSize));
            }
            else
            {
                int totalSize = tactileCaches.Length * tactileCaches[0].Length * sizeof(float);
                byte[] result = new byte[totalSize];
                int index = 0;
                for (int i = 0; i < tactileCaches.Length; i++)
                {
                    for (int j = 0; j < tactileCaches[i].Length; j++)
                    {
                        float tactile = tactileCaches[i][j];
                        index = result.WriteFloat(index, tactile);
                    }
                }
                return Task.FromResult((result, index));
            }
        }

        [DllImport("HapticCodec")]
        private static extern int Encode(float[] input, int inputSize, ref IntPtr output, ref int outputSize);
    }
}
