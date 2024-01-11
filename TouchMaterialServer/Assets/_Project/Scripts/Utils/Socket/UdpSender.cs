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
        private readonly int _bufferSize;
        private readonly IPEndPoint _remoteEndPoint;

        public UdpSender(string remoteIp, int remotePort, int bufferSize)
        {
            if (IPAddress.TryParse(remoteIp, out var remoteIpAddr))
            {
                _remoteEndPoint = new IPEndPoint(remoteIpAddr, remotePort);
                _bufferSize = bufferSize;
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

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                SendBufferSize = _bufferSize
            };

            return true;
        }

        public bool SendData(byte[] data, int offset, int length)
        {
            if (_socket == null)
            {
                return false;
            }
            try
            {
                Task.Run(async () =>
                {
                    if (sizeof(int) + length < _socket.SendBufferSize)
                    {
                        byte[] actualData = new byte[sizeof(int) + length];
                        int index = actualData.WriteInt(0, 1);
                        Array.Copy(data, offset, actualData, index, length);
                        int res = await _socket.SendToAsync(new ArraySegment<byte>(actualData, 0, actualData.Length),
                            SocketFlags.None, _remoteEndPoint);
                    }
                    else
                    {
                        for (int i = 0; i < length; i += _socket.SendBufferSize - sizeof(int))
                        {
                            int actualLength = Mathf.Min(_socket.SendBufferSize - sizeof(int), length - i);
                            byte[] actualData = new byte[sizeof(int) + actualLength];
                            int index = 0;
                            if (i + _socket.SendBufferSize - sizeof(int) < length)
                            {
                                index = actualData.WriteInt(0, 0);
                            }
                            else
                            {
                                index = actualData.WriteInt(0, 1);
                            }
                            Array.Copy(data, offset + i, actualData, index, actualLength);
                            int res = await _socket.SendToAsync(new ArraySegment<byte>(actualData, 0, actualData.Length),
                                SocketFlags.None, _remoteEndPoint);
                        }
                    }

                });
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"服务端发送数据异常：\n{ex.ToString()}");
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
