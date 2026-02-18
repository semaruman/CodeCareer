using System.Collections.Generic;
using System.Text.Json;

namespace CodeCareer.Areas.User.Models
{
    public class UserModelDb
    {
        private readonly string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "User", "Data", "user_db.json");

        public List<UserModel> GetUserModels()
        {
            List<UserModel> users = new List<UserModel>();

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    users = JsonSerializer.Deserialize<List<UserModel>>(json) ?? new List<UserModel>();
                }
            }

            return users;
        }

        public void AddUserModel(UserModel user)
        {
            List<UserModel> users = new List<UserModel>();

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    users = JsonSerializer.Deserialize<List<UserModel>>(json) ?? new List<UserModel>();
                }
            }

            users.Add(user);
            string jsonWrite = JsonSerializer.Serialize(users);
            File.WriteAllText(filepath, jsonWrite);
        }

        public void RemoveUserModel(UserModel user)
        {
            List<UserModel> users = new List<UserModel>();

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    users = JsonSerializer.Deserialize<List<UserModel>>(json) ?? new List<UserModel>();
                }
            }

            users = users.Where(u => !(u.Email == user.Email)).ToList();
            string jsonWrite = JsonSerializer.Serialize(users);
            File.WriteAllText(filepath, jsonWrite);
        }
    }
}
