using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace TouchMaterial
{
    public class UdpReceiver
    {
        private Socket _socket;
        private readonly IPEndPoint _localEndPoint;
        private readonly int _bufferSize;
        private readonly Action<byte[], int> _onReceiveData;
        private List<byte[]> _receivedSegments = new List<byte[]>();
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
                Task.Run(ReceiveDataAsync);

                return true;
            }
            catch { return false; }
        }

        private async Task ReceiveDataAsync()
        {
            while (_isReceiving)
            {
                try
                {
                    EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] buffer = new byte[_bufferSize];
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
                    var result = await _socket.ReceiveFromAsync(segment, SocketFlags.None, endPoint);
                    int index = buffer.ReadInt(0, out int isLastPacket);
                    byte[] body = new byte[result.ReceivedBytes - sizeof(int)];
                    Array.Copy(buffer, index, body, 0, body.Length);
                    if (isLastPacket == 0)
                    {
                        _receivedSegments.Add(body);
                        continue;
                    }
                    else if (_receivedSegments.Count == 0)
                    {
                        _onReceiveData?.Invoke(body, body.Length);
                    }
                    else
                    {
                        int totalLength = 0;
                        foreach (var seg in _receivedSegments)
                        {
                            totalLength += seg.Length;
                        }
                        totalLength += body.Length;
                        byte[] totalData = new byte[totalLength];
                        int offset = 0;
                        foreach (var seg in _receivedSegments)
                        {
                            Array.Copy(seg, 0, totalData, offset, seg.Length);
                            offset += seg.Length;
                        }
                        Array.Copy(body, 0, totalData, offset, body.Length);
                        _receivedSegments.Clear();
                        _onReceiveData?.Invoke(totalData, totalData.Length);
                    }


                    // _onReceiveData?.Invoke(buffer, result.ReceivedBytes);
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
