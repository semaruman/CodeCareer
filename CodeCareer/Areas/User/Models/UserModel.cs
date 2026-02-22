using System.ComponentModel.DataAnnotations;
using CodeCareer.Areas.User.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.Models
{
    public class UserModel
    {
        public int Id { get; set; } = 1;

        public string FullName { get; set; }

        public string Email {  get; set; }

        public string Password { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(300, ErrorMessage = "текст не должен превышать 300 символов")]
        public string Info {  get; set; } = string.Empty;

        public int Rating { get; set; } = 0;

        public int Subscribers { get; set; } = 0;

        public string Status { get; set; } = "Начинающий";

        public List<TagModel> Tags { get; set; } = new List<TagModel>();

        public void CreateUser(UserViewModel user)
        {
            FullName = user.FullName;
            Email = user.Email;
            Password = user.Password;
        }
    }
}
