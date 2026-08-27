using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces;

public interface IUserService
{
    List<UserModel> GetUserModels();
    void AddUserModel(UserModel user);
    void RemoveUserModel(int userId);
    void UpdateUserModel(UserModel user);
    UserModel? GetUserByEmail(string email);
    UserModel? GetUserById(int id);
    bool Subscribe(int followerId, int followingId);
    bool Unsubscribe(int followerId, int followingId);
    bool IsSubscribed(int followerId, int followingId);
}
