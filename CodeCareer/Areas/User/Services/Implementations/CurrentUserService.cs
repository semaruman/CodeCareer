using System.Security.Claims;
using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private UserModel _cachedUser; // Кэш для текущего запроса

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public UserModel CurrentUser
        {
            get
            {
                // Если уже загрузили в этом запросе - возвращаем из кэша
                if (_cachedUser != null)
                    return _cachedUser;

                // Получаем email из АУТЕНТИФИЦИРОВАННЫХ claims
                var email = _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue(ClaimTypes.Email);

                // Также можно получить ID
                var userId = _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(userId))
                {
                    return new UserModel(); // Не аутентифицирован
                }

                // Загружаем из БД
                if (!string.IsNullOrEmpty(email))
                    _cachedUser = _userService.GetUserByEmail(email);

                return _cachedUser ?? new UserModel();
            }
            set
            {
                // Сохраняем в БД
                if (value != null && !string.IsNullOrEmpty(value.Email))
                {
                    var existingUser = _userService.GetUserByEmail(value.Email);
                    if (existingUser != null)
                    {
                        _userService.UpdateUserModel(value);
                    }
                    else
                    {
                        _userService.AddUserModel(value);
                    }

                    // Кэшируем для текущего запроса
                    _cachedUser = value;

                    // НЕ СОЗДАЕМ CLAIMS ЗДЕСЬ!
                    // Аутентификация должна быть в контроллере через SignInAsync
                }
            }
        }
    }
}