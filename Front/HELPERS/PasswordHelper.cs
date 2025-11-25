using System.Security.Cryptography;
using System.Text;

namespace Front.Helpers
{
    public static class PasswordHelper
    {
        // Convierte la contraseña en hash SHA256
        public static byte[] HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Convierte el hash (bytes) a formato hexadecimal para SQL Server
        public static string HashToHex(byte[] hash)
        {
            return "0x" + BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
