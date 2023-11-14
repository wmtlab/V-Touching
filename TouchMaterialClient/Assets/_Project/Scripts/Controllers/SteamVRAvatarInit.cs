using System;
using UnityEngine;

namespace TouchMaterial.Client
{
    [Serializable]
    public struct SteamVRAvatarInit
    {
        [SerializeField]
        private Transform _avatarHandRoot;
        [SerializeField]
        private Transform _controllerRoot;
        [SerializeField]
        private bool _useVr;

        public void Init()
        {
            if (_useVr)
            {
                _avatarHandRoot.parent = _controllerRoot;
            }
        }
    }
}
