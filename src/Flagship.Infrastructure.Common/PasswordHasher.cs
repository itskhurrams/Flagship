using System.Security.Cryptography;

namespace Flagship.Infrastructure.Common {
    public static class PasswordHasher {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 210_000;

        public static string Hash(string password) {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
            return string.Join(".", Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
        }

        public static bool Verify(string password, string hashedPassword) {
            var parts = hashedPassword.Split('.', 3);
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out var iterations)) return false;

            byte[] salt, expectedKey;
            try {
                salt = Convert.FromBase64String(parts[1]);
                expectedKey = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException) {
                return false;
            }

            var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedKey.Length);
            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }
    }
}
