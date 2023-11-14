using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;

namespace TouchMaterial.Client
{
    [Serializable]
    public class TactileController
    {
        [SerializeField]
        private TactileActuatorController _actuatorController;
        [SerializeField, Range(1, 1024)]
        private int _frameCount = 1;
        [SerializeField]
        private bool _useCompression;

        private UdpReceiver _tactileReceiver;
        private UdpReceiver _encodedReceiver;
        private UdpSender _decodeSender;

        private ConcurrentQueue<byte[]> _queue;
        private byte[] _encodedBuffer;

        private float[][] _tactileCaches;   // [actuator][frame]
        private bool[] _actuationStates;
        private int _frameIdx;

        public void Init(InitParams initParams)
        {
            _encodedBuffer = new byte[initParams.encodedBufferSize];
            _queue = new ConcurrentQueue<byte[]>();

            if (_useCompression)
            {
                _encodedReceiver = new UdpReceiver(
                    initParams.localIp, initParams.localTactilePort, initParams.encodedBufferSize,
                    (data, length) =>
                    {
                        _queue.Enqueue(data);
                    });
                _decodeSender = new UdpSender(initParams.decodeIp, initParams.decodeSendPort);
                _tactileReceiver = new UdpReceiver(
                    initParams.localIp, initParams.decodeReceivePort, initParams.tactileBufferSize,
                    DeserializeTactile);
            }
            else
            {
                _tactileReceiver = new UdpReceiver(
                    initParams.localIp, initParams.localTactilePort, initParams.tactileBufferSize,
                    (data, length) =>
                    {
                        _queue.Enqueue(data);
                    });
            }
        }

        public void Start()
        {
            if (_useCompression)
            {
                _decodeSender?.Start();
                _encodedReceiver?.Start();
            }

            _tactileReceiver?.Start();
        }

        public void Stop()
        {
            _actuatorController.StopAll();

            if (_useCompression)
            {
                _decodeSender?.Close();
                _encodedReceiver?.Close();
            }

            _tactileReceiver?.Close();
        }

        public void Tick()
        {
            byte[] data = null;
            while (_queue.Count > 0)
            {
                _queue.TryDequeue(out data);
            }
            if (_useCompression)
            {
                DecodeTactile(data);
            }
            else
            {
                DeserializeTactile(data, default);
            }
            if (_frameIdx < _frameCount)
            {
                UpdateTactile();
            }
        }

        private void DecodeTactile(byte[] data)
        {
            if (data == null)
            {
                return;
            }
            _encodedBuffer.WriteInt(0, 1);
            data.Take(data.Length - sizeof(int)).ToArray().CopyTo(_encodedBuffer, sizeof(int));
            _decodeSender.SendData(_encodedBuffer);
        }

        private void DeserializeTactile(byte[] buffer, int length)
        {
            if (buffer == null)
            {
                return;
            }

            if (_actuationStates == null)
            {
                _actuationStates = new bool[_actuatorController.ActuatorCount];
                _tactileCaches = new float[_actuatorController.ActuatorCount][];
                for (int i = 0; i < _actuatorController.ActuatorCount; i++)
                {
                    _tactileCaches[i] = new float[_frameCount];
                }
                _frameIdx = 0;
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
                            float intensity = Int2FloatScaled(intIntensity);
                            _tactileCaches[i][j] = intensity;
                            continue;
                        }
                        else
                        {
                            index = buffer.ReadFloat(index, out var intensity);
                            _tactileCaches[i][j] = intensity;
                        }
                    }

                    _frameIdx = 0;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.LogWarning(ex.ToString());
            }
        }

        private void UpdateTactile()
        {
            if (_tactileCaches == null)
            {
                return;
            }
            if (_frameIdx >= _frameCount)
            {
                return;
            }

            for (int i = 0; i < _actuatorController.ActuatorCount; i++)
            {
                float tactile = Mathf.Abs(_tactileCaches[i][_frameIdx]);
                if (Mathf.Approximately(tactile, 0f))
                {
                    if (_actuationStates[i])
                    {
                        _actuatorController.Stop(i);
                        _actuationStates[i] = false;
                    }
                }
                else
                {
                    _actuatorController.Play(i, tactile);
                    _actuationStates[i] = true;
                }
            }
            _frameIdx++;
        }

        private float Int2FloatScaled(int i)
        {
            float ret = ((float)i) / (1 << 15);
            return 1f - Mathf.Abs(ret) < 1e-4f ? 0f : ret;
        }

        public struct InitParams
        {
            public string localIp;
            public int localTactilePort;

            public string decodeIp;
            public int decodeSendPort;
            public int decodeReceivePort;

            public int tactileBufferSize;
            public int encodedBufferSize;
        }
    }
}
