using System.Threading.Tasks;

namespace TouchMaterial.Server
{
    // If you want to use your own encoder, you need to implement this interface, and then set it to the TactileController.
    public interface IEncodeHelper
    {
        void Start();
        void Stop();
        Task<(byte[], int)> EncodeAsync(float[][] tactileCaches);
    }
}