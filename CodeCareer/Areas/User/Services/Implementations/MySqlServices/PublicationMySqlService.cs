using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlServices
{
    public class PublicationMySqlService : IPublicationService
    {
        private IUserService _userService { get; set; }
        private ITagService _tagService { get; set; }

        public PublicationMySqlService(IUserService userService)
        {
            _userService = userService;
        }

        public List<PublicationModel> GetPublicationModels()
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"
SELECT id, created_date, user_id, content, tagnames
FROM publications
";

            using var command = new MySqlCommand(sqlQuery, connection);

            using var reader = command.ExecuteReader();

            List<PublicationModel> publicationsList = new List<PublicationModel>();
            while (reader.Read())
            {
                var publication = new PublicationModel()
                {
                    Id = reader.GetInt32("id"),
                    CreatedDate = reader.GetDateTime("created_date"),
                    User = _userService.GetUserById(reader.GetInt32("user_id")),
                    Content = reader.GetString("content"),
                    Tags = _tagService.GetTagModels().Where(t => reader.GetString("tag_names").Contains(t.Name)).ToHashSet()
                };
                publicationsList.Add(publication);
            }

            return publicationsList;
        }

        public void AddPublicationModel(PublicationModel publication)
        {

        }

        public void RemovePublicationModel(int id)
        {

        }
    }
}
