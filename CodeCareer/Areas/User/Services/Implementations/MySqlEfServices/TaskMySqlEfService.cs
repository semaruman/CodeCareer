using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class TaskMySqlEfService : ITaskService
    {
        private readonly ApplicationDbContext _db;

        public TaskMySqlEfService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<TaskModel> GetTaskModels()
        {
            var dbContext = _db;
            List<TaskModel> res = new List<TaskModel>();
            foreach (var task in dbContext.Tasks.AsNoTracking())
            {
                HydrateSamples(task);
                res.Add(task);
            }
            return res;
        }

        public List<TaskModel> GetByTopicId(int topicId)
        {
            var dbContext = _db;
            var list = dbContext.Tasks.AsNoTracking()
                .Where(t => t.TopicId == topicId)
                .ToList();
            foreach (var task in list) HydrateSamples(task);
            return list;
        }

        public TaskModel? GetByName(string name)
        {
            var dbContext = _db;
            var task = dbContext.Tasks.AsNoTracking().FirstOrDefault(t => t.Name == name);
            if (task != null) HydrateSamples(task);
            return task;
        }

        public TaskModel? GetById(int id)
        {
            var dbContext = _db;
            var task = dbContext.Tasks.AsNoTracking().FirstOrDefault(t => t.Id == id);
            if (task != null) HydrateSamples(task);
            return task;
        }

        public void AddTaskModel(TaskModel task)
        {
            var dbContext = _db;
            task.AllInputStrings = string.Join("; ", task.InputStrings);
            task.AllOutputStrings = string.Join("; ", task.OutputStrings);
            if (task.TopicId.HasValue)
            {
                var topic = dbContext.Topics.AsNoTracking().FirstOrDefault(t => t.Id == task.TopicId.Value);
                if (topic != null)
                {
                    task.Type = topic.Title;
                }
            }
            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();
        }

        public void RemoveTaskModel(int id)
        {
            var dbContext = _db;
            var task = dbContext.Tasks.Find(id);
            if (task == null) return;
            dbContext.Tasks.Remove(task);
            dbContext.SaveChanges();
        }

        public List<TaskModel> Search(string query)
        {
            var dbContext = _db;
            if (string.IsNullOrWhiteSpace(query)) return new List<TaskModel>();
            var q = query.Trim().ToLowerInvariant();
            var list = dbContext.Tasks.AsNoTracking()
                .Where(t => t.Name.ToLower().Contains(q) || t.Content.ToLower().Contains(q) || t.Type.ToLower().Contains(q))
                .Take(50)
                .ToList();
            foreach (var task in list) HydrateSamples(task);
            return list;
        }

        public bool CheckSampleOutput(int taskId, int sampleIndex, string actualOutput)
        {
            var task = GetById(taskId);
            if (task == null || sampleIndex < 0 || sampleIndex >= task.OutputStrings.Count)
            {
                return false;
            }
            var expected = (task.OutputStrings[sampleIndex] ?? string.Empty).Trim().Replace("\r\n", "\n");
            var actual = (actualOutput ?? string.Empty).Trim().Replace("\r\n", "\n");
            return string.Equals(expected, actual, StringComparison.Ordinal);
        }

        private static void HydrateSamples(TaskModel task)
        {
            task.InputStrings = string.IsNullOrEmpty(task.AllInputStrings)
                ? new List<string> { "", "", "" }
                : task.AllInputStrings.Split("; ").ToList();
            task.OutputStrings = string.IsNullOrEmpty(task.AllOutputStrings)
                ? new List<string> { "", "", "" }
                : task.AllOutputStrings.Split("; ").ToList();
        }
    }
}
