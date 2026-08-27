using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class CommentMySqlEfService : ICommentService
{
    private readonly ApplicationDbContext _db;

    public CommentMySqlEfService(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<CommentModel> GetByPublicationId(int publicationId)
    {
        return _db.Comments.AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.PublicationId == publicationId)
            .OrderBy(c => c.CreatedDate)
            .ToList();
    }

    public CommentModel? GetById(int id) =>
        _db.Comments.AsNoTracking().Include(c => c.User).FirstOrDefault(c => c.Id == id);

    public void Add(CommentModel comment)
    {
        comment.CreatedDate = DateTime.UtcNow;
        _db.Comments.Add(comment);
        _db.SaveChanges();
    }

    public bool Delete(int commentId, int userId)
    {
        var comment = _db.Comments.FirstOrDefault(c => c.Id == commentId && c.UserId == userId);
        if (comment == null)
        {
            return false;
        }

        _db.Comments.Remove(comment);
        _db.SaveChanges();
        return true;
    }

    public bool DeleteAsAdmin(int commentId)
    {
        var comment = _db.Comments.Find(commentId);
        if (comment == null)
        {
            return false;
        }

        _db.Comments.Remove(comment);
        _db.SaveChanges();
        return true;
    }
}
