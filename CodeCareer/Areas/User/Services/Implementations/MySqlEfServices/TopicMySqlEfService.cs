using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class TopicMySqlEfService : ITopicService
    {
        public List<TopicModel> GetAll(bool onlyPublished = true)
        {
            using var db = new ApplicationDbContext();
            var q = db.Topics.Include(t => t.Section).AsNoTracking().AsQueryable();
            if (onlyPublished) q = q.Where(t => t.IsPublished);
            return q.OrderBy(t => t.Section!.SortOrder).ThenBy(t => t.SortOrder).ToList();
        }

        public TopicModel? GetById(int id)
        {
            using var db = new ApplicationDbContext();
            return db.Topics.Include(t => t.Section).Include(t => t.Notes).AsNoTracking()
                .FirstOrDefault(t => t.Id == id);
        }

        public TopicModel? GetBySlug(string slug)
        {
            using var db = new ApplicationDbContext();
            return db.Topics.Include(t => t.Section).Include(t => t.Notes).AsNoTracking()
                .FirstOrDefault(t => t.Slug == slug);
        }

        public void Add(TopicModel topic)
        {
            using var db = new ApplicationDbContext();
            db.Topics.Add(topic);
            db.SaveChanges();
        }

        public void Update(TopicModel topic)
        {
            using var db = new ApplicationDbContext();
            db.Topics.Update(topic);
            db.SaveChanges();
        }

        public void Remove(int id)
        {
            using var db = new ApplicationDbContext();
            var entity = db.Topics.Find(id);
            if (entity == null) return;
            db.Topics.Remove(entity);
            db.SaveChanges();
        }
    }
}
