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
        var tagNames = publication.Tags.Select(t => t.Name).ToList();
        publication.Tags.Clear();
        publication.User = null!;
        publication.PublicationTags = new List<PublicationTagModel>();

        _db.Publications.Add(publication);
        _db.SaveChanges();

        SyncTags(publication.Id, tagNames);
    }

    public void UpdatePublicationModel(PublicationModel publication)
    {
        var existing = _db.Publications.FirstOrDefault(p => p.Id == publication.Id);
        if (existing == null)
        {
            return;
        }

        existing.Content = publication.Content;
        _db.SaveChanges();
        SyncTags(publication.Id, publication.Tags.Select(t => t.Name));
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

    private void SyncTags(int publicationId, IEnumerable<string> tagNames)
    {
        var names = tagNames.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToList();
        var existing = _db.Publications
            .Include(p => p.PublicationTags)
            .First(p => p.Id == publicationId);

        existing.PublicationTags.Clear();

        if (names.Count > 0)
        {
            var tagIds = _db.Tags
                .AsNoTracking()
                .Where(t => names.Contains(t.Name))
                .Select(t => t.Id)
                .ToList();

            foreach (var tagId in tagIds)
            {
                existing.PublicationTags.Add(new PublicationTagModel
                {
                    PublicationId = existing.Id,
                    TagId = tagId,
                });
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
