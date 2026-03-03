namespace CodeCareer.Areas.User.ViewModels
{
    public class FindPublicationViewModel
    {
        public string FindPublicationText { get; set; } = string.Empty;

        public List<string> TagNames { get; set; } = new List<string>();
    }
}
