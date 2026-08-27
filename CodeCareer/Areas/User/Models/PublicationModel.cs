using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.Models;

public class PublicationModel
{
    [BindNever]
    public int Id { get; set; }

    [BindNever]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    [BindNever]
    public UserModel User { get; set; } = null!;

    [Required(ErrorMessage = "Введите описание")]
    [MaxLength(1000, ErrorMessage = "Максимальная длина - 1000 символов")]
    public string Content { get; set; } = string.Empty;

    [BindNever]
    public HashSet<TagModel> Tags { get; set; } = new();

    public ICollection<PublicationTagModel> PublicationTags { get; set; } = new List<PublicationTagModel>();
    public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();
}
