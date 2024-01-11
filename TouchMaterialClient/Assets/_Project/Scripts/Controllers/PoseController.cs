using System;
using UnityEngine;

namespace TouchMaterial.Client
{
    [Serializable]
    public class PoseController
    {
        [SerializeField]
        private Transform[] _syncObjects;
        private UdpSender _sender;
        private byte[] _buffer;

        public void Init(string remoteIp, int remotePort, int bufferSize)
        {
            _sender = new UdpSender(remoteIp, remotePort, bufferSize);
            _buffer = new byte[bufferSize];
        }

        public void Start()
        {
            _sender?.Start();
        }

        public void Stop()
        {
            _sender?.Close();
        }

        public void Tick()
        {
            if (!_sender.IsSending)
            {
                return;
            }
            int index = 0;
            index = _buffer.WriteFloat(index, Time.fixedTime);
            index = SerializePose(_buffer, index);
            _sender.SendData(_buffer, 0, index);
        }

        private int SerializePose(byte[] buffer, int start)
        {
            int index = start;

            try
            {
                foreach (var transform in _syncObjects)
                {
                    index = buffer.WriteVector3(index, transform.position);
                    index = buffer.WriteQuaternion(index, transform.rotation);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.LogWarning(ex.ToString());
            }

            return index;
        }
    }
}
