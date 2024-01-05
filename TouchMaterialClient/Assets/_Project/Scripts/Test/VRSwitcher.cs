using System;
using UnityEngine;

namespace TouchMaterial.Client
{
    [Serializable]
    public struct VRSwitcher
    {
        [SerializeField]
        private Transform _avatarHandRoot;
        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private GameObject _cameraRig;
        [SerializeField]
        private Transform _trackerRoot;
        [SerializeField]
        private Camera _vrCamera;

        [SerializeField]
        private Transform _trackerSimulatorRoot;
        [SerializeField]
        private Camera _basicCamera;

        [SerializeField]
        private bool _useVr;

        public void Init()
        {
            if (_useVr)
            {
                _trackerSimulatorRoot.gameObject.SetActive(false);
                _basicCamera.gameObject.SetActive(false);

                _cameraRig.SetActive(true);
                _trackerRoot.gameObject.SetActive(true);
                _avatarHandRoot.SetParent(_trackerRoot);
                _canvas.worldCamera = _vrCamera;
            }
            else
            {
                _cameraRig.SetActive(false);
                _trackerRoot.gameObject.SetActive(false);

                _trackerSimulatorRoot.gameObject.SetActive(true);
                _basicCamera.gameObject.SetActive(true);
                _avatarHandRoot.SetParent(_trackerSimulatorRoot);
                _canvas.worldCamera = _basicCamera;
            }
        }
    }
}
