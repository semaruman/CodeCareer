using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ICommentService
    {
        List<CommentModel> GetByPublicationId(int publicationId);
        void Add(CommentModel comment);
    }
}
