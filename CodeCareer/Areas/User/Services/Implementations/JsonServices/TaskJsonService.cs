using System.Text.Json;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;

namespace CodeCareer.Areas.User.Services.Implementations.JsonServices
{
    public class TaskJsonService : ITaskService
    {
        private readonly string _filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "User", "Data", "JsonFiles", "task_db.json");

        public List<TaskModel> GetTaskModels()
        {
            List<TaskModel> tasks = new List<TaskModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    tasks = JsonSerializer.Deserialize<List<TaskModel>>(json) ?? new List<TaskModel>();
                }
            }

            return tasks;
        }

        public void AddTaskModel(TaskModel task)
        {
            List<TaskModel> tasks = new List<TaskModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    tasks = JsonSerializer.Deserialize<List<TaskModel>>(json) ?? new List<TaskModel>();
                }
            }

            tasks.Add(task);
            string jsonWrite = JsonSerializer.Serialize(tasks);
            File.WriteAllText(_filepath, jsonWrite);
        }

        public void RemoveTaskModel(string taskName)
        {
            List<TaskModel> tasks = new List<TaskModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    tasks = JsonSerializer.Deserialize<List<TaskModel>>(json) ?? new List<TaskModel>();
                }
            }

            tasks = tasks.Where(t => !(t.Name == taskName)).ToList();
            string jsonWrite = JsonSerializer.Serialize(tasks);
            File.WriteAllText(_filepath, jsonWrite);
        }
    }
}
