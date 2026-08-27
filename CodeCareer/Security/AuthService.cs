using System.Security.Claims;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CodeCareer.Security;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(HttpContext httpContext, string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResult> RegisterAsync(HttpContext httpContext, UserModel user, string password, CancellationToken cancellationToken = default);
    Task SignOutAsync(HttpContext httpContext);
    ClaimsPrincipal CreatePrincipal(UserModel user);
}

public sealed record AuthResult(bool Success, string? ErrorMessage = null, bool MustChangePassword = false);

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IPasswordService _passwordService;
    private readonly LoginLockoutService _lockout;

    public AuthService(IUserService userService, IPasswordService passwordService, LoginLockoutService lockout)
    {
        _userService = userService;
        _passwordService = passwordService;
        _lockout = lockout;
    }

    public async Task<AuthResult> SignInAsync(HttpContext httpContext, string email, string password, CancellationToken cancellationToken = default)
    {
        email = email.Trim();
        if (_lockout.IsLockedOut(email, out var remaining))
        {
            var minutes = Math.Max(1, (int)Math.Ceiling(remaining!.Value.TotalMinutes));
            return new AuthResult(false, $"Слишком много попыток входа. Повторите через {minutes} мин.");
        }

        var user = _userService.GetUserByEmail(email);
        if (user == null)
        {
            _lockout.RecordFailure(email);
            return new AuthResult(false, "Неверный email или пароль");
        }

        if (!_passwordService.VerifyPassword(user, password, out var needsRehash))
        {
            _lockout.RecordFailure(email);
            return new AuthResult(false, "Неверный email или пароль");
        }

        if (needsRehash)
        {
            user.PasswordHash = _passwordService.HashPassword(user, password);
            _userService.UpdateUserModel(user);
        }

        _lockout.Reset(email);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, CreatePrincipal(user));
        return new AuthResult(true, MustChangePassword: user.MustChangePassword);
    }

    public async Task<AuthResult> RegisterAsync(HttpContext httpContext, UserModel user, string password, CancellationToken cancellationToken = default)
    {
        user.Email = user.Email.Trim();
        if (_userService.GetUserByEmail(user.Email) != null)
        {
            return new AuthResult(false, "Пользователь с таким email уже существует");
        }

        if (!_passwordService.MeetsPolicy(password, out var policyError))
        {
            return new AuthResult(false, policyError);
        }

        user.PasswordHash = _passwordService.HashPassword(user, password);
        user.Role = Roles.User;
        user.RegistrationDate = DateTime.UtcNow;
        _userService.AddUserModel(user);

        var created = _userService.GetUserByEmail(user.Email)!;
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, CreatePrincipal(created));
        return new AuthResult(true);
    }

    public Task SignOutAsync(HttpContext httpContext) =>
        httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    public ClaimsPrincipal CreatePrincipal(UserModel user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName ?? user.Email),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, string.IsNullOrEmpty(user.Role) ? Roles.User : user.Role),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}
