using System.Security.Cryptography;
using System.Text;

namespace Devices_tasks1_2_3.Security
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using SHA256 sha256 = SHA256.Create();

            byte[] passwordBytes =
                Encoding.UTF8.GetBytes(password);

            byte[] hashBytes =
                sha256.ComputeHash(passwordBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}