using System;

namespace TouchMaterial.Client
{
    [Serializable]
    public struct NetworkSetting
    {
        #region Ip
        public string RemoteIp;
        public string LocalIp;
        #endregion

        #region Port
        public int RemotePosePort;
        public int LocalVideoPort;
        public int LocalTactilePort;
        public int DecodeSendPort;
        public int DecodeReceivePort;
        #endregion

        #region BufferSize
        public int PoseBufferSize;
        public int VideoBufferSize;
        public int TactileBufferSize;
        public int EncodedTactileBufferSize;
        #endregion

    }
}
