using CodeCareer.Areas.User.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Data;

/// <summary>Migrates legacy denormalized columns into normalized tables.</summary>
public static class LegacyDataMigrator
{
    public static void Migrate(ApplicationDbContext db)
    {
        MigratePasswordColumn(db);
        MigrateSubscriptions(db);
        MigratePublicationTags(db);
        MigrateSkillTags(db);
        MigrateCommentUserIds(db);
    }

    private static void MigratePasswordColumn(ApplicationDbContext db)
    {
        try
        {
            db.Database.ExecuteSqlRaw("""
                UPDATE users SET password_hash = password
                WHERE (password_hash IS NULL OR password_hash = '') AND password IS NOT NULL AND password != ''
                """);
        }
        catch
        {
            // Legacy column may not exist on fresh installs.
        }
    }

    private static void MigrateSubscriptions(ApplicationDbContext db)
    {
        try
        {
            var legacyUsers = db.Database.SqlQueryRaw<LegacyUserSubs>(
                "SELECT id, subscriptions_emails, subscribers_emails FROM users WHERE subscriptions_emails IS NOT NULL OR subscribers_emails IS NOT NULL")
                .ToList();

            foreach (var row in legacyUsers)
            {
                MigrateUserSubscriptions(db, row.Id, row.SubscriptionsEmails, isFollowing: true);
                MigrateUserSubscriptions(db, row.Id, row.SubscribersEmails, isFollowing: false);
            }
        }
        catch
        {
            // Legacy columns absent.
        }
    }

    private static void MigrateUserSubscriptions(ApplicationDbContext db, int userId, string? emailsCsv, bool isFollowing)
    {
        if (string.IsNullOrWhiteSpace(emailsCsv))
        {
            return;
        }

        foreach (var email in emailsCsv.Split("; ", StringSplitOptions.RemoveEmptyEntries))
        {
            var other = db.Users.AsNoTracking().FirstOrDefault(u => u.Email == email);
            if (other == null)
            {
                continue;
            }

            var followerId = isFollowing ? userId : other.Id;
            var followingId = isFollowing ? other.Id : userId;

            if (!db.UserSubscriptions.Any(s => s.FollowerId == followerId && s.FollowingId == followingId))
            {
                db.UserSubscriptions.Add(new Models.UserSubscriptionModel
                {
                    FollowerId = followerId,
                    FollowingId = followingId,
                    CreatedAt = DateTime.UtcNow,
                });
            }
        }

        db.SaveChanges();
    }

    private static void MigratePublicationTags(ApplicationDbContext db)
    {
        try
        {
            var pubs = db.Database.SqlQueryRaw<LegacyPublicationTags>(
                "SELECT id, tag_names FROM publications WHERE tag_names IS NOT NULL AND tag_names != ''").ToList();

            foreach (var pub in pubs)
            {
                foreach (var tagName in pub.TagNames.Split("; ", StringSplitOptions.RemoveEmptyEntries))
                {
                    var tag = db.Tags.FirstOrDefault(t => t.Name == tagName);
                    if (tag == null)
                    {
                        continue;
                    }

                    if (!db.PublicationTags.Any(pt => pt.PublicationId == pub.Id && pt.TagId == tag.Id))
                    {
                        db.PublicationTags.Add(new Models.PublicationTagModel { PublicationId = pub.Id, TagId = tag.Id });
                    }
                }
            }

            db.SaveChanges();
        }
        catch
        {
        }
    }

    private static void MigrateSkillTags(ApplicationDbContext db)
    {
        try
        {
            var users = db.Database.SqlQueryRaw<LegacySkillTags>(
                "SELECT id, skill_tags_names FROM users WHERE skill_tags_names IS NOT NULL AND skill_tags_names != ''").ToList();

            foreach (var user in users)
            {
                foreach (var tagName in user.SkillTagsNames.Split("; ", StringSplitOptions.RemoveEmptyEntries))
                {
                    var tag = db.Tags.FirstOrDefault(t => t.Name == tagName);
                    if (tag == null)
                    {
                        continue;
                    }

                    if (!db.UserSkillTags.Any(ust => ust.UserId == user.Id && ust.TagId == tag.Id))
                    {
                        db.UserSkillTags.Add(new Models.UserSkillTagModel { UserId = user.Id, TagId = tag.Id });
                    }
                }
            }

            db.SaveChanges();
        }
        catch
        {
        }
    }

    private static void MigrateCommentUserIds(ApplicationDbContext db)
    {
        // Comments created before normalization may lack user_id; no safe auto-fix without legacy data.
    }

    private sealed record LegacyUserSubs(int Id, string? SubscriptionsEmails, string? SubscribersEmails);
    private sealed record LegacyPublicationTags(int Id, string TagNames);
    private sealed record LegacySkillTags(int Id, string SkillTagsNames);
}
