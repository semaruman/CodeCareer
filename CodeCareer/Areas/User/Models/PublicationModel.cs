using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.Models
{
    public class PublicationModel
    {
        [BindNever]
        public int Id { get; set; }

        [BindNever]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [BindNever]
        public UserModel User { get; set; }

        [Required(ErrorMessage = "Введите описание")]
        [MaxLength(1000, ErrorMessage = "Максимальная длина - 1000 символов")]
        public string Content {  get; set; }

    }
}
