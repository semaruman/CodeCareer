using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class TaskMySqlEfService : ITaskService
    {
        public List<TaskModel> GetTaskModels()
        {
            using var dbContext = new ApplicationDbContext();
            return dbContext.Tasks.ToList();
        }

        public void AddTaskModel(TaskModel task)
        {
            using var dbContext = new ApplicationDbContext();
            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();
        }

        public void RemoveTaskModel(int id)
        {
            using var dbContext = new ApplicationDbContext();
            var task = dbContext.Tasks.Find(id);
            dbContext.Tasks.Remove(task);
            dbContext.SaveChanges();
        }
    }
}
