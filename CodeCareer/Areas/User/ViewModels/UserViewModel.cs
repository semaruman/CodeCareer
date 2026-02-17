using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Введите Имя и фамилию")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Введите почту")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
    }
}
