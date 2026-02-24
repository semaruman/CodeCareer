using System.Text.Json;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;

namespace CodeCareer.Areas.User.Services.Implementations.JsonServices
{
    public class TagJsonService : ITagService
    {
        private readonly string _filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "User", "Data", "JsonFiles", "tag_db.json");

        public List<TagModel> GetTagModels()
        {
            List<TagModel> tags = new List<TagModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    tags = JsonSerializer.Deserialize<List<TagModel>>(json) ?? new List<TagModel>();
                }
            }

            return tags;
        }

        public void AddTagModel(TagModel tag)
        {
            List<TagModel> tags = new List<TagModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    tags = JsonSerializer.Deserialize<List<TagModel>>(json) ?? new List<TagModel>();
                }
            }

            tags.Add(tag);
            string jsonWrite = JsonSerializer.Serialize(tags);
            File.WriteAllText(_filepath, jsonWrite);
        }

        public void RemoveTagModel(string tagName)
        {
            List<TagModel> tags = new List<TagModel>();

            if (File.Exists(_filepath))
            {
                string json = File.ReadAllText(_filepath);
                if (!string.IsNullOrEmpty(json))
                {
                    tags = JsonSerializer.Deserialize<List<TagModel>>(json) ?? new List<TagModel>();
                }
            }

            tags = tags.Where(t => !(t.Name == tagName)).ToList();
            string jsonWrite = JsonSerializer.Serialize(tags);
            File.WriteAllText(_filepath, jsonWrite);
        }
    }
}
