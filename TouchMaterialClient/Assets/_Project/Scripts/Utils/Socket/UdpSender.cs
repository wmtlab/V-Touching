using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace TouchMaterial
{
    public class UdpSender
    {
        private Socket _socket;
        private readonly IPEndPoint _remoteEndPoint;
        private bool _isSending = false;
        public bool IsSending => _isSending;

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
            _isSending = true;

            return true;
        }

        public bool SendData(byte[] data, int offset = 0, int length = -1)
        {
            if (_socket == null)
            {
                return false;
            }
            try
            {
                if (length == -1)
                {
                    length = data.Length;
                }
                Task.Run(async () =>
                {
                    int res = await _socket.SendToAsync(new ArraySegment<byte>(data, offset, length),
                        SocketFlags.None, _remoteEndPoint);
                });
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
            _isSending = false;
            _socket?.Shutdown(SocketShutdown.Send);
            _socket?.Close();
            _socket = null;
        }
    }
}
