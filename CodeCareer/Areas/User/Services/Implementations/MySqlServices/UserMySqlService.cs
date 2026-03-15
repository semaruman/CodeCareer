using System.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlServices
{
    // класс для работы с таблицей users при помощи ADO.NET
    public class UserMySqlService : IUserService
    {
        private ITagService _tagService;
        public UserMySqlService(ITagService tagService)
        {
            _tagService = tagService;
        }

        public List<UserModel> GetUserModels()
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"
SELECT *
FROM users
";

            using var command = new MySqlCommand(sqlQuery, connection);

            using var reader = command.ExecuteReader();

            List<UserModel> users = new List<UserModel>();

            while (reader.Read())
            {
                HashSet<TagModel> skillTags;
                var tagNames = reader.GetString("skill_tags_names");

                if (tagNames != null && tagNames.Length > 0) {
                    skillTags = _tagService.GetTagModels().Where(t => tagNames.Contains(t.Name)).ToHashSet();
                }
                else
                {
                    skillTags = new HashSet<TagModel>();
                }

                UserModel model = new UserModel
                {
                    Id = reader.GetInt32("id"),
                    FullName = reader.GetString("full_name"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password"),
                    BirthDate = reader.GetDateTime("birth_date"),
                    Info = reader.GetString("info"),
                    Rating = reader.GetInt32("rating"),
                    Subscribers = reader.GetInt32("subscribers"),
                    SubscribersEmails = reader.GetString("subscribers_emails").Split("; ").ToHashSet(),
                    Subscriptions = reader.GetInt32("subscriptions"),
                    SubscriptionsEmails = reader.GetString("subscriptions_emails").Split("; ").ToHashSet(),
                    Status = reader.GetString("status"),
                    SkillTags = skillTags,
                    ShowSubscriptions = reader.GetBoolean("show_subscriptions"),
                    RegistrationDate = reader.IsDBNull("registration_date") ? DateTime.Now : reader.GetDateTime("registration_date")
                };

                users.Add(model);
            }

            return users;
        }

        public void AddUserModel(UserModel user)
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"
INSERT INTO users(full_name, email, password, birth_date, info, rating, subscribers, subscribers_emails, subscriptions, subscriptions_emails, status, skill_tag_names, show_subscriptions, registration_date) VALUES
    (@fullName, @email, @password, @birthDate, @info, @rating, @subscribers, @subscribersEmails, @subscriptions, @subscriptionsEmails, @status, @skillTagNames, @showSubscriptions, @registrationDate)
";

            string subscribersEmails = user.SubscribersEmails.Aggregate((x, y) => x + "; " + y);
            string subscriptionsEmails = user.SubscriptionsEmails.Aggregate((x, y) => x + "; " + y);
            string skillTagNames = user.SkillTags.Select(t => t.Name).Aggregate((x, y) => x + "; " + y);

            using var command = new MySqlCommand(sqlQuery, connection);

            command.Parameters.AddWithValue("@fullName", user.FullName);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password", user.Password);
            command.Parameters.AddWithValue("@birthDate", user.BirthDate);
            command.Parameters.AddWithValue("@info", user.Info);
            command.Parameters.AddWithValue("@rating", user.Rating);
            command.Parameters.AddWithValue("@subscribers", user.Subscribers);
            command.Parameters.AddWithValue("@subscribersEmails", subscribersEmails);
            command.Parameters.AddWithValue("@subscriptions", user.Subscriptions);
            command.Parameters.AddWithValue("@subscriptionsEmails", subscriptionsEmails);
            command.Parameters.AddWithValue("@status", user.Status);
            command.Parameters.AddWithValue("@skillTagNames", skillTagNames);
            command.Parameters.AddWithValue("@showSubscriptions", user.ShowSubscriptions);
            command.Parameters.AddWithValue("@registrationDate", user.RegistrationDate);

            command.ExecuteNonQuery();
        }

        public void RemoveUserModel(int userId)
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"
DELETE FROM users WHERE id = @userId
";

            using var command = new MySqlCommand(sqlQuery, connection);

            command.Parameters.AddWithValue("@userId", userId);

            command.ExecuteNonQuery();
        }
    }
}
