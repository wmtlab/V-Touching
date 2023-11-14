using System;
using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TouchMaterial
{
    public class UdpReceiver
    {
        private Socket _socket;
        private readonly IPEndPoint _localEndPoint;
        private readonly int _bufferSize;
        private readonly Action<byte[], int> _onReceiveData;
        private bool _isReceiving;
        public bool IsReceiving => _isReceiving;
        public UdpReceiver(string localIp, int localPort, int bufferSize, Action<byte[], int> onReceiveData)
        {
            if (IPAddress.TryParse(localIp, out var localIpAddr))
            {
                _localEndPoint = new IPEndPoint(localIpAddr, localPort);
                _bufferSize = bufferSize;
                _onReceiveData = onReceiveData;
            }
            else
            {
                _localEndPoint = null;
                _socket = null;
            }
        }

        public bool Start()
        {
            if (_localEndPoint == null)
            {
                return false;
            }

            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.Bind(_localEndPoint);
                _isReceiving = true;
                UniTask.RunOnThreadPool(ReceiveData).Forget();

                return true;
            }
            catch { return false; }
        }

        private void ReceiveData()
        {
            while (_isReceiving)
            {
                try
                {
                    EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] buffer = new byte[_bufferSize];
                    int length = _socket.ReceiveFrom(buffer, ref endPoint);
                    _onReceiveData?.Invoke(buffer, length);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
        }

        public void Close()
        {
            _isReceiving = false;
            _socket?.Shutdown(SocketShutdown.Receive);
            _socket?.Close();
            _socket = null;
        }
    }
}
