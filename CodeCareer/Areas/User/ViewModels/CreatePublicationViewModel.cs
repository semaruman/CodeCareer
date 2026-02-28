using CodeCareer.Areas.User.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace CodeCareer.Areas.User.ViewModels
{
    public class CreatePublicationViewModel
    {
        [BindNever]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Введите описание")]
        [MaxLength(1000, ErrorMessage = "Максимальная длина - 1000 символов")]
        public string Content { get; set; }
    }
}
