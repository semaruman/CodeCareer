using System.ComponentModel.DataAnnotations;

namespace CodeCareer.Areas.User.ViewModels
{
    public class FindUserViewModel
    {
        public string FindUserName {  get; set; } = string.Empty;

        public List<string> SkillTagNames { get; set; } = new List<string>();
    }
}
