using System;
using NDAPIWrapperSpace;
using UnityDLL.Haptic;
using UnityEngine;

namespace TouchMaterial.Client
{
    [Serializable]
    public class TactileActuatorController
    {
        // Customizable, need to be an array and corresponding one-to-one to the detectors in the server.
        [SerializeField]
        private Actuator[] _actuators;
        // Realization customizable.
        public int ActuatorCount => _actuators.Length;

        // Realization customizable.
        public void Play(int actuatorIdx, float value)
        {
            HapticSystem.PlayValue(value, _actuators[actuatorIdx]);
        }
        // Realization customizable.
        public void Stop(int actuatorIdx)
        {
            HapticSystem.StopActuators(_actuators[actuatorIdx]);
        }
        // Realization customizable.
        public void StopAll()
        {
            HapticSystem.StopActuators();
        }

    }
}