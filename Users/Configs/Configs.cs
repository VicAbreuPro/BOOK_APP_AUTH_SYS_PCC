using System.Security.Cryptography;
using System.Text;

namespace Users.Configs
{
    public class Configs
    {
        public static string EncryptPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                string hashString = Convert.ToBase64String(hashBytes);

                return hashString;
            }
        }
    }
}
