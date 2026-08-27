using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces;

public interface INotificationService
{
    void Add(int userId, string type, string message);
    List<NotificationModel> GetUnread(int userId);
    List<NotificationModel> GetAll(int userId, int take = 50);
    void MarkRead(int notificationId, int userId);
    int CountUnread(int userId);
}
