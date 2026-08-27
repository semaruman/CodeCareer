using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class ChatHistoryMySqlEfService : IChatHistoryService
{
    private readonly ApplicationDbContext _db;

    public ChatHistoryMySqlEfService(ApplicationDbContext db) => _db = db;

    public void AddMessage(string userEmail, int noteId, string role, string content)
    {
        if (content.Length > 8000)
        {
            content = content[..8000];
        }

        _db.ChatHistories.Add(new ChatHistoryModel
        {
            UserEmail = userEmail,
            NoteId = noteId,
            Role = role,
            Content = content,
            CreatedAt = DateTime.UtcNow,
        });
        _db.SaveChanges();
    }

    public List<ChatHistoryModel> GetByNote(string userEmail, int noteId) =>
        _db.ChatHistories.AsNoTracking()
            .Where(c => c.UserEmail == userEmail && c.NoteId == noteId)
            .OrderBy(c => c.CreatedAt).ToList();
}
