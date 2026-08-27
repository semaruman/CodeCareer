using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class PublicationMySqlEfService : IPublicationService
{
    private readonly ApplicationDbContext _db;

    public PublicationMySqlEfService(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<PublicationModel> GetPublicationModels()
    {
        var list = _db.Publications.AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.PublicationTags).ThenInclude(pt => pt.Tag)
            .OrderByDescending(p => p.CreatedDate)
            .ToList();

        foreach (var publication in list)
        {
            HydrateTags(publication);
        }

        return list;
    }

    public PublicationModel? GetById(int id)
    {
        var publication = _db.Publications.AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.PublicationTags).ThenInclude(pt => pt.Tag)
            .FirstOrDefault(p => p.Id == id);

        return publication == null ? null : HydrateTags(publication);
    }

    public void AddPublicationModel(PublicationModel publication)
    {
        publication.CreatedDate = DateTime.UtcNow;
        _db.Publications.Add(publication);
        _db.SaveChanges();
        SyncTags(publication);
    }

    public void UpdatePublicationModel(PublicationModel publication)
    {
        var existing = _db.Publications
            .Include(p => p.PublicationTags)
            .FirstOrDefault(p => p.Id == publication.Id);

        if (existing == null)
        {
            return;
        }

        existing.Content = publication.Content;
        existing.PublicationTags.Clear();
        foreach (var tag in publication.Tags)
        {
            var dbTag = _db.Tags.FirstOrDefault(t => t.Name == tag.Name);
            if (dbTag != null)
            {
                existing.PublicationTags.Add(new PublicationTagModel { PublicationId = existing.Id, TagId = dbTag.Id });
            }
        }

        _db.SaveChanges();
    }

    public void RemovePublicationModel(int id)
    {
        var publication = _db.Publications.Find(id);
        if (publication == null)
        {
            return;
        }

        _db.Publications.Remove(publication);
        _db.SaveChanges();
    }

    public bool IsOwner(int publicationId, int userId) =>
        _db.Publications.AsNoTracking().Any(p => p.Id == publicationId && p.UserId == userId);

    private void SyncTags(PublicationModel publication)
    {
        var existing = _db.Publications
            .Include(p => p.PublicationTags)
            .First(p => p.Id == publication.Id);

        existing.PublicationTags.Clear();
        foreach (var tag in publication.Tags)
        {
            var dbTag = _db.Tags.FirstOrDefault(t => t.Name == tag.Name);
            if (dbTag != null)
            {
                existing.PublicationTags.Add(new PublicationTagModel { PublicationId = existing.Id, TagId = dbTag.Id });
            }
        }

        _db.SaveChanges();
    }

    private static PublicationModel HydrateTags(PublicationModel publication)
    {
        publication.Tags = publication.PublicationTags.Select(pt => pt.Tag).ToHashSet();
        return publication;
    }
}
