using System.Security.Claims;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Implementations.JsonServices;
using CodeCareer.Areas.User.Services.Interfaces;
using CodeCareer.Areas.User.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {


        private readonly ICurrentUserService _currentUserService;

        private readonly IUserService _userService;
        private readonly IPublicationService _publicationService;
        private readonly ITagService _tagService;

        public UserModel currentUser
        {
            get => _currentUserService.CurrentUser;
            set => _currentUserService.CurrentUser = value;
        }

        public HomeController(IUserService userService, IPublicationService publicationService, ITagService tagService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _publicationService = publicationService;
            _tagService = tagService;
            _currentUserService = currentUserService;
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
        public async Task<IActionResult> Authorizate(UserViewModel user)
        {
            var dbUser = _userService.GetUserByEmail(user.Email);

            if (dbUser != null && user.Password == dbUser.Password)
            {
                // Те же шаги - создаем claims и вызываем SignInAsync
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(claimsPrincipal);

                return RedirectToAction("Index", "Home");
            }

            return View(user);
            
        }
        public IActionResult SuccessAuthorizate()
        {
            return View(currentUser);
        }

        public async Task<IActionResult> LogoutUser()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
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
            AlienProfileViewModel model = new AlienProfileViewModel()
            {
                CurrentUserEmail = currentUser.Email,
                AlienUserEmail = userEmail
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AlienProfile(AlienProfileViewModel viewModel)
        {
            // если пользователь не зарегистрирован
            if (currentUser.FullName == string.Empty)
            {
                // Ничего не делаем
            }
            else
            {
                UserModel alienUser = _userService.GetUserModels().FirstOrDefault(u => u.Email == viewModel.AlienUserEmail);

                if (alienUser == null)
                {
                    Console.WriteLine($"Null. {viewModel.AlienUserEmail}");
                }

                if (viewModel.WantsToSubscribe)
                {
                    alienUser.SubscribersEmails.Add(currentUser.Email);
                    alienUser.Subscribers += 1;
                    currentUser.Subscriptions += 1;
                    currentUser.SubscriptionsEmails.Add(alienUser.Email);

                    alienUser.Rating += Constants.PlUS_RATING_FOR_SUBSCRIBE;
                    currentUser.Rating += Constants.PlUS_RATING_FOR_SUBSCRIPTION;

                    _userService.UpdateUserModel(alienUser);
                    _userService.UpdateUserModel(currentUser);

                }
                else
                {

                    if (alienUser.SubscribersEmails.Remove(currentUser.Email))
                    {
                        alienUser.Subscribers -= 1;
                        currentUser.Subscriptions -= 1;
                        currentUser.SubscriptionsEmails.Remove(alienUser.Email);

                        alienUser.Rating -= Constants.PlUS_RATING_FOR_SUBSCRIBE;
                        currentUser.Rating -= Constants.PlUS_RATING_FOR_SUBSCRIPTION;
                    }

                    _userService.UpdateUserModel(alienUser);
                    _userService.UpdateUserModel(currentUser);

                }

            }

            AlienProfileViewModel model = new AlienProfileViewModel()
            {
                CurrentUserEmail = currentUser.Email,
                AlienUserEmail = viewModel.AlienUserEmail
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ShowSubscribes(string userEmail)
        {
            UserModel user = _userService.GetUserModels().FirstOrDefault(u => u.Email == userEmail);
            return View(user);
        }

        [HttpGet]
        public IActionResult ShowSubscriptions(string userEmail)
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
                currentUser.ShowSubscriptions = user.ShowSubscriptions;
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
            return View(new CreatePublicationViewModel());
        }

        [HttpPost]
        public IActionResult CreatePublication(CreatePublicationViewModel viewModel, List<string> TagNames)
        {

            if (!string.IsNullOrEmpty(viewModel.Content))
            {
                var publication = new PublicationModel { Content = viewModel.Content };

                publication.Tags = _tagService.GetTagModels().Where(t => TagNames.Contains(t.Name)).ToHashSet();

                publication.User = currentUser;
                currentUser.Rating += Constants.PlUS_RATING_FOR_POST;
                _publicationService.AddPublicationModel(publication);

                _userService.UpdateUserModel(currentUser);

                return RedirectToAction("Profile");
            }
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult PublicationFeed()
        {
            return View(new PublicationFeedViewModel(currentUser.Email));
        }

        [HttpPost]
        public IActionResult PublicationFeed(PublicationFeedViewModel viewModel, List<string> SelectedTags)
        {
            // если пользователь не зарегистрирован
            if (viewModel.CurrentUserEmail == null)
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

            viewModel = new PublicationFeedViewModel
            {
                CurrentUserEmail = currentUser.Email,
                TagNames = SelectedTags,
                SortType = viewModel.SortType,
            };

            return View(viewModel);
        }

        public IActionResult Top100Users()
        {
            var users = _userService.GetUserModels().OrderByDescending(u => u.Rating).Take(100).ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult AddSkillTags()
        {
            return View(new List<string>());
        }

        [HttpPost]
        public IActionResult AddSkillTags(List<string> skillTagNames)
        {
            if (skillTagNames != null)
            {
                currentUser.SkillTags = _tagService.GetTagModels().Where(t => skillTagNames.Contains(t.Name)).ToHashSet();
                _userService.UpdateUserModel(currentUser);
            }
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult Community()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FindUser()
        {
            return View(new FindUserViewModel());
        }

        [HttpPost]
        public IActionResult FindUser(FindUserViewModel viewModel, List<string> skillTagNames)
        {
            viewModel.SkillTagNames = skillTagNames;

            return View(viewModel);
        }
    }
}
