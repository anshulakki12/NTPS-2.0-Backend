using System.Security.Cryptography;

namespace AuthenticationService.Helpers
{
    public static class RsaKeyGenerator
    {
        public static void GenerateKeys(out string publicKey, out string privateKey)
        {
            using var rsa = RSA.Create(2048);

            // Export keys in XML format
            publicKey = rsa.ToXmlString(false); // Public key only
            privateKey = rsa.ToXmlString(true); // Private + Public
        }
    }
}
