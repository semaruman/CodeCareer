using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class ProgressMySqlEfService : IProgressService
    {
        public void MarkNoteRead(string userEmail, int topicId, int noteId)
        {
            using var db = new ApplicationDbContext();
            var existing = db.UserTopicProgress
                .FirstOrDefault(p => p.UserEmail == userEmail && p.TopicId == topicId && p.NoteId == noteId);
            if (existing == null)
            {
                db.UserTopicProgress.Add(new UserTopicProgressModel
                {
                    UserEmail = userEmail,
                    TopicId = topicId,
                    NoteId = noteId,
                    NoteReadAt = DateTime.UtcNow,
                });
            }
            else
            {
                existing.NoteReadAt = DateTime.UtcNow;
            }
            db.SaveChanges();
        }

        public void MarkTaskSolved(string userEmail, int taskId)
        {
            using var db = new ApplicationDbContext();
            var existing = db.UserTaskProgress
                .FirstOrDefault(p => p.UserEmail == userEmail && p.TaskId == taskId);
            if (existing == null)
            {
                db.UserTaskProgress.Add(new UserTaskProgressModel
                {
                    UserEmail = userEmail,
                    TaskId = taskId,
                    Status = "solved",
                    SolvedAt = DateTime.UtcNow,
                });
            }
            else
            {
                existing.Status = "solved";
                existing.SolvedAt = DateTime.UtcNow;
            }
            db.SaveChanges();
        }

        public bool IsNoteRead(string userEmail, int noteId)
        {
            using var db = new ApplicationDbContext();
            return db.UserTopicProgress.AsNoTracking()
                .Any(p => p.UserEmail == userEmail && p.NoteId == noteId && p.NoteReadAt != null);
        }

        public bool IsTaskSolved(string userEmail, int taskId)
        {
            using var db = new ApplicationDbContext();
            return db.UserTaskProgress.AsNoTracking()
                .Any(p => p.UserEmail == userEmail && p.TaskId == taskId && p.Status == "solved");
        }

        public List<UserTopicProgressModel> GetTopicProgress(string userEmail)
        {
            using var db = new ApplicationDbContext();
            return db.UserTopicProgress.AsNoTracking()
                .Where(p => p.UserEmail == userEmail)
                .ToList();
        }

        public List<UserTaskProgressModel> GetTaskProgress(string userEmail)
        {
            using var db = new ApplicationDbContext();
            return db.UserTaskProgress.AsNoTracking()
                .Where(p => p.UserEmail == userEmail)
                .ToList();
        }

        public int CountSolvedTasks(string userEmail) =>
            GetTaskProgress(userEmail).Count(p => p.Status == "solved");

        public int CountReadNotes(string userEmail) =>
            GetTopicProgress(userEmail).Count(p => p.NoteReadAt != null);

        public double GetTopicCompletionPercent(string userEmail, int topicId, int notesCount, int tasksCount)
        {
            var total = notesCount + tasksCount;
            if (total == 0) return 0;
            using var db = new ApplicationDbContext();
            var readNotes = db.UserTopicProgress.AsNoTracking()
                .Count(p => p.UserEmail == userEmail && p.TopicId == topicId && p.NoteReadAt != null);
            var taskIds = db.Tasks.AsNoTracking().Where(t => t.TopicId == topicId).Select(t => t.Id).ToList();
            var solved = db.UserTaskProgress.AsNoTracking()
                .Count(p => p.UserEmail == userEmail && taskIds.Contains(p.TaskId) && p.Status == "solved");
            return Math.Round(100.0 * (readNotes + solved) / total, 1);
        }
    }
}
