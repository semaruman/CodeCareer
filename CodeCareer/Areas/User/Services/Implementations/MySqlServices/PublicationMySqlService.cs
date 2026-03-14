using System.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlServices
{
    public class PublicationMySqlService : IPublicationService
    {
        private IUserService _userService { get; set; }
        private ITagService _tagService { get; set; }

        public PublicationMySqlService(IUserService userService, ITagService tagService)
        {
            _userService = userService;
            _tagService = tagService;
        }

        public List<PublicationModel> GetPublicationModels()
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"
SELECT id, created_date, user_id, content, tag_names
FROM publications
";

            using var command = new MySqlCommand(sqlQuery, connection);

            using var reader = command.ExecuteReader();

            List<PublicationModel> publicationsList = new List<PublicationModel>();
            while (reader.Read())
            {
                HashSet<TagModel> tags = new HashSet<TagModel>();
                string tagNames = reader.GetString("tag_names");
                if (!string.IsNullOrEmpty(tagNames))
                {
                    tags = _tagService.GetTagModels().Where(t => tagNames.Contains(t.Name)).ToHashSet();

                }

                var publication = new PublicationModel()
                {
                    //Id = reader.IsDBNull("id") ? 0: reader.GetInt32("id"),
                    CreatedDate = reader.GetDateTime("created_date"),
                    User = _userService.GetUserById(reader.GetInt32("user_id")),
                    Content = reader.GetString("content"),
                    Tags = tags
                };
                publicationsList.Add(publication);
            }

            return publicationsList;
        }

        public void AddPublicationModel(PublicationModel publication)
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            DateTime createdDateP = publication.CreatedDate;
            int userIdP = publication.User.Id;
            string contentP = publication.Content;
            string tagNamesP = (publication.Tags == null?"" : string.Join("; ", publication.Tags));

            string sqlQuery = @"
INSERT INTO publications (created_date, user_id, content, tag_names) VALUES
    (@createdDateP, @userIdP, @contentP, @tagNamesP)
";

            using var command = new MySqlCommand(sqlQuery, connection);

            command.Parameters.AddWithValue("@createdDateP", createdDateP);
            command.Parameters.AddWithValue("@userIdP", userIdP);
            command.Parameters.AddWithValue("@contentP", contentP);
            command.Parameters.AddWithValue("@tagNamesP", tagNamesP);

            command.ExecuteNonQuery();
        }

        public void RemovePublicationModel(int id)
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"DELETE FROM publications WHERE id = @id";

            using var command = new MySqlCommand(sqlQuery, connection);

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
}
