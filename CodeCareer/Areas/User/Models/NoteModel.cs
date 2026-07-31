using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCareer.Areas.User.Models
{
    public class NoteModel
    {
        public int Id { get; set; }

        public int TopicId { get; set; }

        [ForeignKey(nameof(TopicId))]
        public TopicModel? Topic { get; set; }

        [Required(ErrorMessage = "Введите заголовок конспекта")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите текст конспекта")]
        public string BodyMarkdown { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
