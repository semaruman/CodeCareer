using System.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlAdoNetServices
{
    public class TaskMySqlAdoNetService : ITaskService
    {
        public List<TaskModel> GetTaskModels()
        {
            using var connection = new MySqlConnection(Constants.CONNECTION_STRING);
            connection.Open();

            string sqlQuery = @"
SELECT *
FROM tasks
";

            using var command = new MySqlCommand(sqlQuery, connection);
            using var reader = command.ExecuteReader();

            List<TaskModel> res = new List<TaskModel>();

            while (reader.Read())
            {
                var model = new TaskModel
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.IsDBNull("name") ? null : reader.GetString("name"),
                    Type = reader.IsDBNull("type") ? null : reader.GetString("type"),
                    Content = reader.IsDBNull("content") ? null : reader.GetString("content"),
                    InputContent = reader.IsDBNull("input_content") ? null : reader.GetString("input_content"),
                    OutputContent = reader.IsDBNull("output_content") ? null : reader.GetString("output_content")
                };

                res.Add(model);
            }

            return res;
        }
    }
}
