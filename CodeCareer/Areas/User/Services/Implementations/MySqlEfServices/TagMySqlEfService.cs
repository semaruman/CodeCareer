using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class TagMySqlEfService : ITagService
{
    private readonly ApplicationDbContext _db;

    public TagMySqlEfService(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<TagModel> GetTagModels() =>
        _db.Tags.AsNoTracking().Select(t => new TagModel
        {
            Id = t.Id,
            Name = t.Name,
            ImgPath = t.ImgPath ?? string.Empty,
        }).ToList();

    public void AddTagModel(TagModel tag)
    {
        _db.Tags.Add(tag);
        _db.SaveChanges();
    }

    public void RemoveTagModel(string tagName)
    {
        var dbTag = _db.Tags.FirstOrDefault(t => t.Name == tagName);
        if (dbTag == null)
        {
            return;
        }

        _db.Tags.Remove(dbTag);
        _db.SaveChanges();
    }
}
