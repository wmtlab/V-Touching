using System;
using System.Collections.Concurrent;
using System.Linq;
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
        private SteamVRAvatarInit _steamVrAvatarInit;
        [SerializeField]
        private VideoController _videoController;
        [SerializeField]
        private TactileController _tactileController;

        void Start()
        {
            _net = JsonMapper.ToObject<NetworkSetting>(_netJson.text);
            _steamVrAvatarInit.Init();

            _poseController.Init(_net.RemoteIp, _net.RemotePosePort, _net.PoseBufferSize);
            _videoController.Init(_net.LocalIp, _net.LocalVideoPort, _net.VideoBufferSize);
            _tactileController.Init(new TactileController.InitParams()
            {
                localIp = _net.LocalIp,
                localTactilePort = _net.LocalTactilePort,
                decodeIp = _net.RemoteIp,
                decodeSendPort = _net.DecodeSendPort,
                decodeReceivePort = _net.DecodeReceivePort,
                tactileBufferSize = _net.TactileBufferSize,
                encodedBufferSize = _net.EncodedTactileBufferSize,
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
