using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class TopicMySqlEfService : ITopicService
{
    private readonly ApplicationDbContext _db;

    public TopicMySqlEfService(ApplicationDbContext db) => _db = db;

    public List<TopicModel> GetAll(bool onlyPublished = true)
    {
        var q = _db.Topics.Include(t => t.Section).AsNoTracking().AsQueryable();
        if (onlyPublished) q = q.Where(t => t.IsPublished);
        return q.OrderBy(t => t.Section!.SortOrder).ThenBy(t => t.SortOrder).ToList();
    }

    public TopicModel? GetById(int id) =>
        _db.Topics.Include(t => t.Section).Include(t => t.Notes).AsNoTracking()
            .FirstOrDefault(t => t.Id == id);

    public TopicModel? GetBySlug(string slug) =>
        _db.Topics.Include(t => t.Section).Include(t => t.Notes).AsNoTracking()
            .FirstOrDefault(t => t.Slug == slug);

    public void Add(TopicModel topic)
    {
        _db.Topics.Add(topic);
        _db.SaveChanges();
    }

    public void Update(TopicModel topic)
    {
        _db.Topics.Update(topic);
        _db.SaveChanges();
    }

    public void Remove(int id)
    {
        var entity = _db.Topics.Find(id);
        if (entity == null) return;
        _db.Topics.Remove(entity);
        _db.SaveChanges();
    }
}
