using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace TouchMaterial.Server
{
    [Serializable]
    public class PoseController
    {
        [SerializeField]
        private Transform[] _syncObjects;
        [SerializeField, Range(0f, 10f)]
        private float _timeOutSec;

        private UdpReceiver _receiver;
        private ConcurrentQueue<byte[]> _queue;

        private float _timeOffset;
        private float _lastPoseTime;

        public bool IsEmpty => _queue?.IsEmpty ?? true;
        public bool IsTimeOut => Time.fixedTime - _lastPoseTime > _timeOutSec;


        public void Init(string localIp, int localPort, int bufferSize)
        {
            _queue = new ConcurrentQueue<byte[]>();
            _receiver = new UdpReceiver(localIp, localPort, bufferSize, (data, length) =>
            {
                _queue.Enqueue(data);
            });
            _timeOffset = 0f;
        }

        public void Start()
        {
            _receiver?.Start();
        }

        public void Stop()
        {
            _receiver?.Close();
        }

        public void Tick()
        {
            byte[] data = null;
            while (!_queue.IsEmpty)
            {
                _queue.TryDequeue(out data);
            }
            if (data == null)
            {
                return;
            }
            int index = 0;
            index = data.ReadFloat(index, out float tick);
            _lastPoseTime = tick + _timeOffset;
            index = DeserializePose(data, index);
        }

        private int DeserializePose(byte[] buffer, int start)
        {
            int index = start;

            try
            {
                foreach (var transform in _syncObjects)
                {
                    index = buffer.ReadVector3(index, out var position);
                    index = buffer.ReadQuaternion(index, out var rotation);
                    transform.position = position;
                    transform.rotation = rotation;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.LogWarning(ex.ToString());
            }

            return index;
        }

        public void CalibrateTime()
        {
            _timeOffset = Time.fixedTime - _lastPoseTime;
        }

    }
}
