using System.Text.Json;

namespace CodeCareer.Areas.User.Models
{
    public class PublicationModelDb
    {
        private readonly string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "User", "Data", "publication_db.json");

        public List<PublicationModel> GetPublicationModels()
        {
            List<PublicationModel> publications = new List<PublicationModel>();

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
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

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    publications = JsonSerializer.Deserialize<List<PublicationModel>>(json) ?? new List<PublicationModel>();
                }
            }

            publications.Add(publication);
            string jsonWrite = JsonSerializer.Serialize(publications);
            File.WriteAllText(filepath, jsonWrite);
        }

        public void RemovePublicationModel(string content, string userName)
        {
            List<PublicationModel> publications = new List<PublicationModel>();

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    publications = JsonSerializer.Deserialize<List<PublicationModel>>(json) ?? new List<PublicationModel>();
                }
            }

            publications = publications.Where(p => !(p.Content == content && p.User.FullName == userName)).ToList();
            string jsonWrite = JsonSerializer.Serialize(publications);
            File.WriteAllText(filepath, jsonWrite);
        }
    }
}