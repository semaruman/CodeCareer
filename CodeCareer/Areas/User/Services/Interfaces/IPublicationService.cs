using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces;

public interface IPublicationService
{
    List<PublicationModel> GetPublicationModels();
    PublicationModel? GetById(int id);
    void AddPublicationModel(PublicationModel publication);
    void UpdatePublicationModel(PublicationModel publication);
    void RemovePublicationModel(int id);
    bool IsOwner(int publicationId, int userId);
}
