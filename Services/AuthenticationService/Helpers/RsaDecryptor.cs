using System.Security.Cryptography;
using System.Text;

namespace AuthenticationService.Helpers
{
    public static class RsaDecryptor
    {
        public static string Decrypt(string cipherTextBase64, string privateKeyXml)
        {
            using var rsa = RSA.Create();
            rsa.FromXmlString(privateKeyXml);

            byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);
            byte[] plainBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.Pkcs1);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
