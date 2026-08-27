using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces;

public interface ICommentService
{
    List<CommentModel> GetByPublicationId(int publicationId);
    CommentModel? GetById(int id);
    void Add(CommentModel comment);
    bool Delete(int commentId, int userId);
    bool DeleteAsAdmin(int commentId);
}
