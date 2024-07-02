// #define LOG_AFTER_CODEC

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace TouchMaterial.Client
{
    [Serializable]
    public class TactileController
    {
        private ITactileActuatorController _actuatorController;
        [SerializeField, Range(1, 1024)]
        private int _frameCount = 1;
        [SerializeField]
        private int _taxelFixedSize = 8;
        [SerializeField]
        private bool _useCompression;

        private UdpReceiver _receiver;
        private IDecodeHelper _decodeHelper;

        private ConcurrentQueue<byte[]> _queue;

        private float[][] _tactileCaches;   // [actuator][signal]
        private float[] _curSignals;
        private int _frameIdx;


        public void Init(string localIp, int localPort, int bufferSize)
        {
            _actuatorController = new TactileActuatorController();
            _queue = new ConcurrentQueue<byte[]>();
            _receiver = new UdpReceiver(localIp, localPort, bufferSize,
                (data, length) =>
                {
                    _queue.Enqueue(data);
                });

            _decodeHelper = new MPEGDecodeHelper(
                _actuatorController.ActuatorCount, _frameCount, _taxelFixedSize, _useCompression);
            //_decodeHelper = new DecodeHelper(initParams, _useCompression);

            _curSignals = new float[_taxelFixedSize];
            _frameIdx = 0;
        }

        public void Start()
        {
#if LOG_AFTER_CODEC
            count = 0;
            string logRoot = Path.Combine(Application.streamingAssetsPath, "Log");
            if (Directory.Exists(logRoot))
            {
                Directory.Delete(logRoot, true);
            }
            Directory.CreateDirectory(logRoot);
#endif
            _decodeHelper?.Start();
            _receiver?.Start();
        }

        public void Stop()
        {
            _actuatorController.StopAll();
            _decodeHelper?.Stop();
            _receiver?.Close();
        }

        public void Tick()
        {
            byte[] data = null;
            while (!_queue.IsEmpty)
            {
                _queue.TryDequeue(out data);
            }
            Task.Run(() => _decodeHelper.Decode(data, OnDecodeFinished));
            if (_frameIdx < _frameCount)
            {
                UpdateTactile();
            }
        }

        int count = 0;
        private void OnDecodeFinished(float[][] tactileCaches)
        {
            _tactileCaches = tactileCaches;
#if LOG_AFTER_CODEC
            string logFile = Path.Combine(Application.streamingAssetsPath, "Log", $"log_{count++}.txt");
            File.WriteAllText(logFile, string.Join(",", tactileCaches[0]));
#endif
            _frameIdx = 0;
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

            for (int i = 0; i < _tactileCaches.Length; i++)
            {
                int startIdx = _frameIdx * _taxelFixedSize;
                for (int j = 0; j < _taxelFixedSize; j++)
                {
                    _curSignals[j] = _tactileCaches[i][startIdx + j];
                }
                _actuatorController.Play(i, _curSignals);
            }
            _frameIdx++;
        }
    }
}
