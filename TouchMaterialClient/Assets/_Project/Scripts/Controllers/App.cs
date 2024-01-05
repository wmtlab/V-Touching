using LitJson;
using UnityEngine;

namespace TouchMaterial.Client
{
    public class App : MonoBehaviour
    {
        [SerializeField]
        private TextAsset _netJson;
        private NetworkSetting _net;

        [SerializeField]
        private PoseController _poseController;
        [SerializeField]
        private VRSwitcher _vrSwitcher;
        [SerializeField]
        private VideoController _videoController;
        [SerializeField]
        private TactileController _tactileController;

        void Start()
        {
            _net = JsonMapper.ToObject<NetworkSetting>(_netJson.text);
            _vrSwitcher.Init();

            _poseController.Init(_net.RemoteIp, _net.RemotePosePort, _net.PoseBufferSize);
            _videoController.Init(_net.LocalIp, _net.LocalVideoPort, _net.VideoBufferSize);
            _tactileController.Init(_net.LocalIp, _net.LocalTactilePort, _net.TactileBufferSize,
                new DecodeHelper.InitParams()
                {
                    sendIp = _net.RemoteIp,
                    sendPort = _net.DecodeSendPort,
                    receiveIp = _net.LocalIp,
                    receivePort = _net.DecodeReceivePort,
                    bufferSize = _net.DecodeTactileBufferSize,
                });

            _poseController.Start();
            _videoController.Start();
            _tactileController.Start();
        }

        void FixedUpdate()
        {
            _poseController.Tick();
            _videoController.Tick();
            _tactileController.Tick();
        }

        void OnDestroy()
        {
            _poseController.Stop();
            _videoController.Stop();
            _tactileController.Stop();
        }
    }
}
