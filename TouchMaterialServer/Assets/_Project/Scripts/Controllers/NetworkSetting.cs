using System;

namespace TouchMaterial.Server
{
    [Serializable]
    public struct NetworkSetting
    {
        #region Ip
        public string LocalIp;
        public string RemoteIp;
        #endregion

        #region Port
        public int LocalPosePort;
        public int RemoteVideoPort;
        public int RemoteTactilePort;
        public int EncodeSendPort;
        public int EncodeReceivePort;
        #endregion

        #region BufferSize
        public int PoseBufferSize;
        public int VideoBufferSize;
        public int TactileBufferSize;
        public int EncodedTactileBufferSize;
        #endregion

    }
}
