using System.Security.Claims;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;

namespace CodeCareer.Areas.User.Services.Implementations;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private UserModel? _cachedUser;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public UserModel CurrentUser
    {
        get
        {
            if (_cachedUser != null)
            {
                return _cachedUser;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return new UserModel();
            }

            var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out var userId))
            {
                _cachedUser = _userService.GetUserById(userId);
                if (_cachedUser != null)
                {
                    return _cachedUser;
                }
            }

            var email = httpContext.User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(email))
            {
                _cachedUser = _userService.GetUserByEmail(email);
            }

            return _cachedUser ?? new UserModel();
        }
        set => _cachedUser = value;
    }
}
