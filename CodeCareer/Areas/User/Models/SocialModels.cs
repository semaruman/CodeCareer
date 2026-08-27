using System.ComponentModel.DataAnnotations;

namespace CodeCareer.Areas.User.Models;

public class UserSubscriptionModel
{
    public int Id { get; set; }

    public int FollowerId { get; set; }

    public UserModel Follower { get; set; } = null!;

    public int FollowingId { get; set; }

    public UserModel Following { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class PublicationTagModel
{
    public int PublicationId { get; set; }
    public PublicationModel Publication { get; set; } = null!;
    public int TagId { get; set; }
    public TagModel Tag { get; set; } = null!;
}

public class UserSkillTagModel
{
    public int UserId { get; set; }
    public UserModel User { get; set; } = null!;
    public int TagId { get; set; }
    public TagModel Tag { get; set; } = null!;
}

public class NotificationModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public UserModel User { get; set; } = null!;

    [MaxLength(64)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class UserAchievementModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public UserModel User { get; set; } = null!;

    [MaxLength(64)]
    public string AchievementKey { get; set; } = string.Empty;

    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
}

public static class AchievementKeys
{
    public const string FirstTaskSolved = "FirstTaskSolved";
    public const string TenTasksSolved = "TenTasksSolved";
    public const string FirstPost = "FirstPost";
    public const string FirstFollower = "FirstFollower";
}

public static class NotificationTypes
{
    public const string NewFollower = "NewFollower";
    public const string CommentOnPost = "CommentOnPost";
}
