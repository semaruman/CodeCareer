using CodeCareer.Areas.User.Models;

namespace CodeCareer.Security;

public interface IPasswordService
{
    string HashPassword(UserModel user, string password);
    bool VerifyPassword(UserModel user, string password, out bool needsRehash);
    bool MeetsPolicy(string password, out string? error);
}
