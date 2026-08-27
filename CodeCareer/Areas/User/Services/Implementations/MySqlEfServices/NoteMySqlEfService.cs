using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class NoteMySqlEfService : INoteService
{
    private readonly ApplicationDbContext _db;

    public NoteMySqlEfService(ApplicationDbContext db) => _db = db;

    public List<NoteModel> GetByTopicId(int topicId) =>
        _db.Notes.AsNoTracking().Where(n => n.TopicId == topicId).OrderBy(n => n.SortOrder).ToList();

    public NoteModel? GetById(int id) =>
        _db.Notes.Include(n => n.Topic).ThenInclude(t => t!.Section)
            .AsNoTracking().FirstOrDefault(n => n.Id == id);

    public NoteModel? GetFirstByTopicId(int topicId) =>
        _db.Notes.AsNoTracking().Where(n => n.TopicId == topicId).OrderBy(n => n.SortOrder).FirstOrDefault();

    public void Add(NoteModel note)
    {
        note.UpdatedAt = DateTime.UtcNow;
        _db.Notes.Add(note);
        _db.SaveChanges();
    }

    public void Update(NoteModel note)
    {
        note.UpdatedAt = DateTime.UtcNow;
        _db.Notes.Update(note);
        _db.SaveChanges();
    }

    public void Remove(int id)
    {
        var entity = _db.Notes.Find(id);
        if (entity == null) return;
        _db.Notes.Remove(entity);
        _db.SaveChanges();
    }

    public List<NoteModel> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length > 200)
        {
            return new List<NoteModel>();
        }

        var q = query.Trim().ToLowerInvariant();
        return _db.Notes.Include(n => n.Topic).AsNoTracking()
            .Where(n => n.Title.ToLower().Contains(q) || n.BodyMarkdown.ToLower().Contains(q))
            .OrderBy(n => n.Title).Take(50).ToList();
    }
}
