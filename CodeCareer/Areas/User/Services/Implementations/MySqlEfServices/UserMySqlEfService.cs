using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class UserMySqlEfService : IUserService
{
    private readonly ApplicationDbContext _db;
    private readonly ITagService _tagService;

    public UserMySqlEfService(ApplicationDbContext db, ITagService tagService)
    {
        _db = db;
        _tagService = tagService;
    }

    public List<UserModel> GetUserModels()
    {
        var users = _db.Users.AsNoTracking()
            .Include(u => u.UserSkillTags).ThenInclude(ust => ust.Tag)
            .Include(u => u.Following).ThenInclude(s => s.Following)
            .Include(u => u.Followers).ThenInclude(s => s.Follower)
            .ToList();

        foreach (var user in users)
        {
            HydrateUser(user);
        }

        return users;
    }

    public UserModel? GetUserByEmail(string email)
    {
        var user = _db.Users.AsNoTracking()
            .Include(u => u.UserSkillTags).ThenInclude(ust => ust.Tag)
            .Include(u => u.Following).ThenInclude(s => s.Following)
            .Include(u => u.Followers).ThenInclude(s => s.Follower)
            .FirstOrDefault(u => u.Email == email);

        if (user != null)
        {
            HydrateUser(user);
        }

        return user;
    }

    public UserModel? GetUserById(int id)
    {
        var user = _db.Users.AsNoTracking()
            .Include(u => u.UserSkillTags).ThenInclude(ust => ust.Tag)
            .Include(u => u.Following).ThenInclude(s => s.Following)
            .Include(u => u.Followers).ThenInclude(s => s.Follower)
            .FirstOrDefault(u => u.Id == id);

        if (user != null)
        {
            HydrateUser(user);
        }

        return user;
    }

    public void AddUserModel(UserModel user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
        SyncSkillTags(user);
    }

    public void UpdateUserModel(UserModel user)
    {
        var existing = _db.Users
            .Include(u => u.UserSkillTags)
            .FirstOrDefault(u => u.Id == user.Id);

        if (existing == null)
        {
            return;
        }

        existing.FullName = user.FullName;
        existing.Email = user.Email;
        existing.PasswordHash = user.PasswordHash;
        existing.BirthDate = user.BirthDate;
        existing.Info = user.Info;
        existing.Rating = user.Rating;
        existing.Status = user.Status;
        existing.Role = user.Role;
        existing.AvatarPath = user.AvatarPath;
        existing.ShowSubscriptions = user.ShowSubscriptions;
        existing.MustChangePassword = user.MustChangePassword;
        existing.FailedLoginAttempts = user.FailedLoginAttempts;
        existing.LockoutEndUtc = user.LockoutEndUtc;

        SyncSkillTags(user, existing);
        _db.SaveChanges();
    }

    public void RemoveUserModel(int userId)
    {
        var user = _db.Users.Find(userId);
        if (user == null)
        {
            return;
        }

        _db.Users.Remove(user);
        _db.SaveChanges();
    }

    public bool Subscribe(int followerId, int followingId)
    {
        if (followerId == followingId)
        {
            return false;
        }

        if (_db.UserSubscriptions.Any(s => s.FollowerId == followerId && s.FollowingId == followingId))
        {
            return false;
        }

        _db.UserSubscriptions.Add(new UserSubscriptionModel
        {
            FollowerId = followerId,
            FollowingId = followingId,
            CreatedAt = DateTime.UtcNow,
        });

        var follower = _db.Users.Find(followerId);
        var following = _db.Users.Find(followingId);
        if (follower != null && following != null)
        {
            follower.Rating += Constants.PlUS_RATING_FOR_SUBSCRIPTION;
            following.Rating += Constants.PlUS_RATING_FOR_SUBSCRIBE;
        }

        _db.SaveChanges();
        return true;
    }

    public bool Unsubscribe(int followerId, int followingId)
    {
        var sub = _db.UserSubscriptions.FirstOrDefault(s => s.FollowerId == followerId && s.FollowingId == followingId);
        if (sub == null)
        {
            return false;
        }

        _db.UserSubscriptions.Remove(sub);

        var follower = _db.Users.Find(followerId);
        var following = _db.Users.Find(followingId);
        if (follower != null && following != null)
        {
            follower.Rating -= Constants.PlUS_RATING_FOR_SUBSCRIPTION;
            following.Rating -= Constants.PlUS_RATING_FOR_SUBSCRIBE;
        }

        _db.SaveChanges();
        return true;
    }

    public bool IsSubscribed(int followerId, int followingId) =>
        _db.UserSubscriptions.AsNoTracking().Any(s => s.FollowerId == followerId && s.FollowingId == followingId);

    private void SyncSkillTags(UserModel source, UserModel? tracked = null)
    {
        var user = tracked ?? _db.Users.Include(u => u.UserSkillTags).First(u => u.Id == source.Id);
        var tagIds = source.SkillTags
            .Select(t => _tagService.GetTagModels().FirstOrDefault(x => x.Name == t.Name)?.Id ?? 0)
            .Where(id => id > 0)
            .ToHashSet();

        user.UserSkillTags.Clear();
        foreach (var tagId in tagIds)
        {
            user.UserSkillTags.Add(new UserSkillTagModel { UserId = user.Id, TagId = tagId });
        }

        if (tracked == null)
        {
            _db.SaveChanges();
        }
    }

    private static void HydrateUser(UserModel user)
    {
        user.SubscriptionsEmails = user.Following
            .Select(s => s.Following?.Email)
            .Where(e => !string.IsNullOrEmpty(e))
            .ToHashSet()!;

        user.SubscribersEmails = user.Followers
            .Select(s => s.Follower?.Email)
            .Where(e => !string.IsNullOrEmpty(e))
            .ToHashSet()!;

        user.Subscriptions = user.SubscriptionsEmails.Count;
        user.Subscribers = user.SubscribersEmails.Count;
        user.SkillTags = user.UserSkillTags.Select(ust => ust.Tag).ToHashSet();
    }
}
