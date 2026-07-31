using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCareer.Areas.User.Models
{
    public class CourseModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsPublished { get; set; } = true;

        public List<CourseTopicModel> CourseTopics { get; set; } = new();
    }

    public class CourseTopicModel
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public CourseModel? Course { get; set; }

        public int TopicId { get; set; }

        [ForeignKey(nameof(TopicId))]
        public TopicModel? Topic { get; set; }

        public int SortOrder { get; set; }
    }
}
