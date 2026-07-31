using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface IChatHistoryService
    {
        void AddMessage(string userEmail, int noteId, string role, string content);
        List<ChatHistoryModel> GetByNote(string userEmail, int noteId);
    }
}
