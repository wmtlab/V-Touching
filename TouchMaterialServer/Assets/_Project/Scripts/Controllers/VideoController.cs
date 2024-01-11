using System;
using UnityEngine;

namespace TouchMaterial.Server
{
    [Serializable]
    public class VideoController
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField, Range(1, 10)]
        private int _intervalFrames;

        private UdpSender _sender;
        private RenderTexture _rt;
        private Texture2D _texture;

        private int _frameCounter = 0;
        private int _gcCounter = 0;
        private int _bufferSize;


        public void Init(string remoteIp, int remotePort, int bufferSize)
        {
            _sender = new UdpSender(remoteIp, remotePort, bufferSize);
            _bufferSize = bufferSize;
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
            if (_frameCounter == 0)
            {
                byte[] data = CaptureWindow();
                if (data.Length < _bufferSize)
                {
                    _sender.SendData(data, 0, data.Length);
                }
            }
            _frameCounter = (_frameCounter + 1) % _intervalFrames;
        }

        private byte[] CaptureWindow()
        {
            if (_rt == null)
            {
                _rt = new RenderTexture(480, 480, 24)
                {
                    enableRandomWrite = true
                };
                _camera.targetTexture = _rt;
                _texture = new Texture2D(480, 480, TextureFormat.RGB24, false);
                _gcCounter = 0;
            }

            RenderTexture.active = _rt;
            _texture.ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
            RenderTexture.active = null;
            byte[] ret = _texture.EncodeToJPG(20);

            _gcCounter++;
            if (_gcCounter >= 1000)
            {
                GC.Collect();
                _gcCounter = 0;
            }

            return ret;
        }

    }
}
