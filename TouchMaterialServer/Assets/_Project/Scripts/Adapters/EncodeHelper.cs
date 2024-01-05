using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TouchMaterial.Server
{
    public class EncodeHelper : IEncodeHelper
    {
        private bool _useCompression;
        private UdpSender _sender;
        private UdpReceiver _receiver;
        private byte[] _sendBuffer;
        private ConcurrentQueue<byte[]> _queue;

        private CancellationTokenSource _cts;

        public EncodeHelper(InitParams initParams)
        {
            _useCompression = initParams.useCompression;
            _sendBuffer = new byte[initParams.sendBufferSize];
            if (!_useCompression)
            {
                return;
            }
            _sender = new UdpSender(initParams.sendIp, initParams.sendPort);
            _receiver = new UdpReceiver(
                initParams.receiveIp, initParams.receivePort, initParams.receiveBufferSize,
                (data, length) =>
                {
                    byte[] buffer = new byte[length];
                    Array.Copy(data, buffer, length);
                    _queue.Enqueue(buffer);
                });
            _queue = new ConcurrentQueue<byte[]>();
        }

        public void Start()
        {
            if (!_useCompression)
            {
                return;
            }
            _receiver?.Start();
            _sender?.Start();
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

        public async Task<(byte[], int)> EncodeAsync(float[][] tactileCaches)
        {
            if (_useCompression)
            {
                int index = 0;
                // 0 means server
                index = _sendBuffer.WriteInt(index, 0);
                for (int i = 0; i < tactileCaches.Length; i++)
                {
                    for (int j = 0; j < tactileCaches[i].Length; j++)
                    {
                        int tactile = (int)(tactileCaches[i][j] * (1 << 15));
                        index = _sendBuffer.WriteInt(index, tactile);
                    }
                }
                _sender.SendData(_sendBuffer);
                await Task.Run(() =>
                {
                    while (_queue.IsEmpty && !_cts.IsCancellationRequested) ;
                });
                _queue.TryDequeue(out byte[] data);
                return (data, -1);
            }
            else
            {
                int index = 0;
                for (int i = 0; i < tactileCaches.Length; i++)
                {
                    for (int j = 0; j < tactileCaches[i].Length; j++)
                    {
                        float tactile = tactileCaches[i][j];
                        index = _sendBuffer.WriteFloat(index, tactile);
                    }
                }
                return (_sendBuffer, index);
            }
        }


        public struct InitParams
        {
            public bool useCompression;
            public int sendBufferSize;

            public string sendIp;
            public int sendPort;
            public string receiveIp;
            public int receivePort;
            public int receiveBufferSize;
        }
    }
}
