using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces;

public interface ISubmissionService
{
    SubmissionModel Save(SubmissionModel submission);
    List<SubmissionModel> GetByUserId(int userId, int take = 50);
    List<SubmissionModel> GetByUserAndTask(int userId, int taskId);
    SubmissionModel? GetById(int id);
}
