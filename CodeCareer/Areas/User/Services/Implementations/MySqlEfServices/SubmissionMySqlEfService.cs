using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class SubmissionMySqlEfService : ISubmissionService
{
    private readonly ApplicationDbContext _db;

    public SubmissionMySqlEfService(ApplicationDbContext db)
    {
        _db = db;
    }

    public SubmissionModel Save(SubmissionModel submission)
    {
        submission.CreatedAt = DateTime.UtcNow;
        _db.Submissions.Add(submission);
        _db.SaveChanges();
        return submission;
    }

    public List<SubmissionModel> GetByUserId(int userId, int take = 50) =>
        _db.Submissions.AsNoTracking()
            .Include(s => s.Task)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(take)
            .ToList();

    public List<SubmissionModel> GetByUserAndTask(int userId, int taskId) =>
        _db.Submissions.AsNoTracking()
            .Where(s => s.UserId == userId && s.TaskId == taskId)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();

    public SubmissionModel? GetById(int id) =>
        _db.Submissions.AsNoTracking().Include(s => s.Task).FirstOrDefault(s => s.Id == id);
}
