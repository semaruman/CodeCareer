using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.Models
{
    public class UserModel
    {
        [BindNever]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите Имя и фамилию")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Введите почту")]
        public string Email {  get; set; }

        [BindNever]
        public string BirthDate {  get; set; }

        [BindNever]
        public string Info {  get; set; }
    }
}
