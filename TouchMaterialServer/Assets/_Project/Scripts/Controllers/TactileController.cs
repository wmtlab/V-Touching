// #define LOG_BEFORE_CODEC

using System;
using System.IO;
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

        private float[][] _tactileCaches;   // [actuator][signal]
        private int _frameIdx;
        private float[] _emptyTactile;

        public void Init(string remoteIp, int remotePort, int tactileBufferSize)
        {
            _sender = new UdpSender(remoteIp, remotePort, tactileBufferSize);

            //_encodeHelper = new EncodeHelper(initParams, _useCompression);
            _encodeHelper = new MPEGEncodeHelper(_useCompression);

            _tactileCaches = new float[_detectors.Length][];
            for (int i = 0; i < _detectors.Length; i++)
            {
                _tactileCaches[i] = new float[_frameCount * _taxelFixedSize];
            }
            _emptyTactile = new float[_taxelFixedSize];
            for (int i = 0; i < _taxelFixedSize; i++)
            {
                _emptyTactile[i] = 0f;
            }
            _frameIdx = 0;
        }

        public void Start()
        {
            count = 0;
#if LOG_BEFORE_CODEC
            string logRoot = Path.Combine(Application.streamingAssetsPath, "Log");
            if (Directory.Exists(logRoot))
            {
                Directory.Delete(logRoot, true);
            }
            Directory.CreateDirectory(logRoot);
#endif
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
                if (signals == null)
                {
                    signals = _emptyTactile;
                }
                Array.Copy(signals, 0, _tactileCaches[i], _frameIdx * _taxelFixedSize, _taxelFixedSize);
            }
            _frameIdx++;
        }

        int count = 0;
        private async Task SendTactileAsync()
        {
#if LOG_BEFORE_CODEC
            string logFile = Path.Combine(Application.streamingAssetsPath, "Log", $"log_{count++}.txt");
            File.WriteAllText(logFile, string.Join(",", _tactileCaches[0]));
#endif
            (byte[] dataToSend, int length) = await _encodeHelper.EncodeAsync(_tactileCaches);
            _sender.SendData(dataToSend, 0, length);
        }

    }
}
