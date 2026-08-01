using System.ComponentModel.DataAnnotations;
using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.ViewModels
{
    public class PublicationDetailsViewModel
    {
        public PublicationModel Publication { get; set; }

        public List<CommentModel> Comments { get; set; } = new();

        [Required(ErrorMessage = "Введите текст комментария")]
        [MaxLength(1000, ErrorMessage = "Максимальная длина — 1000 символов")]
        public string NewCommentContent { get; set; } = string.Empty;
    }
}
