using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ITagService
    {
        public List<TagModel> GetTagModels();
        public void AddPublicationModel(TagModel tag);
        public void RemovePublicationModel(string tagName);
    }
}
