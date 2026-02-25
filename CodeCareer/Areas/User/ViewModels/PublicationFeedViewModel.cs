using CodeCareer.Areas.User.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.ViewModels
{
    public class PublicationFeedViewModel
    {
        public UserModel CurrentUser { get; set; }

        public UserModel PublicationUser { get; set; }

        public bool WantsToSubscribe { get; set; } = false;

        public bool WantsToBoostRating { get; set; } = false;

        public PublicationFeedViewModel()
        {

        }

        public PublicationFeedViewModel(UserModel currentUser)
        {
            CurrentUser = currentUser;
        }
        
    }
}
