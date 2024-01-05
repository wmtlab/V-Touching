using NDAPIWrapperSpace;
using UnityDLL.Haptic;
using UnityEngine;

namespace TouchMaterial.Client
{
    public class TactileActuatorController : ITactileActuatorController
    {
        // Customizable, need to be an array and corresponding one-to-one to the detectors in the server.
        private Actuator[] _actuators;
        private bool[] _actuationStates;
        public int ActuatorCount => _actuators.Length;

        public TactileActuatorController()
        {
            // Customizable, need to be an array and corresponding one-to-one to the detectors in the server.
            _actuators = new Actuator[]
            {
                Actuator.ACT_INDEX,
                Actuator.ACT_MIDDLE,
                Actuator.ACT_RING,
            };
            _actuationStates = new bool[_actuators.Length];
        }

        // Realization customizable.
        public void Play(int actuatorIdx, float[] signals)
        {
            float value = 0f;
            for (int i = 0; i < signals.Length; i++)
            {
                value += Mathf.Abs(signals[i]);
            }
            value /= signals.Length;
            if (value < 0.01f)
            {
                if (_actuationStates[actuatorIdx])
                {
                    Stop(actuatorIdx);
                    _actuationStates[actuatorIdx] = false;
                }
            }
            else
            {
                HapticSystem.PlayValue(value, _actuators[actuatorIdx]);
                _actuationStates[actuatorIdx] = true;
            }

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