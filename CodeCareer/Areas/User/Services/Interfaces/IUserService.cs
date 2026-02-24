using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface IUserService
    {
        public List<UserModel> GetUserModels();
        public void AddUserModel(UserModel user);
        public void RemoveUserModel(UserModel user);

        public void UpdateUserModel(UserModel user);
    }
}
