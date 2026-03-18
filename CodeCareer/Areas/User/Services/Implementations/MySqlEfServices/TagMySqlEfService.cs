using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class TagMySqlEfService : ITagService
    {
        public List<TagModel> GetTagModels()
        {
            using var dbContext = new ApplicationDbContext();

            return dbContext.Tags.ToList();
        }

        public void AddTagModel(TagModel tag)
        {
            using var dbContext = new ApplicationDbContext();
            dbContext.Tags.Add(tag);
            dbContext.SaveChanges();
        }

        public void RemoveTagModel(string tagName)
        {
            using var dbContext = new ApplicationDbContext();
            var dbTag = dbContext.Tags.FirstOrDefault(t => t.Name == tagName);
            dbContext.Tags.Remove(dbTag);
            dbContext.SaveChanges();
        }
    }
}
