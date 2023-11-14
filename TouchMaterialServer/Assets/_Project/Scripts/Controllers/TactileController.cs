using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TouchMaterial.Server
{
    [Serializable]
    public class TactileController
    {
        [SerializeField]
        private TactileDetector[] _detectors;
        [SerializeField, Range(1, 1024)]
        private int _frameCount = 1;
        [SerializeField]
        private bool _useCompression;

        private UdpSender _tactileSender;
        private UdpSender _encodeSender;
        private UdpReceiver _encodeReceiver;

        private float[][] _tactileCaches;   // [actuator][frame]
        private int _frameIdx;
        private byte[] _tactileBuffer;

        public event Action<float[][]> OnTactileFlushed;

        public void Init(InitParams initParams)
        {
            _tactileSender = new UdpSender(initParams.remoteIp, initParams.remoteTactilePort);
            if (_useCompression)
            {
                _encodeSender = new UdpSender(initParams.encodeIp, initParams.encodeSendPort);
                _encodeReceiver = new UdpReceiver(
                    initParams.localIp, initParams.encodeReceivePort, initParams.encodedBufferSize,
                    (data, length) =>
                    {
                        _tactileSender.SendData(data);
                    });
            }
            _tactileBuffer = new byte[initParams.tactileBufferSize];
        }

        public void Start()
        {
            if (_useCompression)
            {
                _encodeReceiver?.Start();
                _encodeSender?.Start();
            }
            _tactileSender?.Start();
        }

        public void Stop()
        {
            if (_useCompression)
            {
                _encodeSender?.Close();
                _encodeReceiver?.Close();
            }
            _tactileSender?.Close();
        }

        public void Tick()
        {
            UpdateTactile();
            if (_frameIdx == _frameCount)
            {
                SendTactile();
            }
        }

        private void UpdateTactile()
        {
            if (_tactileCaches == null)
            {
                _tactileCaches = new float[_detectors.Length][];
                for (int i = 0; i < _detectors.Length; i++)
                {
                    _tactileCaches[i] = new float[_frameCount];
                }
                _frameIdx = 0;
            }
            if (_frameIdx == _frameCount)
            {
                OnTactileFlushed?.Invoke(_tactileCaches);
                _frameIdx = 0;
            }

            for (int i = 0; i < _detectors.Length; i++)
            {
                _tactileCaches[i][_frameIdx] = _detectors[i].GetTactileUnsigned();
            }
            _frameIdx++;
        }

        private int SerializeTactile(byte[] buffer, int start)
        {
            if (_frameIdx != _frameCount)
            {
                return -1;
            }
            int index = start;
            try
            {
                if (_useCompression)
                {
                    index = buffer.WriteInt(index, 0);
                    for (int i = 0; i < _tactileCaches.Length; i++)
                    {
                        for (int j = 0; j < _tactileCaches[i].Length; j++)
                        {
                            int tactile = Float2IntScaled(_tactileCaches[i][j]);
                            index = buffer.WriteInt(index, tactile);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < _tactileCaches.Length; i++)
                    {
                        for (int j = 0; j < _tactileCaches[i].Length; j++)
                        {
                            float tactile = _tactileCaches[i][j];
                            index = buffer.WriteFloat(index, tactile);
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.LogWarning(ex.ToString());
            }

            return index;
        }

        private void SendTactile()
        {
            SerializeTactile(_tactileBuffer, 0);
            if (_useCompression)
            {
                _encodeSender.SendData(_tactileBuffer);
            }
            else
            {
                _tactileSender.SendData(_tactileBuffer);
            }
        }

        private static int Float2IntScaled(float f)
        {
            int ret = (int)(f * (1 << 15));
            return ret;// == 0 ? 1 : ret;
        }

        public struct InitParams
        {
            public string remoteIp;
            public int remoteTactilePort;

            public string localIp;
            public string encodeIp;
            public int encodeSendPort;
            public int encodeReceivePort;

            public int tactileBufferSize;
            public int encodedBufferSize;
        }

    }
}
