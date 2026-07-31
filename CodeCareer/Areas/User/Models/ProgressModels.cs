using System.ComponentModel.DataAnnotations;

namespace CodeCareer.Areas.User.Models
{
    public class UserTopicProgressModel
    {
        public int Id { get; set; }

        [Required]
        public string UserEmail { get; set; } = string.Empty;

        public int TopicId { get; set; }

        public int? NoteId { get; set; }

        public DateTime? NoteReadAt { get; set; }
    }

    public class UserTaskProgressModel
    {
        public int Id { get; set; }

        [Required]
        public string UserEmail { get; set; } = string.Empty;

        public int TaskId { get; set; }

        public string Status { get; set; } = "solved";

        public DateTime SolvedAt { get; set; } = DateTime.UtcNow;
    }

    public class ChatHistoryModel
    {
        public int Id { get; set; }

        [Required]
        public string UserEmail { get; set; } = string.Empty;

        public int NoteId { get; set; }

        public string Role { get; set; } = "user";

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
