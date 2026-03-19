using CodeCareer.Areas.User.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }

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
            modelBuilder.Entity<TagModel>(entity => {
                entity.Property(p  => p.Name).HasColumnName("name");
                entity.Property(p => p.ImgPath).HasColumnName("img_path");
            });
        }
    }
}
