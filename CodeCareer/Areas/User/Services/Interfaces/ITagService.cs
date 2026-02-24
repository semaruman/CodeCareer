using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ITagService
    {
        public List<TagModel> GetTagModels();
        public void AddTagModel(TagModel tag);
        public void RemoveTagModel(string tagName);
    }
}
