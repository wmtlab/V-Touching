using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TouchMaterial
{
    public class UdpSender
    {
        private Socket _socket;
        private readonly IPEndPoint _remoteEndPoint;

        public UdpSender(string remoteIp, int remotePort)
        {
            if (IPAddress.TryParse(remoteIp, out var remoteIpAddr))
            {
                _remoteEndPoint = new IPEndPoint(remoteIpAddr, remotePort);
            }
            else
            {
                _remoteEndPoint = null;
                _socket = null;
            }
        }

        public bool Start()
        {
            if (_remoteEndPoint == null)
            {
                Debug.LogError("RemoteEndPoint为null");
                return false;
            }
            if (_socket != null)
            {
                Debug.LogError("已存在Socket");
                return false;
            }

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            return true;
        }

        public bool SendData(byte[] data)
        {
            if (_socket == null)
            {
                return false;
            }
            try
            {
                UniTask.RunOnThreadPool(() =>
                {
                    _socket.SendTo(data, _remoteEndPoint);
                }).Forget();
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"客户端发送数据异常：\n{ex.ToString()}");
                return false;
            }
        }

        public void Close()
        {
            _socket?.Shutdown(SocketShutdown.Send);
            _socket?.Close();
            _socket = null;
        }
    }
}
