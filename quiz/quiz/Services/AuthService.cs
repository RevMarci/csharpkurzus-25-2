using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using quiz.Models;

namespace quiz.Services
{
    public class AuthService
    {
        private const string FilePath = "users.json";

        public bool register(string username, string password)
        {
            var users = getUsers();

            if (users.Any(u => u.Username == username))
            {
                return false;
            }

            var newUser = new User
            {
                Username = username,
                PasswordHash = hashPassword(password)
            };

            users.Add(newUser);
            saveUsers(users);
            return true;
        }

        public bool login(string username, string password)
        {
            var users = getUsers();
            var user = users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return false;
            }

            var inputHash = hashPassword(password);
            return user.PasswordHash == inputHash;
        }

        private string hashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        private List<User> getUsers()
        {
            if (!File.Exists(FilePath))
            {
                return new List<User>();
            }

            var json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return new List<User>();

            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        private void saveUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
