namespace CodeCareer.Areas.User.ViewModels
{
    public class AlienProfileViewModel
    {
        public string CurrentUserEmail { get; set; }
        public string AlienUserEmail { get; set; }

        public bool WantsToSubscribe { get; set; } = false;
    }
}
