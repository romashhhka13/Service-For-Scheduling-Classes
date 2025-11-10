using BCrypt.Net;

namespace ScheduleMaster.Helpers
{
    public class PasswordHasher
    {
        static public string Generate(string password)
        {
            // SHA-384 + bcrypt (Blowfish)
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        static public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }
    }
}