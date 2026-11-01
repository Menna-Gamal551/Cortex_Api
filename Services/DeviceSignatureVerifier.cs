using Cortex_Api.Services.IServices;

namespace Cortex_Api.Services
{
    public class DeviceSignatureVerifier : IDeviceSignatureVerifier
    {
        public bool Verify(string publicKey, string challenge, string signature)
        {
            throw new NotImplementedException();
        }
    }
}
