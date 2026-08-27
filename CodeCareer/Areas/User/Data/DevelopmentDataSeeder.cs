namespace CodeCareer.Areas.User.Data;

public static class DevelopmentDataSeeder
{
    public static void Seed(ApplicationDbContext db) =>
        LearningDatabaseInitializer.SeedCurriculumOnly(db);
}
