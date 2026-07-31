using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ITaskService
    {
        List<TaskModel> GetTaskModels();
        List<TaskModel> GetByTopicId(int topicId);
        TaskModel? GetByName(string name);
        TaskModel? GetById(int id);
        void AddTaskModel(TaskModel task);
        void RemoveTaskModel(int id);
        List<TaskModel> Search(string query);
        bool CheckSampleOutput(int taskId, int sampleIndex, string actualOutput);
    }
}
