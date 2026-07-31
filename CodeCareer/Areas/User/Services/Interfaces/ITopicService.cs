using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ITopicService
    {
        List<TopicModel> GetAll(bool onlyPublished = true);
        TopicModel? GetById(int id);
        TopicModel? GetBySlug(string slug);
        void Add(TopicModel topic);
        void Update(TopicModel topic);
        void Remove(int id);
    }
}
