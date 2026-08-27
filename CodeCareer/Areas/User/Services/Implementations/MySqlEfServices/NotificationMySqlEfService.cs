using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class NotificationMySqlEfService : INotificationService
{
    private readonly ApplicationDbContext _db;

    public NotificationMySqlEfService(ApplicationDbContext db)
    {
        _db = db;
    }

    public void Add(int userId, string type, string message)
    {
        _db.Notifications.Add(new NotificationModel
        {
            UserId = userId,
            Type = type,
            Message = message,
            CreatedAt = DateTime.UtcNow,
        });
        _db.SaveChanges();
    }

    public List<NotificationModel> GetUnread(int userId) =>
        _db.Notifications.AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

    public List<NotificationModel> GetAll(int userId, int take = 50) =>
        _db.Notifications.AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .ToList();

    public void MarkRead(int notificationId, int userId)
    {
        var notification = _db.Notifications.FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);
        if (notification == null)
        {
            return;
        }

        notification.IsRead = true;
        _db.SaveChanges();
    }

    public int CountUnread(int userId) =>
        _db.Notifications.AsNoTracking().Count(n => n.UserId == userId && !n.IsRead);
}
