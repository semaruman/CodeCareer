using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ICurrentUserService
    {
        bool IsAuthenticated { get; }

        UserModel CurrentUser { get; set; }
    }
}
