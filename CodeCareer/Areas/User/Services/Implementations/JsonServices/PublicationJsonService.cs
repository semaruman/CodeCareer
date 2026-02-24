using System.Text.Json;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;

namespace CodeCareer.Areas.User.Services.Implementations.JsonServices
{
    public class PublicationJsonService : IPublicationService
    {
        private readonly string _filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "User", "Data", "JsonFiles", "publication_db.json");

        public List<PublicationModel> GetPublicationModels()
        {
            List<PublicationModel> publications = new List<PublicationModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    publications = JsonSerializer.Deserialize<List<PublicationModel>>(json) ?? new List<PublicationModel>();
                }
            }

            return publications;
        }

        public void AddPublicationModel(PublicationModel publication)
        {
            List<PublicationModel> publications = new List<PublicationModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    publications = JsonSerializer.Deserialize<List<PublicationModel>>(json) ?? new List<PublicationModel>();
                }
            }

            publications.Add(publication);
            string jsonWrite = JsonSerializer.Serialize(publications);
            File.WriteAllText(_filepath, jsonWrite);
        }

        public void RemovePublicationModel(string content, string userName)
        {
            List<PublicationModel> publications = new List<PublicationModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    publications = JsonSerializer.Deserialize<List<PublicationModel>>(json) ?? new List<PublicationModel>();
                }
            }

            publications = publications.Where(p => !(p.Content == content && p.User.FullName == userName)).ToList();
            string jsonWrite = JsonSerializer.Serialize(publications);
            File.WriteAllText(_filepath, jsonWrite);
        }
    }
}