using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Введите Имя и фамилию")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Введите почту")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(8, ErrorMessage = "Минимум 8 символов")]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;
    }
}
