namespace CodeCareer.Areas.User.ViewModels
{
    public class FindUserViewModel
    {
        public string CurrentUserEmail {  get; set; }

        public string FindUserName {  get; set; }

        public List<string> SkillTagNames { get; set; }
    }
}
