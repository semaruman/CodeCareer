using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface IPublicationService
    {
        public List<PublicationModel> GetPublicationModels();
        public void AddPublicationModel(PublicationModel publication);
        public void RemovePublicationModel(string content, string userName);
    }
}
