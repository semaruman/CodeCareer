using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCareer.Areas.User.Models
{
    public class SectionModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public List<TopicModel> Topics { get; set; } = new();
    }
}
