using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CodeCareer.Areas.User.Models;

public class UserModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Имя обязательно")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен")]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Stored hash. Never expose in views.</summary>
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Form-only password, not persisted.</summary>
    [NotMapped]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public DateTime? BirthDate { get; set; }

    [MaxLength(300, ErrorMessage = "текст не должен превышать 300 символов")]
    public string Info { get; set; } = string.Empty;

    public int Rating { get; set; }

    [MaxLength(64)]
    public string Status { get; set; } = "Начинающий";

    [MaxLength(32)]
    public string Role { get; set; } = Security.Roles.User;

    [MaxLength(256)]
    public string? AvatarPath { get; set; }

    public bool ShowSubscriptions { get; set; } = true;

    public bool MustChangePassword { get; set; }

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutEndUtc { get; set; }

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    // Legacy denormalized counters — kept for display; source of truth is UserSubscriptions.
    [NotMapped]
    public int Subscribers { get; set; }

    [NotMapped]
    [JsonInclude]
    public HashSet<string> SubscribersEmails { get; set; } = new();

    [NotMapped]
    public int Subscriptions { get; set; }

    [NotMapped]
    [JsonInclude]
    public HashSet<string> SubscriptionsEmails { get; set; } = new();

    [NotMapped]
    public HashSet<TagModel> SkillTags { get; set; } = new();

    public ICollection<UserSubscriptionModel> Following { get; set; } = new List<UserSubscriptionModel>();
    public ICollection<UserSubscriptionModel> Followers { get; set; } = new List<UserSubscriptionModel>();
    public ICollection<UserSkillTagModel> UserSkillTags { get; set; } = new List<UserSkillTagModel>();
    public ICollection<PublicationModel> Publications { get; set; } = new List<PublicationModel>();
    public ICollection<NotificationModel> Notifications { get; set; } = new List<NotificationModel>();
    public ICollection<UserAchievementModel> Achievements { get; set; } = new List<UserAchievementModel>();
}
