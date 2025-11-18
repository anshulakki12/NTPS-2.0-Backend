using System.Security.Cryptography;

namespace AuthenticationService.Services
{
    public class RsaKeyService
    {
        private readonly RSA _rsa;

        public RsaKeyService()
        {
            _rsa = RSA.Create(2048); // demo, generate new each time
        }

        public string GetPublicKeyBase64()
        {
            var spki = _rsa.ExportSubjectPublicKeyInfo();
            return Convert.ToBase64String(spki);
        }

        public byte[] DecryptKey(byte[] encryptedKey) =>
            _rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
    }
}
