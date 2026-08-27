using CodeCareer.Areas.User.Models;
using Microsoft.AspNetCore.Identity;

namespace CodeCareer.Security;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<UserModel> _hasher = new();

    public string HashPassword(UserModel user, string password) =>
        _hasher.HashPassword(user, password);

    public bool VerifyPassword(UserModel user, string password, out bool needsRehash)
    {
        needsRehash = false;
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return false;
        }

        // Legacy plaintext migration: hashes start with AQAAAA (Identity v3 format).
        if (!user.PasswordHash.StartsWith("AQAAAA", StringComparison.Ordinal))
        {
            var legacyMatch = string.Equals(user.PasswordHash, password, StringComparison.Ordinal);
            if (legacyMatch)
            {
                needsRehash = true;
            }
            return legacyMatch;
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        needsRehash = result == PasswordVerificationResult.SuccessRehashNeeded;
        return result != PasswordVerificationResult.Failed;
    }

    public bool MeetsPolicy(string password, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(password))
        {
            error = "Пароль обязателен";
            return false;
        }

        if (password.Length < 8)
        {
            error = "Минимальная длина пароля — 8 символов";
            return false;
        }

        if (password.Length > 128)
        {
            error = "Максимальная длина пароля — 128 символов";
            return false;
        }

        return true;
    }
}
