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

        public void RemoveTaskModel(int id)
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

            tasks = tasks.Where(t => !(t.Id == id)).ToList();
            string jsonWrite = JsonSerializer.Serialize(tasks);
            File.WriteAllText(_filepath, jsonWrite);
        }

        public List<TaskModel> GetByTopicId(int topicId) =>
            GetTaskModels().Where(t => t.TopicId == topicId).ToList();

        public TaskModel? GetByName(string name) =>
            GetTaskModels().FirstOrDefault(t => t.Name == name);

        public TaskModel? GetById(int id) =>
            GetTaskModels().FirstOrDefault(t => t.Id == id);

        public List<TaskModel> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<TaskModel>();
            var q = query.Trim().ToLowerInvariant();
            return GetTaskModels()
                .Where(t => (t.Name ?? "").ToLower().Contains(q) || (t.Content ?? "").ToLower().Contains(q) || (t.Type ?? "").ToLower().Contains(q))
                .Take(50)
                .ToList();
        }

        public bool CheckSampleOutput(int taskId, int sampleIndex, string actualOutput)
        {
            var task = GetById(taskId);
            if (task == null || sampleIndex < 0 || sampleIndex >= task.OutputStrings.Count) return false;
            var expected = (task.OutputStrings[sampleIndex] ?? "").Trim().Replace("\r\n", "\n");
            var actual = (actualOutput ?? "").Trim().Replace("\r\n", "\n");
            return string.Equals(expected, actual, StringComparison.Ordinal);
        }
    }
}
