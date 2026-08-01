using System.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlAdoNetServices
{
    public class CommentMySqlAdoNetService : ICommentService
    {
        private readonly IUserService _userService;

        public CommentMySqlAdoNetService(IUserService userService)
        {
            _userService = userService;
        }

        public List<CommentModel> GetByPublicationId(int publicationId)
        {
            EnsureTable();

            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            const string sql = @"
SELECT id, publication_id, user_email, content, created_date
FROM comments
WHERE publication_id = @publicationId
ORDER BY created_date ASC, id ASC
";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@publicationId", publicationId);

            using var reader = command.ExecuteReader();
            var usersByEmail = _userService.GetUserModels()
                .Where(u => !string.IsNullOrEmpty(u.Email))
                .GroupBy(u => u.Email)
                .ToDictionary(g => g.Key, g => g.First());

            var result = new List<CommentModel>();
            while (reader.Read())
            {
                var email = reader.GetString("user_email");
                usersByEmail.TryGetValue(email, out var user);
                result.Add(new CommentModel
                {
                    Id = reader.GetInt32("id"),
                    PublicationId = reader.GetInt32("publication_id"),
                    User = user ?? new UserModel { Email = email, FullName = email },
                    Content = reader.GetString("content"),
                    CreatedDate = reader.GetDateTime("created_date"),
                });
            }

            return result;
        }

        public void Add(CommentModel comment)
        {
            EnsureTable();

            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            const string sql = @"
INSERT INTO comments (publication_id, user_email, content, created_date)
VALUES (@publicationId, @userEmail, @content, @createdDate)
";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@publicationId", comment.PublicationId);
            command.Parameters.AddWithValue("@userEmail", comment.User.Email);
            command.Parameters.AddWithValue("@content", comment.Content);
            command.Parameters.AddWithValue("@createdDate", comment.CreatedDate == default ? DateTime.Now : comment.CreatedDate);
            command.ExecuteNonQuery();
        }

        private static void EnsureTable()
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();
            const string sql = @"
CREATE TABLE IF NOT EXISTS comments (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    publication_id INT NOT NULL,
    user_email VARCHAR(256) NOT NULL,
    content TEXT NOT NULL,
    created_date DATETIME NOT NULL,
    INDEX IX_comments_publication_id (publication_id)
)";
            using var command = new MySqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}
