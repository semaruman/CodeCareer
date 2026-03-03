using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ITaskService
    {
        public List<TaskModel> GetTaskModels();
        public void AddTaskModel(TaskModel tag);
        public void RemoveTaskModel(string taskName);
    }
}
