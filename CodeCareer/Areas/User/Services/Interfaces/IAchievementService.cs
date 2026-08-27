namespace CodeCareer.Areas.User.Services.Interfaces;

public interface IAchievementService
{
    void TryGrant(int userId, string achievementKey);
    IReadOnlyList<string> GetUserAchievements(int userId);
}
