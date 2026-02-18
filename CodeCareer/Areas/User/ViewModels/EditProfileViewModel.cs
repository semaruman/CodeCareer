using System.ComponentModel.DataAnnotations;

namespace CodeCareer.Areas.User.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(300, ErrorMessage = "текст не должен превышать 300 символов")]
        public string Info { get; set; } = string.Empty;
    }
}
