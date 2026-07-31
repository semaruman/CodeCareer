using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class ChatHistoryMySqlEfService : IChatHistoryService
    {
        public void AddMessage(string userEmail, int noteId, string role, string content)
        {
            using var db = new ApplicationDbContext();
            db.ChatHistories.Add(new ChatHistoryModel
            {
                UserEmail = userEmail,
                NoteId = noteId,
                Role = role,
                Content = content,
                CreatedAt = DateTime.UtcNow,
            });
            db.SaveChanges();
        }

        public List<ChatHistoryModel> GetByNote(string userEmail, int noteId)
        {
            using var db = new ApplicationDbContext();
            return db.ChatHistories.AsNoTracking()
                .Where(c => c.UserEmail == userEmail && c.NoteId == noteId)
                .OrderBy(c => c.CreatedAt)
                .ToList();
        }
    }
}
