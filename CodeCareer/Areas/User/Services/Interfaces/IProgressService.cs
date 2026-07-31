using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface IProgressService
    {
        void MarkNoteRead(string userEmail, int topicId, int noteId);
        void MarkTaskSolved(string userEmail, int taskId);
        bool IsNoteRead(string userEmail, int noteId);
        bool IsTaskSolved(string userEmail, int taskId);
        List<UserTopicProgressModel> GetTopicProgress(string userEmail);
        List<UserTaskProgressModel> GetTaskProgress(string userEmail);
        int CountSolvedTasks(string userEmail);
        int CountReadNotes(string userEmail);
        double GetTopicCompletionPercent(string userEmail, int topicId, int notesCount, int tasksCount);
    }
}
