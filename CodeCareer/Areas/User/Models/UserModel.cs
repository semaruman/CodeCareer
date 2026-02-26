using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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

        [JsonInclude]
        public List<string> SubscribersEmails = new List<string>();

        public int Subscriptions { get; set; } = 0;

        [JsonInclude]
        public HashSet<string> SubscriptionsEmails = new HashSet<string>();

        public string Status { get; set; } = "Начинающий";

        public HashSet<TagModel> Tags { get; set; } = new HashSet<TagModel>();

        public bool ShowSubscriptions { get; set; } = true;

        public void CreateUser(UserViewModel user)
        {
            FullName = user.FullName;
            Email = user.Email;
            Password = user.Password;
        }
    }
}
