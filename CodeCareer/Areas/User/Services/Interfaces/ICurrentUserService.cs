using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ICurrentUserService
    {
        UserModel CurrentUser { get; set; }
    }
}
