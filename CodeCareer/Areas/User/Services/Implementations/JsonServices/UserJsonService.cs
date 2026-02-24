using System.Collections.Generic;
using System.Text.Json;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;

namespace CodeCareer.Areas.User.Services.Implementations.JsonServices
{
    public class UserJsonService : IUserService
    {
        private readonly string _filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "User", "Data", "JsonFiles", "user_db.json");

        public List<UserModel> GetUserModels()
        {
            List<UserModel> users = new List<UserModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
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

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    users = JsonSerializer.Deserialize<List<UserModel>>(json) ?? new List<UserModel>();
                }
            }

            users.Add(user);
            string jsonWrite = JsonSerializer.Serialize(users);
            File.WriteAllText(_filepath, jsonWrite);
        }

        public void RemoveUserModel(UserModel user)
        {
            List<UserModel> users = new List<UserModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    users = JsonSerializer.Deserialize<List<UserModel>>(json) ?? new List<UserModel>();
                }
            }

            users = users.Where(u => !(u.Email == user.Email)).ToList();
            string jsonWrite = JsonSerializer.Serialize(users);
            File.WriteAllText(_filepath, jsonWrite);
        }

        public void UpdateUserModel(UserModel user)
        {
            List<UserModel> users = new List<UserModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    users = JsonSerializer.Deserialize<List<UserModel>>(json) ?? new List<UserModel>();
                }
            }

            users = users.Where(u => !(u.Email == user.Email)).ToList();
            users.Add(user);

            string jsonWrite = JsonSerializer.Serialize(users);
            File.WriteAllText(_filepath, jsonWrite);
        }
    }
}
