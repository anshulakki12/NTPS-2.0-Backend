using System.Security.Cryptography;
using System.Text;

public static class AesDecryptor
{
    public static string Decrypt(string cipherTextBase64, string keyBase64, string ivBase64)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);
        byte[] keyBytes = Convert.FromBase64String(keyBase64);
        byte[] ivBytes = Convert.FromBase64String(ivBase64);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(cipherBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);
        return sr.ReadToEnd();
    }
}
