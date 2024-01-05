namespace TouchMaterial.Client
{
    // If you want to use other tactile glove, you need to implement this interface, and then set it to the TactileController.
    public interface ITactileActuatorController
    {
        int ActuatorCount { get; }
        void Play(int actuatorIdx, float[] signals);
        void Stop(int actuatorIdx);
        void StopAll();
    }
}