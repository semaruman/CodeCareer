using CodeCareer.Areas.User.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<PublicationModel> Publications => Set<PublicationModel>();
    public DbSet<CommentModel> Comments => Set<CommentModel>();
    public DbSet<UserSubscriptionModel> UserSubscriptions => Set<UserSubscriptionModel>();
    public DbSet<PublicationTagModel> PublicationTags => Set<PublicationTagModel>();
    public DbSet<UserSkillTagModel> UserSkillTags => Set<UserSkillTagModel>();
    public DbSet<NotificationModel> Notifications => Set<NotificationModel>();
    public DbSet<UserAchievementModel> UserAchievements => Set<UserAchievementModel>();
    public DbSet<SubmissionModel> Submissions => Set<SubmissionModel>();
    public DbSet<TagModel> Tags => Set<TagModel>();
    public DbSet<TaskModel> Tasks => Set<TaskModel>();
    public DbSet<SectionModel> Sections => Set<SectionModel>();
    public DbSet<TopicModel> Topics => Set<TopicModel>();
    public DbSet<NoteModel> Notes => Set<NoteModel>();
    public DbSet<CourseModel> Courses => Set<CourseModel>();
    public DbSet<CourseTopicModel> CourseTopics => Set<CourseTopicModel>();
    public DbSet<UserTopicProgressModel> UserTopicProgress => Set<UserTopicProgressModel>();
    public DbSet<UserTaskProgressModel> UserTaskProgress => Set<UserTaskProgressModel>();
    public DbSet<ChatHistoryModel> ChatHistories => Set<ChatHistoryModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsers(modelBuilder);
        ConfigurePublications(modelBuilder);
        ConfigureComments(modelBuilder);
        ConfigureSocial(modelBuilder);
        ConfigureLearning(modelBuilder);
        ConfigureSubmissions(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(100);
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(256);
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(512);
            entity.Property(u => u.BirthDate).HasColumnName("birth_date");
            entity.Property(u => u.Info).HasColumnName("info");
            entity.Property(u => u.Rating).HasColumnName("rating");
            entity.Property(u => u.Status).HasColumnName("status");
            entity.Property(u => u.Role).HasColumnName("role").HasMaxLength(32);
            entity.Property(u => u.AvatarPath).HasColumnName("avatar_path").HasMaxLength(256);
            entity.Property(u => u.ShowSubscriptions).HasColumnName("show_subscriptions");
            entity.Property(u => u.MustChangePassword).HasColumnName("must_change_password");
            entity.Property(u => u.FailedLoginAttempts).HasColumnName("failed_login_attempts");
            entity.Property(u => u.LockoutEndUtc).HasColumnName("lockout_end_utc");
            entity.Property(u => u.RegistrationDate).HasColumnName("registration_date");
        });
    }

    private static void ConfigurePublications(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicationModel>(entity =>
        {
            entity.ToTable("publications");
            entity.Property(p => p.CreatedDate).HasColumnName("created_date");
            entity.Property(p => p.UserId).HasColumnName("user_id");
            entity.Property(p => p.Content).HasColumnName("content").HasMaxLength(1000);
            entity.HasIndex(p => p.UserId);
            entity.HasOne(p => p.User).WithMany(u => u.Publications).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PublicationTagModel>(entity =>
        {
            entity.ToTable("publication_tags");
            entity.HasKey(pt => new { pt.PublicationId, pt.TagId });
            entity.Property(pt => pt.PublicationId).HasColumnName("publication_id");
            entity.Property(pt => pt.TagId).HasColumnName("tag_id");
            entity.HasIndex(pt => pt.TagId);
            entity.HasOne(pt => pt.Publication).WithMany(p => p.PublicationTags).HasForeignKey(pt => pt.PublicationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(pt => pt.Tag).WithMany().HasForeignKey(pt => pt.TagId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureComments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommentModel>(entity =>
        {
            entity.ToTable("comments");
            entity.Property(c => c.PublicationId).HasColumnName("publication_id");
            entity.Property(c => c.UserId).HasColumnName("user_id");
            entity.Property(c => c.Content).HasColumnName("content").HasMaxLength(1000);
            entity.Property(c => c.CreatedDate).HasColumnName("created_date");
            entity.HasIndex(c => c.PublicationId);
            entity.HasOne(c => c.Publication).WithMany(p => p.Comments).HasForeignKey(c => c.PublicationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSocial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSubscriptionModel>(entity =>
        {
            entity.ToTable("user_subscriptions");
            entity.HasIndex(s => new { s.FollowerId, s.FollowingId }).IsUnique();
            entity.HasIndex(s => s.FollowerId);
            entity.HasIndex(s => s.FollowingId);
            entity.Property(s => s.FollowerId).HasColumnName("follower_id");
            entity.Property(s => s.FollowingId).HasColumnName("following_id");
            entity.Property(s => s.CreatedAt).HasColumnName("created_at");
            entity.HasOne(s => s.Follower).WithMany(u => u.Following).HasForeignKey(s => s.FollowerId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(s => s.Following).WithMany(u => u.Followers).HasForeignKey(s => s.FollowingId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserSkillTagModel>(entity =>
        {
            entity.ToTable("user_skill_tags");
            entity.HasKey(ust => new { ust.UserId, ust.TagId });
            entity.Property(ust => ust.UserId).HasColumnName("user_id");
            entity.Property(ust => ust.TagId).HasColumnName("tag_id");
            entity.HasOne(ust => ust.User).WithMany(u => u.UserSkillTags).HasForeignKey(ust => ust.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ust => ust.Tag).WithMany().HasForeignKey(ust => ust.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NotificationModel>(entity =>
        {
            entity.ToTable("notifications");
            entity.Property(n => n.UserId).HasColumnName("user_id");
            entity.Property(n => n.Type).HasColumnName("type");
            entity.Property(n => n.Message).HasColumnName("message");
            entity.Property(n => n.IsRead).HasColumnName("is_read");
            entity.Property(n => n.CreatedAt).HasColumnName("created_at");
            entity.HasIndex(n => new { n.UserId, n.IsRead });
            entity.HasOne(n => n.User).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserAchievementModel>(entity =>
        {
            entity.ToTable("user_achievements");
            entity.Property(a => a.UserId).HasColumnName("user_id");
            entity.Property(a => a.AchievementKey).HasColumnName("achievement_key");
            entity.Property(a => a.EarnedAt).HasColumnName("earned_at");
            entity.HasIndex(a => new { a.UserId, a.AchievementKey }).IsUnique();
            entity.HasOne(a => a.User).WithMany(u => u.Achievements).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSubmissions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubmissionModel>(entity =>
        {
            entity.ToTable("submissions");
            entity.Property(s => s.UserId).HasColumnName("user_id");
            entity.Property(s => s.TaskId).HasColumnName("task_id");
            entity.Property(s => s.Language).HasColumnName("language");
            entity.Property(s => s.SourceCode).HasColumnName("source_code");
            entity.Property(s => s.Status).HasColumnName("status");
            entity.Property(s => s.Score).HasColumnName("score");
            entity.Property(s => s.ExecutionTime).HasColumnName("execution_time");
            entity.Property(s => s.MemoryUsed).HasColumnName("memory_used");
            entity.Property(s => s.CreatedAt).HasColumnName("created_at");
            entity.HasIndex(s => new { s.UserId, s.TaskId });
            entity.HasIndex(s => s.CreatedAt);
            entity.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(s => s.Task).WithMany().HasForeignKey(s => s.TaskId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureLearning(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TagModel>(entity =>
        {
            entity.ToTable("tags");
            entity.Property(p => p.Name).HasColumnName("name");
            entity.Property(p => p.ImgPath).HasColumnName("img_path");
        });

        modelBuilder.Entity<TaskModel>(entity =>
        {
            entity.ToTable("tasks");
            entity.Property(p => p.Name).HasColumnName("name");
            entity.Property(p => p.Type).HasColumnName("type");
            entity.Property(p => p.TopicId).HasColumnName("topic_id");
            entity.Property(p => p.Content).HasColumnName("content");
            entity.Property(p => p.InputContent).HasColumnName("input_content");
            entity.Property(p => p.OutputContent).HasColumnName("output_content");
            entity.Property(p => p.AllInputStrings).HasColumnName("all_input_strings");
            entity.Property(p => p.AllOutputStrings).HasColumnName("all_output_strings");
            entity.HasOne(p => p.Topic).WithMany(t => t.Tasks).HasForeignKey(p => p.TopicId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SectionModel>(entity =>
        {
            entity.ToTable("sections");
            entity.Property(p => p.Title).HasColumnName("title");
            entity.Property(p => p.SortOrder).HasColumnName("sort_order");
        });

        modelBuilder.Entity<TopicModel>(entity =>
        {
            entity.ToTable("topics");
            entity.Property(p => p.SectionId).HasColumnName("section_id");
            entity.Property(p => p.Title).HasColumnName("title");
            entity.Property(p => p.Slug).HasColumnName("slug");
            entity.Property(p => p.SortOrder).HasColumnName("sort_order");
            entity.Property(p => p.IsPublished).HasColumnName("is_published");
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.HasOne(p => p.Section).WithMany(s => s.Topics).HasForeignKey(p => p.SectionId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NoteModel>(entity =>
        {
            entity.ToTable("notes");
            entity.Property(p => p.TopicId).HasColumnName("topic_id");
            entity.Property(p => p.Title).HasColumnName("title");
            entity.Property(p => p.BodyMarkdown).HasColumnName("body_markdown").HasColumnType("longtext");
            entity.Property(p => p.SortOrder).HasColumnName("sort_order");
            entity.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne(p => p.Topic).WithMany(t => t.Notes).HasForeignKey(p => p.TopicId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CourseModel>(entity =>
        {
            entity.ToTable("courses");
            entity.Property(p => p.Title).HasColumnName("title");
            entity.Property(p => p.Description).HasColumnName("description").HasColumnType("longtext");
            entity.Property(p => p.SortOrder).HasColumnName("sort_order");
            entity.Property(p => p.IsPublished).HasColumnName("is_published");
        });

        modelBuilder.Entity<CourseTopicModel>(entity =>
        {
            entity.ToTable("course_topics");
            entity.Property(p => p.CourseId).HasColumnName("course_id");
            entity.Property(p => p.TopicId).HasColumnName("topic_id");
            entity.Property(p => p.SortOrder).HasColumnName("sort_order");
            entity.HasOne(p => p.Course).WithMany(c => c.CourseTopics).HasForeignKey(p => p.CourseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Topic).WithMany().HasForeignKey(p => p.TopicId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserTopicProgressModel>(entity =>
        {
            entity.ToTable("user_topic_progress");
            entity.Property(p => p.UserEmail).HasColumnName("user_email");
            entity.Property(p => p.TopicId).HasColumnName("topic_id");
            entity.Property(p => p.NoteId).HasColumnName("note_id");
            entity.Property(p => p.NoteReadAt).HasColumnName("note_read_at");
            entity.HasIndex(p => new { p.UserEmail, p.TopicId, p.NoteId });
        });

        modelBuilder.Entity<UserTaskProgressModel>(entity =>
        {
            entity.ToTable("user_task_progress");
            entity.Property(p => p.UserEmail).HasColumnName("user_email");
            entity.Property(p => p.TaskId).HasColumnName("task_id");
            entity.Property(p => p.Status).HasColumnName("status");
            entity.Property(p => p.SolvedAt).HasColumnName("solved_at");
            entity.HasIndex(p => new { p.UserEmail, p.TaskId }).IsUnique();
        });

        modelBuilder.Entity<ChatHistoryModel>(entity =>
        {
            entity.ToTable("chat_histories");
            entity.Property(p => p.UserEmail).HasColumnName("user_email");
            entity.Property(p => p.NoteId).HasColumnName("note_id");
            entity.Property(p => p.Role).HasColumnName("role");
            entity.Property(p => p.Content).HasColumnName("content").HasColumnType("longtext");
            entity.Property(p => p.CreatedAt).HasColumnName("created_at");
        });
    }
}
