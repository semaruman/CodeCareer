using System.ComponentModel.DataAnnotations;
using CodeCareer.Areas.User.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.Models
{
    public class UserModel
    {
        [BindNever]
        public int Id { get; set; } = 1;

        [BindNever]
        public string FullName { get; set; }

        [BindNever]
        public string Email {  get; set; }

        [BindNever]
        public string Password { get; set; }

        [BindNever]
        public DateTime? BirthDate { get; set; }

        [BindNever]
        public string Info {  get; set; } = string.Empty;

        public void CreateUser(UserViewModel user)
        {
            FullName = user.FullName;
            Email = user.Email;
            Password = user.Password;
        }
    }
}
