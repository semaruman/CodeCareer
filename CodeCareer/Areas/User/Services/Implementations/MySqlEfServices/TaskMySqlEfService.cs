using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class TaskMySqlEfService : ITaskService
    {
        public List<TaskModel> GetTaskModels()
        {
            using var dbContext = new ApplicationDbContext();
            List<TaskModel> res = new List<TaskModel>();
            foreach (var task in dbContext.Tasks.AsNoTracking())
            {
                task.InputStrings = task.AllInputStrings.Split("; ").ToList();
                task.OutputStrings = task.AllOutputStrings.Split("; ").ToList();
                res.Add(task);
            }
            return res;
        }

        public void AddTaskModel(TaskModel task)
        {
            using var dbContext = new ApplicationDbContext();
            task.AllInputStrings = string.Join("; ", task.InputStrings);
            task.AllOutputStrings = string.Join("; ", task.OutputStrings);
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
