using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class NoteMySqlEfService : INoteService
    {
        public List<NoteModel> GetByTopicId(int topicId)
        {
            using var db = new ApplicationDbContext();
            return db.Notes.AsNoTracking()
                .Where(n => n.TopicId == topicId)
                .OrderBy(n => n.SortOrder)
                .ToList();
        }

        public NoteModel? GetById(int id)
        {
            using var db = new ApplicationDbContext();
            return db.Notes.Include(n => n.Topic).ThenInclude(t => t!.Section)
                .AsNoTracking().FirstOrDefault(n => n.Id == id);
        }

        public NoteModel? GetFirstByTopicId(int topicId)
        {
            using var db = new ApplicationDbContext();
            return db.Notes.AsNoTracking()
                .Where(n => n.TopicId == topicId)
                .OrderBy(n => n.SortOrder)
                .FirstOrDefault();
        }

        public void Add(NoteModel note)
        {
            using var db = new ApplicationDbContext();
            note.UpdatedAt = DateTime.UtcNow;
            db.Notes.Add(note);
            db.SaveChanges();
        }

        public void Update(NoteModel note)
        {
            using var db = new ApplicationDbContext();
            note.UpdatedAt = DateTime.UtcNow;
            db.Notes.Update(note);
            db.SaveChanges();
        }

        public void Remove(int id)
        {
            using var db = new ApplicationDbContext();
            var entity = db.Notes.Find(id);
            if (entity == null) return;
            db.Notes.Remove(entity);
            db.SaveChanges();
        }

        public List<NoteModel> Search(string query)
        {
            using var db = new ApplicationDbContext();
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<NoteModel>();
            }
            var q = query.Trim().ToLowerInvariant();
            return db.Notes.Include(n => n.Topic).AsNoTracking()
                .Where(n => n.Title.ToLower().Contains(q) || n.BodyMarkdown.ToLower().Contains(q))
                .OrderBy(n => n.Title)
                .Take(50)
                .ToList();
        }
    }
}
