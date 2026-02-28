using CodeCareer.Areas.User.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.ViewModels
{
    public class PublicationFeedViewModel
    {
        public string CurrentUserEmail {  get; set; }
        public string PublicationUserEmail { get; set; }

        public bool WantsToSubscribe { get; set; } = false;


        //public bool WantsToBoostRating { get; set; } = false;

        public List<string> TagNames { get; set; } = new List<string>();

        public PublicationFeedViewModel()
        {

        }
        public PublicationFeedViewModel(string currentUserEmail)
        {
            CurrentUserEmail = currentUserEmail;
        }
    }
}
