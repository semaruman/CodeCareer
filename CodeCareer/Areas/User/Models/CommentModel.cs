using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.Models
{
    public class CommentModel
    {
        [BindNever]
        public int Id { get; set; }

        [BindNever]
        public int PublicationId { get; set; }

        [BindNever]
        public UserModel User { get; set; }

        [Required(ErrorMessage = "Введите текст комментария")]
        [MaxLength(1000, ErrorMessage = "Максимальная длина — 1000 символов")]
        public string Content { get; set; } = string.Empty;

        [BindNever]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
