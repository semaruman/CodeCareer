using CodeCareer.Areas.User.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }

        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // настройка модели тегов
            modelBuilder.Entity<TagModel>(entity => {
                entity.Property(p  => p.Name).HasColumnName("name");
                entity.Property(p => p.ImgPath).HasColumnName("img_path");
            });

            // настройка модели задач
            modelBuilder.Entity<TaskModel>(entity =>
            {
                entity.Property(p => p.Name).HasColumnName("name");
                entity.Property(p => p.Type).HasColumnName("type");
                entity.Property(p => p.Content).HasColumnName("content");
                entity.Property(p => p.InputContent).HasColumnName("input_content");
                entity.Property(p => p.OutputContent).HasColumnName("output_content");
                entity.Property(p => p.AllInputStrings).HasColumnName("all_input_strings");
                entity.Property(p => p.AllOutputStrings).HasColumnName("all_output_strings");
            });
        }
    }
}
