using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ISectionService
    {
        List<SectionModel> GetSectionsWithTopics(bool onlyPublishedTopics = true);
        SectionModel? GetById(int id);
        void Add(SectionModel section);
        void Update(SectionModel section);
        void Remove(int id);
    }
}
