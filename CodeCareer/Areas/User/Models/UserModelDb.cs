namespace CodeCareer.Areas.User.Models
{
    public class UserModelDb
    {
        private readonly string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "User", "Data", "user_db.json");
    }
}
