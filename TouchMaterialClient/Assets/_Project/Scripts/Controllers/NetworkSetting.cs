using System;

namespace TouchMaterial.Client
{
    [Serializable]
    public struct NetworkSetting
    {
        #region Ip 
        public string LocalIp;
        public string RemoteIp;
        #endregion

        #region Port
        public int LocalVideoPort;
        public int LocalTactilePort;
        public int RemotePosePort;
        public int DecodeSendPort;
        public int DecodeReceivePort;
        #endregion

        #region BufferSize
        public int PoseBufferSize;
        public int VideoBufferSize;
        public int TactileBufferSize;
        public int DecodeTactileBufferSize;
        #endregion

    }
}
