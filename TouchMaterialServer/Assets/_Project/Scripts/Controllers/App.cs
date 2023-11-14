using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
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

        public bool ToRecordAcc = false;
        public string RecordName;
        private StringBuilder _sb;


        void Start()
        {
            _net = LitJson.JsonMapper.ToObject<NetworkSetting>(_netJson.text);
            _poseController.Init(_net.LocalIp, _net.LocalPosePort, _net.PoseBufferSize);
            _videoController.Init(_net.RemoteIp, _net.RemoteVideoPort, _net.VideoBufferSize);
            _tactileController.Init(new TactileController.InitParams()
            {
                remoteIp = _net.RemoteIp,
                remoteTactilePort = _net.RemoteTactilePort,

                localIp = _net.LocalIp,
                encodeIp = _net.LocalIp,
                encodeSendPort = _net.EncodeSendPort,
                encodeReceivePort = _net.EncodeReceivePort,

                tactileBufferSize = _net.TactileBufferSize,
                encodedBufferSize = _net.EncodedTactileBufferSize
            });

            if (ToRecordAcc)
            {
                _sb = new StringBuilder();
                _tactileController.OnTactileFlushed += (data) =>
                {
                    _sb.Append(string.Join(";", data.Select(actuator => string.Join(",", actuator))));
                    _sb.AppendLine();
                };
            }

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
            _videoController.Stop();
            _tactileController.Stop();
        }

        void OnDestroy()
        {
            _poseController.Stop();
            _videoController.Stop();
            if (ToRecordAcc)
            {
                string record = _sb.ToString();
                string outPath = Path.Combine(Application.dataPath, "OutData", RecordName + ".txt");
                File.WriteAllText(outPath, record);
                AssetDatabase.Refresh();
            }
            _tactileController.Stop();
        }

    }
}
