using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface INoteService
    {
        List<NoteModel> GetByTopicId(int topicId);
        NoteModel? GetById(int id);
        NoteModel? GetFirstByTopicId(int topicId);
        void Add(NoteModel note);
        void Update(NoteModel note);
        void Remove(int id);
        List<NoteModel> Search(string query);
    }
}
