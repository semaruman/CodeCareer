using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class SectionMySqlEfService : ISectionService
    {
        public List<SectionModel> GetSectionsWithTopics(bool onlyPublishedTopics = true)
        {
            using var db = new ApplicationDbContext();
            var query = db.Sections
                .Include(s => s.Topics)
                .AsNoTracking()
                .OrderBy(s => s.SortOrder)
                .AsQueryable();

            var sections = query.ToList();
            foreach (var section in sections)
            {
                section.Topics = section.Topics
                    .Where(t => !onlyPublishedTopics || t.IsPublished)
                    .OrderBy(t => t.SortOrder)
                    .ToList();
            }
            return sections;
        }

        public SectionModel? GetById(int id)
        {
            using var db = new ApplicationDbContext();
            return db.Sections.AsNoTracking().FirstOrDefault(s => s.Id == id);
        }

        public void Add(SectionModel section)
        {
            using var db = new ApplicationDbContext();
            db.Sections.Add(section);
            db.SaveChanges();
        }

        public void Update(SectionModel section)
        {
            using var db = new ApplicationDbContext();
            db.Sections.Update(section);
            db.SaveChanges();
        }

        public void Remove(int id)
        {
            using var db = new ApplicationDbContext();
            var entity = db.Sections.Find(id);
            if (entity == null) return;
            db.Sections.Remove(entity);
            db.SaveChanges();
        }
    }
}
