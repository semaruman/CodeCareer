using System.ComponentModel.DataAnnotations;

namespace CodeCareer.Areas.User.Models
{
    public class TagModel
    {
        [Required]
        public string Name {  get; set; }
    }
}
