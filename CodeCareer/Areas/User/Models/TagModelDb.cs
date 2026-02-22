using System.Text.Json;

namespace CodeCareer.Areas.User.Models
{
    public class TagModelDb
    {
        private readonly string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "User", "Data", "tag_db.json");

        public List<TagModel> GetTagModels()
        {
            List<TagModel> tags = new List<TagModel>();

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    tags = JsonSerializer.Deserialize<List<TagModel>>(json) ?? new List<TagModel>();
                }
            }

            return tags;
        }

        public void AddPublicationModel(TagModel tag)
        {
            List<TagModel> tags = new List<TagModel>();

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    tags = JsonSerializer.Deserialize<List<TagModel>>(json) ?? new List<TagModel>();
                }
            }

            tags.Add(tag);
            string jsonWrite = JsonSerializer.Serialize(tags);
            File.WriteAllText(filepath, jsonWrite);
        }

        public void RemovePublicationModel(string tagName)
        {
            List<TagModel> tags = new List<TagModel>();

            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    tags = JsonSerializer.Deserialize<List<TagModel>>(json) ?? new List<TagModel>();
                }
            }

            tags = tags.Where(t => !(t.Name == tagName)).ToList();
            string jsonWrite = JsonSerializer.Serialize(tags);
            File.WriteAllText(filepath, jsonWrite);
        }
    }
}
