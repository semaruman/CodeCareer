using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class SectionMySqlEfService : ISectionService
{
    private readonly ApplicationDbContext _db;

    public SectionMySqlEfService(ApplicationDbContext db) => _db = db;

    public List<SectionModel> GetSectionsWithTopics(bool onlyPublishedTopics = true)
    {
        var sections = _db.Sections.Include(s => s.Topics).AsNoTracking()
            .OrderBy(s => s.SortOrder).ToList();

        foreach (var section in sections)
        {
            section.Topics = section.Topics
                .Where(t => !onlyPublishedTopics || t.IsPublished)
                .OrderBy(t => t.SortOrder).ToList();
        }

        return sections;
    }

    public SectionModel? GetById(int id) =>
        _db.Sections.AsNoTracking().FirstOrDefault(s => s.Id == id);

    public void Add(SectionModel section)
    {
        _db.Sections.Add(section);
        _db.SaveChanges();
    }

    public void Update(SectionModel section)
    {
        _db.Sections.Update(section);
        _db.SaveChanges();
    }

    public void Remove(int id)
    {
        var entity = _db.Sections.Find(id);
        if (entity == null) return;
        _db.Sections.Remove(entity);
        _db.SaveChanges();
    }
}
