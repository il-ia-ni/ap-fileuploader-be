using FileUploaderBackend.DTOs;
using ProLibrary.Models;
using System.Security.Cryptography;
using System.Text;

namespace FileUploaderBackend.Services
{
    public class AuthServicePSK : IAuthServicePSK
    {
        private readonly PROContext _dbContext;

        public AuthServicePSK(PROContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AuthenticateUser(string username, string password)
        {
            var user = _dbContext.TmiUsers.SingleOrDefault(u => u.Username == username);

            if (user == null || user.PasswordHash == null || user.PasswordSalt == null)
            {
                return false;
            }

            // Convert the stored binary salt and password hash to byte arrays
            byte[] storedSalt = user.PasswordSalt;
            byte[] storedHash = user.PasswordHash;

            // Generate a password hash using the stored salt
            byte[] passwordHash = HashingFunction.ComputeHash(Encoding.UTF8.GetBytes(password), storedSalt);

            // Compare the generated hash with the stored hash
            bool isPasswordValid = passwordHash.SequenceEqual(storedHash);

            return isPasswordValid;
        }

        public bool UserExists(string username)
        {
            return _dbContext.TmiUsers.Any(u => u.Username == username);
        }

        public bool CreateUser(UserLoginDto userData, string password, string role = "guest")
        {
            if (userData == null)
            {
                return false;
            }

            // Generate a new random salt
            byte[] salt = HashingFunction.GenerateSalt();

            // Compute the password hash using the provided password and the generated salt
            byte[] passwordHash = HashingFunction.ComputeHash(Encoding.UTF8.GetBytes(password), salt);

            TmiUser newUser = new TmiUser();

            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = salt;
            newUser.RoleName = role;

            try
            {
                _dbContext.TmiUsers.Add(newUser);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                // TODO: Handle any database-related exceptions here
                return false;
            }
        }

        public bool CreateUser(TmiUser newUser, string password)
        {
            throw new NotImplementedException();
        }
    }

    class HashingFunction
    {
        internal static byte[] ComputeHash(byte[] password, byte[] salt)
        {
            using (HMACSHA512 hmac = new HMACSHA512(salt))
            {
                return hmac.ComputeHash(password);
            }
        }

        internal static byte[] GenerateSalt()
        {
            byte[] salt = new byte[32]; // 32 bytes for a secure salt (adjust according to your needs)
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
