using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TouchMaterial.Client
{
    [Serializable]
    public class VideoController
    {
        [SerializeField]
        private RawImage _rawImage;
        [SerializeField, Range(1, 10)]
        private int _intervalFrames;

        private UdpReceiver _receiver;
        private ConcurrentQueue<byte[]> _queue;
        private Texture2D _texture;

        private int _frameCounter = 0;
        private int _gcCounter = 0;

        public void Init(string localIp, int localVideoPort, int bufferSize)
        {
            _queue = new ConcurrentQueue<byte[]>();
            _receiver = new UdpReceiver(
                localIp, localVideoPort, bufferSize,
                (data, length) =>
                {
                    _queue.Enqueue(data.Take(length).ToArray());
                });
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
            if (_frameCounter == 0)
            {
                byte[] data = DeserializeVideo();
                Render(data);
            }
            _frameCounter = (_frameCounter + 1) % _intervalFrames;
        }

        private byte[] DeserializeVideo()
        {
            if (_queue.Count == 0)
            {
                return null;
            }
            byte[] data = null;
            while (_queue.Count > 0)
            {
                _queue.TryDequeue(out data);
            }

            return data;
        }

        private void Render(byte[] data)
        {
            if (data == null)
            {
                return;
            }
            if (_texture == null)
            {
                _texture = new Texture2D(480, 480);
                _gcCounter = 0;
            }
            _texture.LoadImage(data);
            _rawImage.texture = _texture;
            _gcCounter++;
            if (_gcCounter >= 1000)
            {
                GC.Collect();
            }
        }
    }
}
