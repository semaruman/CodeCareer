using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCareer.Areas.User.Models
{
    public class TopicModel
    {
        public int Id { get; set; }

        public int SectionId { get; set; }

        [ForeignKey(nameof(SectionId))]
        public SectionModel? Section { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Slug { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsPublished { get; set; } = true;

        public List<NoteModel> Notes { get; set; } = new();

        public List<TaskModel> Tasks { get; set; } = new();
    }
}
