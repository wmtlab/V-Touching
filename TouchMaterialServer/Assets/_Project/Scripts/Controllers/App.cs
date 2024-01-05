using UnityEngine;

namespace TouchMaterial.Server
{
    public class App : MonoBehaviour
    {
        [SerializeField]
        private TextAsset _netJson;
        [SerializeField]
        private PoseController _poseController;
        [SerializeField]
        private VideoController _videoController;
        [SerializeField]
        private TactileController _tactileController;

        private NetworkSetting _net;
        private bool _isSending = false;


        void Start()
        {
            _net = LitJson.JsonMapper.ToObject<NetworkSetting>(_netJson.text);
            _poseController.Init(_net.LocalIp, _net.LocalPosePort, _net.PoseBufferSize);
            _videoController.Init(_net.RemoteIp, _net.RemoteVideoPort, _net.VideoBufferSize);
            _tactileController.Init(_net.RemoteIp, _net.RemoteTactilePort, new EncodeHelper.InitParams()
            {
                sendIp = _net.LocalIp,
                sendPort = _net.EncodeSendPort,
                receiveIp = _net.LocalIp,
                receivePort = _net.EncodeReceivePort,

                sendBufferSize = _net.TactileBufferSize,
                receiveBufferSize = _net.EncodedTactileBufferSize
            });

            _poseController.Start();
        }

        void FixedUpdate()
        {
            if (_poseController.IsEmpty)
            {
                if (_isSending && _poseController.IsTimeOut)
                {
                    OnTimeout();
                    _isSending = false;
                }
                return;
            }

            _poseController.Tick();
            if (!_isSending)
            {
                OnConnected();
                _isSending = true;
            }

            _videoController.Tick();
            _tactileController.Tick();
        }

        private void OnConnected()
        {
            _poseController.CalibrateTime();
            _videoController.Start();
            _tactileController.Start();
        }

        private void OnTimeout()
        {
            Debug.Log("Pose timeout");
            _videoController.Stop();
            _tactileController.Stop();
        }

        void OnDestroy()
        {
            _poseController.Stop();
            _videoController.Stop();
            _tactileController.Stop();
        }

    }
}
