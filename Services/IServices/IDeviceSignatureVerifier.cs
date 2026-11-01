
namespace Cortex_Api.Services.IServices
{
    public interface IDeviceSignatureVerifier
    {
        bool Verify(string publicKey,string challenge, string signature);
    }
}
