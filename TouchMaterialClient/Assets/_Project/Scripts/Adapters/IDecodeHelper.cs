using System;
using System.Threading.Tasks;

namespace TouchMaterial.Client
{
    // If you want to use your own decoder, you need to implement this interface, and then set it to the TactileController.
    public interface IDecodeHelper
    {
        void Start();
        void Stop();
        // Since the decoding process is asynchronous, you need to pass a callback to get the result.
        // float[][]: [actuator][signal]
        Task Decode(byte[] data, Action<float[][]> onFinished = null);
    }
}