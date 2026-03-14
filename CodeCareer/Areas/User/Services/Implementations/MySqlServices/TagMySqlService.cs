using System.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlServices
{
    // Класс для работы с таблицей tags. Использовал ADO.NET
    public class TagMySqlService : ITagService
    {
        public List<TagModel> GetTagModels()
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"
SELECT id, name, img_path
FROM tags
";

            using var command = new MySqlCommand(sqlQuery, connection);

            using var reader = command.ExecuteReader();

            List<TagModel> tagModels = new List<TagModel>();

            while (reader.Read())
            {
                TagModel tagModel = new TagModel()
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    ImgPath = reader.IsDBNull("img_path") ? Constants.DEFAULT_TAG_IMG_PATH : reader.GetString("img_path")
                };
            }

            return tagModels;
        }

        public void AddTagModel(TagModel tagModel)
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"
INSERT INTO tags (name, img_path) VALUES
    (@name, @imgPath)
";

            using var command = new MySqlCommand(sqlQuery, connection);

            command.Parameters.AddWithValue("@name", tagModel.Name);
            command.Parameters.AddWithValue("@imgPath", tagModel.ImgPath);

            command.ExecuteNonQuery();
        }

        public void RemoveTagModel(string tagName)
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"
DELETE FROM tags WHERE name = @tagName
";

            using var command = new MySqlCommand(sqlQuery, connection);

            command.Parameters.AddWithValue("@tagName", tagName);

            command.ExecuteNonQuery();
        }
    }
}
