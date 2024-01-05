using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TouchMaterial.Client
{
    public class DecodeHelper : IDecodeHelper
    {
        private bool _useCompression;
        private UdpReceiver _receiver;
        private UdpSender _sender;
        private ConcurrentQueue<byte[]> _queue;
        private byte[] _buffer;
        private int _actuatorCount;
        private int _frameCount;
        private int _taxelFixedSize;
        private float[][] _tactileCaches;   // [actuator][frame]

        private CancellationTokenSource _cts;

        public DecodeHelper(InitParams initParams)
        {
            _useCompression = initParams.useCompression;
            _actuatorCount = initParams.actuatorCount;
            _frameCount = initParams.frameCount;
            _taxelFixedSize = initParams.taxelFixedSize;
            _tactileCaches = new float[_actuatorCount][];
            for (int i = 0; i < _actuatorCount; i++)
            {
                _tactileCaches[i] = new float[_frameCount * _taxelFixedSize];
            }
            if (!_useCompression)
            {
                return;
            }
            _buffer = new byte[initParams.bufferSize];
            _queue = new ConcurrentQueue<byte[]>();
            _sender = new UdpSender(initParams.sendIp, initParams.sendPort);
            _receiver = new UdpReceiver(
                initParams.receiveIp, initParams.receivePort, initParams.bufferSize,
                (data, length) =>
                {
                    _queue.Enqueue(data);
                });

        }

        public void Start()
        {
            if (!_useCompression)
            {
                return;
            }
            _sender?.Start();
            _receiver?.Start();
            _cts = new CancellationTokenSource();
        }

        public void Stop()
        {
            if (!_useCompression)
            {
                return;
            }
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _sender?.Close();
            _receiver?.Close();
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
                _buffer.WriteInt(0, 1);
                data.Take(data.Length - sizeof(int)).ToArray().CopyTo(_buffer, sizeof(int));
                _sender.SendData(_buffer);
                await Task.Run(() =>
                {
                    while (_queue.IsEmpty && !_cts.IsCancellationRequested) ;
                });
                _queue.TryDequeue(out rawData);
            }
            Deserialize(rawData);
            onFinished?.Invoke(_tactileCaches);
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
                        if (_useCompression)
                        {
                            index = buffer.ReadInt(index, out var intIntensity);
                            float signal = Mathf.Clamp((float)intIntensity / (1 << 15), -1f, 1f);
                            if (Mathf.Approximately(Mathf.Abs(signal), 1f))
                            {
                                signal = 0f;
                            }
                            _tactileCaches[i][j] = signal;
                            continue;
                        }
                        else
                        {
                            index = buffer.ReadFloat(index, out var signal);
                            _tactileCaches[i][j] = signal;
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.LogWarning(ex.ToString());
            }
        }

        public struct InitParams
        {
            public bool useCompression;
            public int actuatorCount;
            public int frameCount;
            public int taxelFixedSize;
            public string sendIp;
            public int sendPort;
            public string receiveIp;
            public int receivePort;
            public int bufferSize;
        }
    }
}
