using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Implementations.JsonServices;
using CodeCareer.Areas.User.Services.Interfaces;
using CodeCareer.Areas.User.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        public static UserModel currentUser { get; set; } = new UserModel();

        private readonly IUserService _userService;
        private readonly IPublicationService _publicationService;

        public HomeController(IUserService userService, IPublicationService publicationService)
        {
            _userService = userService;
            _publicationService = publicationService;
        }

        [HttpGet]
        [Route("")]
        [Route("{action}")]
        public ActionResult Index()
        {
            return View(currentUser);
        }

        [HttpGet]
        public IActionResult Authorizate()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        public IActionResult Authorizate(UserViewModel user)
        {
            if (ModelState.IsValid)
            {

                var dbUser = _userService.GetUserModels().Where(u => u.Email == user.Email).FirstOrDefault();

                

                // Если пользователь не найден в БД
                if (dbUser == null)
                {
                    currentUser.CreateUser(user);
                    _userService.AddUserModel(currentUser);
                    return RedirectToAction("SuccessAuthorizate");
                }
                else
                {
                    if (dbUser.Password == user.Password)
                    {
                        currentUser = dbUser;
                        return RedirectToAction("SuccessAuthorizate");
                    }
                    else
                    {
                        ViewData["isWrongAnswer"] = "true";
                        return View(user);
                    }
                }
            }
            else
            {
                return View(user);
            }
        }
        public IActionResult SuccessAuthorizate()
        {
            return View(currentUser);
        }

        public IActionResult LogoutUser()
        {
            currentUser = new UserModel();
            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            if (currentUser.FullName != null)
            {
                return View(currentUser);
            }
            else
            {
                return RedirectToAction("Authorizate");
            }
        }

        [HttpGet]
        public IActionResult AlienProfile(string userEmail)
        {
            UserModel user = _userService.GetUserModels().FirstOrDefault(u => u.Email == userEmail);
            return View(user);
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            return View(currentUser);
        }

        [HttpPost]
        public IActionResult EditProfile(UserModel user)
        {
            if (user.Info != string.Empty)
            {
                _userService.RemoveUserModel(currentUser);
                currentUser.Info = user.Info;
                _userService.AddUserModel(currentUser);

                return RedirectToAction("EditProfileSuccess");
            }
            return View(user);
        }

        public IActionResult EditProfileSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreatePublication()
        {
            return View(new PublicationModel());
        }

        [HttpPost]
        public IActionResult CreatePublication(PublicationModel publication)
        {
            if (!string.IsNullOrEmpty(publication.Content))
            {
                publication.User = currentUser;
                currentUser.Rating += Constants.PlUS_RATING_FOR_POST;
                _publicationService.AddPublicationModel(publication);

                _userService.UpdateUserModel(currentUser);

                return RedirectToAction("Profile");
            }
            return View(publication);
        }

        [HttpGet]
        public IActionResult PublicationFeed()
        {
            return View(new PublicationFeedViewModel(currentUser.Email));
        }

        [HttpPost]
        public IActionResult PublicationFeed(PublicationFeedViewModel viewModel)
        {
            // если пользователь не зарегистрирован
            if (currentUser.FullName == string.Empty)
            {
                // Ничего не делаем
            }
            else
            {
                UserModel publicationUser = _userService.GetUserModels().FirstOrDefault(u => u.Email == viewModel.PublicationUserEmail);
                if (viewModel.WantsToSubscribe)
                {
                    publicationUser.SubscribersEmails.Add(currentUser.Email);
                    publicationUser.Subscribers += 1;
                    currentUser.Subscriptions += 1;
                    currentUser.SubscriptionsEmails.Add(publicationUser.Email);

                    publicationUser.Rating += Constants.PlUS_RATING_FOR_SUBSCRIBE;
                    currentUser.Rating += Constants.PlUS_RATING_FOR_SUBSCRIPTION;

                    _userService.UpdateUserModel(publicationUser);
                    _userService.UpdateUserModel(currentUser);
                    
                }
                else
                {
                    
                    if (publicationUser.SubscribersEmails.Remove(currentUser.Email))
                    {
                        publicationUser.Subscribers -= 1;
                        currentUser.Subscriptions -= 1;
                        currentUser.SubscriptionsEmails.Remove(publicationUser.Email);

                        publicationUser.Rating -= Constants.PlUS_RATING_FOR_SUBSCRIBE;
                        currentUser.Rating -= Constants.PlUS_RATING_FOR_SUBSCRIPTION;
                    }

                    _userService.UpdateUserModel(publicationUser);
                    _userService.UpdateUserModel(currentUser);

                }
                
            }

            return View(new PublicationFeedViewModel(currentUser.Email));
        }

        public IActionResult Top100Users()
        {
            var users = _userService.GetUserModels().OrderByDescending(u => u.Rating).Take(100).ToList();
            return View(users);
        }
    }
}
