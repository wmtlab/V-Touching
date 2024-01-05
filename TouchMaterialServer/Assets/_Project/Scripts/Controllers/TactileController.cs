using System;
using System.Threading.Tasks;
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
        private int _taxelFixedSize = 8;
        [SerializeField]
        private bool _useCompression;

        private IEncodeHelper _encodeHelper;
        private UdpSender _sender;

        private float[][] _tactileCaches;   // [actuator][frame]
        private int _frameIdx;

        public void Init(string remoteIp, int remotePort, EncodeHelper.InitParams initParams)
        {
            _sender = new UdpSender(remoteIp, remotePort);

            initParams.useCompression = _useCompression;
            _encodeHelper = new EncodeHelper(initParams);

            _tactileCaches = new float[_detectors.Length][];
            for (int i = 0; i < _detectors.Length; i++)
            {
                _tactileCaches[i] = new float[_frameCount * _taxelFixedSize];
            }
            _frameIdx = 0;
        }

        public void Start()
        {
            _encodeHelper?.Start();
            _sender?.Start();
        }

        public void Stop()
        {
            _encodeHelper?.Stop();
            _sender?.Close();
        }

        public void Tick()
        {
            UpdateTactile();
            if (_frameIdx == _frameCount)
            {
                Task.Run(SendTactileAsync);
            }
        }

        private void UpdateTactile()
        {
            if (_frameIdx == _frameCount)
            {
                _frameIdx = 0;
            }

            for (int i = 0; i < _detectors.Length; i++)
            {
                float[] signals = _detectors[i].GetTactile();
                Array.Copy(signals, 0, _tactileCaches[i], _frameIdx * _taxelFixedSize, _taxelFixedSize);
            }
            _frameIdx++;
        }

        private async Task SendTactileAsync()
        {
            (byte[] dataToSend, int length) = await _encodeHelper.EncodeAsync(_tactileCaches);
            _sender.SendData(dataToSend, 0, length);
        }

    }
}
