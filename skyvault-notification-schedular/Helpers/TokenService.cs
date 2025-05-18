using System.Security.Cryptography;
using System.Text;

namespace skyvault_notification_schedular.Helpers
{
    public class TokenService
    {
        public static string GenerateToken(string email)
        {

            string secret = Environment.GetEnvironmentVariable("UNSUBSCRIBE_TOKEN_SECRET");

            if (string.IsNullOrEmpty(secret))
            {
                throw new InvalidOperationException("Unsubscribe token secret not found in environment variables.");
            }

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var tokenBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(email));
            return Convert.ToBase64String(tokenBytes)
                          .Replace("+", "-")
                          .Replace("/", "_") // URL safe
                          .TrimEnd('=');
        }
    }
}
