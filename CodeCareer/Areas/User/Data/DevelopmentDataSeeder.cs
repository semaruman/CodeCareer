using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Data;

public static class DevelopmentDataSeeder
{
    private static readonly string[] DefaultTagNames =
    [
        "C#",
        ".NET",
        "Python",
        "SQL",
        "JavaScript",
        "C++",
        "Algorithms",
        "TypeScript",
        "Architecture",
        "NoSql",
    ];

    public static void Seed(ApplicationDbContext db) =>
        LearningDatabaseInitializer.SeedCurriculumOnly(db);

    public static void SeedTags(ApplicationDbContext db)
    {
        var existingNames = db.Tags.Select(t => t.Name).ToHashSet();
        var missing = DefaultTagNames
            .Where(name => !existingNames.Contains(name))
            .Select(name => new TagModel { Name = name, ImgPath = string.Empty })
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        db.Tags.AddRange(missing);
        db.SaveChanges();
    }
}
