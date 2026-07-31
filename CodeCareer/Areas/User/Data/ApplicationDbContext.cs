using CodeCareer.Areas.User.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<SectionModel> Sections { get; set; }
        public DbSet<TopicModel> Topics { get; set; }
        public DbSet<NoteModel> Notes { get; set; }
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<CourseTopicModel> CourseTopics { get; set; }
        public DbSet<UserTopicProgressModel> UserTopicProgress { get; set; }
        public DbSet<UserTaskProgressModel> UserTaskProgress { get; set; }
        public DbSet<ChatHistoryModel> ChatHistories { get; set; }

        public ApplicationDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = Constants.CONNECTION_STRING;
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
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
}
