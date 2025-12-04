using System.Security.Cryptography;
using System.Text;

namespace AuthenticationService.Services
{
    public class PasswodService : IPasswordService
    {
        public string HashPassword(string password)
        {
            // SHA512 hashing (you can switch to BCrypt/Argon2 later)
            using var sha = SHA512.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }
}
