using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class AchievementMySqlEfService : IAchievementService
{
    private readonly ApplicationDbContext _db;

    public AchievementMySqlEfService(ApplicationDbContext db)
    {
        _db = db;
    }

    public void TryGrant(int userId, string achievementKey)
    {
        if (_db.UserAchievements.Any(a => a.UserId == userId && a.AchievementKey == achievementKey))
        {
            return;
        }

        _db.UserAchievements.Add(new Areas.User.Models.UserAchievementModel
        {
            UserId = userId,
            AchievementKey = achievementKey,
            EarnedAt = DateTime.UtcNow,
        });
        _db.SaveChanges();
    }

    public IReadOnlyList<string> GetUserAchievements(int userId) =>
        _db.UserAchievements.AsNoTracking()
            .Where(a => a.UserId == userId)
            .Select(a => a.AchievementKey)
            .ToList();
}
