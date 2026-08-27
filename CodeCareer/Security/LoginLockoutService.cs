using System.Collections.Concurrent;

namespace CodeCareer.Security;

public class LoginLockoutService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private readonly ConcurrentDictionary<string, (int Failures, DateTime? LockoutEnd)> _attempts = new();

    public bool IsLockedOut(string email, out TimeSpan? remaining)
    {
        remaining = null;
        if (!_attempts.TryGetValue(Normalize(email), out var state))
        {
            return false;
        }

        if (state.LockoutEnd is { } lockoutEnd && lockoutEnd > DateTime.UtcNow)
        {
            remaining = lockoutEnd - DateTime.UtcNow;
            return true;
        }

        return false;
    }

    public void RecordFailure(string email)
    {
        var key = Normalize(email);
        _attempts.AddOrUpdate(
            key,
            _ => (1, null),
            (_, existing) =>
            {
                var failures = existing.Failures + 1;
                var lockout = failures >= MaxFailedAttempts
                    ? DateTime.UtcNow.Add(LockoutDuration)
                    : existing.LockoutEnd;
                return (failures, lockout);
            });
    }

    public void Reset(string email) => _attempts.TryRemove(Normalize(email), out _);

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
