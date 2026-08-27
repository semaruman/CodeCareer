using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using CodeCareer.Areas.User.ViewModels;
using CodeCareer.Infrastructure;
using CodeCareer.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CodeCareer.Areas.User.Controllers;

[Area("User")]
public class HomeController : Controller
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;
    private readonly IPublicationService _publicationService;
    private readonly ITagService _tagService;
    private readonly ICommentService _commentService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;
    private readonly IAchievementService _achievementService;
    private readonly IFileStorage _fileStorage;

    private UserModel currentUser => _currentUserService.CurrentUser;

    public HomeController(
        IUserService userService,
        IPublicationService publicationService,
        ITagService tagService,
        ICurrentUserService currentUserService,
        ICommentService commentService,
        IAuthService authService,
        INotificationService notificationService,
        IAchievementService achievementService,
        IFileStorage fileStorage)
    {
        _userService = userService;
        _publicationService = publicationService;
        _tagService = tagService;
        _currentUserService = currentUserService;
        _commentService = commentService;
        _authService = authService;
        _notificationService = notificationService;
        _achievementService = achievementService;
        _fileStorage = fileStorage;
    }

    [HttpGet]
    [Route("")]
    [Route("{action}")]
    [AllowAnonymous]
    public ActionResult Index() => View(currentUser);

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Authorizate() => View(new UserViewModel());

    [HttpPost]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Authorizate(UserViewModel user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var result = await _authService.SignInAsync(HttpContext, user.Email, user.Password ?? string.Empty);
        if (!result.Success)
        {
            var existing = _userService.GetUserByEmail(user.Email);
            if (existing == null)
            {
                var registerResult = await _authService.RegisterAsync(HttpContext, new UserModel
                {
                    FullName = user.FullName,
                    Email = user.Email,
                }, user.Password ?? string.Empty);

                if (registerResult.Success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, registerResult.ErrorMessage ?? "Ошибка регистрации");
                return View(user);
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Ошибка входа");
            return View(user);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> LogoutUser()
    {
        await _authService.SignOutAsync(HttpContext);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize]
    public IActionResult Profile() => View(currentUser);

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AlienProfile(string userEmail)
    {
        var model = new AlienProfileViewModel
        {
            CurrentUserEmail = currentUser.Email,
            AlienUserEmail = userEmail,
        };
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("write-content")]
    public IActionResult AlienProfile(AlienProfileViewModel viewModel)
    {
        var alienUser = _userService.GetUserByEmail(viewModel.AlienUserEmail);
        if (alienUser == null)
        {
            return RedirectToAction(nameof(AlienProfile), new { userEmail = viewModel.AlienUserEmail });
        }

        if (viewModel.WantsToSubscribe)
        {
            if (_userService.Subscribe(currentUser.Id, alienUser.Id))
            {
                _notificationService.Add(alienUser.Id, NotificationTypes.NewFollower,
                    $"{currentUser.FullName} подписался на вас");
                _achievementService.TryGrant(alienUser.Id, AchievementKeys.FirstFollower);
            }
        }
        else
        {
            _userService.Unsubscribe(currentUser.Id, alienUser.Id);
        }

        return RedirectToAction(nameof(AlienProfile), new { userEmail = viewModel.AlienUserEmail });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ShowSubscribes(string userEmail)
    {
        var user = _userService.GetUserByEmail(userEmail);
        return View(user);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ShowSubscriptions(string userEmail)
    {
        var user = _userService.GetUserByEmail(userEmail);
        return View(user);
    }

    [HttpGet]
    [Authorize]
    public IActionResult EditProfile() => View(currentUser);

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("write-content")]
    public async Task<IActionResult> EditProfile(UserModel user, IFormFile? avatar)
    {
        if (string.IsNullOrWhiteSpace(user.Info) || user.Info.Length > 300)
        {
            ModelState.AddModelError(nameof(UserModel.Info), "Информация профиля обязательна (до 300 символов)");
            return View(user);
        }

        currentUser.Info = user.Info;
        currentUser.ShowSubscriptions = user.ShowSubscriptions;

        if (avatar != null)
        {
            var path = await _fileStorage.SaveAvatarAsync(currentUser.Id, avatar);
            if (path != null)
            {
                currentUser.AvatarPath = path;
            }
        }

        _userService.UpdateUserModel(currentUser);
        return RedirectToAction(nameof(EditProfileSuccess));
    }

    [Authorize]
    public IActionResult EditProfileSuccess() => View();

    [HttpGet]
    [Authorize]
    public IActionResult CreatePublication() => View(new CreatePublicationViewModel());

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("write-content")]
    public IActionResult CreatePublication(CreatePublicationViewModel viewModel, List<string>? tagNames)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(viewModel.Content))
        {
            return View(viewModel);
        }

        var publication = new PublicationModel
        {
            Content = viewModel.Content.Trim(),
            UserId = currentUser.Id,
            User = currentUser,
            Tags = _tagService.GetTagModels().Where(t => (tagNames ?? new()).Contains(t.Name)).ToHashSet(),
        };

        currentUser.Rating += Constants.PlUS_RATING_FOR_POST;
        _publicationService.AddPublicationModel(publication);
        _userService.UpdateUserModel(currentUser);
        _achievementService.TryGrant(currentUser.Id, AchievementKeys.FirstPost);

        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    [Authorize]
    public IActionResult EditPublication(int id)
    {
        var publication = _publicationService.GetById(id);
        if (publication == null || !_publicationService.IsOwner(id, currentUser.Id))
        {
            return Forbid();
        }

        return View(new CreatePublicationViewModel { Content = publication.Content, PublicationId = id });
    }

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("write-content")]
    public IActionResult EditPublication(CreatePublicationViewModel viewModel, List<string>? tagNames)
    {
        if (!viewModel.PublicationId.HasValue || string.IsNullOrWhiteSpace(viewModel.Content))
        {
            return View(viewModel);
        }

        var publication = _publicationService.GetById(viewModel.PublicationId.Value);
        if (publication == null || !_publicationService.IsOwner(publication.Id, currentUser.Id))
        {
            return Forbid();
        }

        publication.Content = viewModel.Content.Trim();
        publication.Tags = _tagService.GetTagModels().Where(t => (tagNames ?? new()).Contains(t.Name)).ToHashSet();
        _publicationService.UpdatePublicationModel(publication);
        return RedirectToAction(nameof(Publication), new { id = publication.Id });
    }

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("write-content")]
    public IActionResult DeletePublication(int id)
    {
        if (!_publicationService.IsOwner(id, currentUser.Id))
        {
            return Forbid();
        }

        _publicationService.RemovePublicationModel(id);
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Publication(int id)
    {
        var publication = _publicationService.GetById(id);
        if (publication == null)
        {
            return NotFound();
        }

        return View(new PublicationDetailsViewModel
        {
            Publication = publication,
            Comments = _commentService.GetByPublicationId(id),
        });
    }

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("write-content")]
    public IActionResult AddComment(int publicationId, string newCommentContent)
    {
        var publication = _publicationService.GetById(publicationId);
        if (publication == null)
        {
            return NotFound();
        }

        var content = (newCommentContent ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(content) || content.Length > 1000)
        {
            var viewModel = new PublicationDetailsViewModel
            {
                Publication = publication,
                Comments = _commentService.GetByPublicationId(publicationId),
                NewCommentContent = newCommentContent ?? string.Empty,
            };
            ModelState.AddModelError(nameof(PublicationDetailsViewModel.NewCommentContent),
                string.IsNullOrEmpty(content) ? "Введите текст комментария" : "Максимальная длина — 1000 символов");
            return View(nameof(Publication), viewModel);
        }

        _commentService.Add(new CommentModel
        {
            PublicationId = publicationId,
            UserId = currentUser.Id,
            User = currentUser,
            Content = content,
        });

        if (publication.UserId != currentUser.Id)
        {
            _notificationService.Add(publication.UserId, NotificationTypes.CommentOnPost,
                $"{currentUser.FullName} прокомментировал вашу публикацию");
        }

        return RedirectToAction(nameof(Publication), new { id = publicationId });
    }

    [HttpPost]
    [Authorize]
    public IActionResult DeleteComment(int commentId, int publicationId)
    {
        if (!_commentService.Delete(commentId, currentUser.Id))
        {
            return Forbid();
        }

        return RedirectToAction(nameof(Publication), new { id = publicationId });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult PublicationFeed() => View(new PublicationFeedViewModel(currentUser.Email));

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("write-content")]
    public IActionResult PublicationFeed(PublicationFeedViewModel viewModel, List<string>? selectedTags)
    {
        var publicationUser = _userService.GetUserByEmail(viewModel.PublicationUserEmail ?? string.Empty);
        if (publicationUser != null)
        {
            if (viewModel.WantsToSubscribe)
            {
                _userService.Subscribe(currentUser.Id, publicationUser.Id);
            }
            else
            {
                _userService.Unsubscribe(currentUser.Id, publicationUser.Id);
            }
        }

        viewModel = new PublicationFeedViewModel
        {
            CurrentUserEmail = currentUser.Email,
            TagNames = selectedTags ?? new List<string>(),
            SortType = viewModel.SortType,
        };

        return View(viewModel);
    }

    [AllowAnonymous]
    public IActionResult Top100Users()
    {
        var users = _userService.GetUserModels().OrderByDescending(u => u.Rating).Take(100).ToList();
        return View(users);
    }

    [HttpGet]
    [Authorize]
    public IActionResult AddSkillTags() => View(new List<string>());

    [HttpPost]
    [Authorize]
    public IActionResult AddSkillTags(List<string>? skillTagNames)
    {
        if (skillTagNames != null)
        {
            currentUser.SkillTags = _tagService.GetTagModels().Where(t => skillTagNames.Contains(t.Name)).ToHashSet();
            _userService.UpdateUserModel(currentUser);
        }
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Community() => View();

    [HttpGet]
    [AllowAnonymous]
    public IActionResult FindUser() => View(new FindUserViewModel());

    [HttpPost]
    [AllowAnonymous]
    public IActionResult FindUser(FindUserViewModel viewModel, List<string>? skillTagNames)
    {
        if ((viewModel.FindUserName?.Length ?? 0) > 200)
        {
            viewModel.FindUserName = viewModel.FindUserName![..200];
        }

        viewModel.SkillTagNames = skillTagNames ?? new List<string>();
        return View(viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult FindPublication() => View(new FindPublicationViewModel());

    [HttpPost]
    [AllowAnonymous]
    public IActionResult FindPublication(FindPublicationViewModel viewModel, List<string>? tagNames)
    {
        if ((viewModel.FindPublicationText?.Length ?? 0) > 200)
        {
            viewModel.FindPublicationText = viewModel.FindPublicationText![..200];
        }

        viewModel.TagNames = tagNames ?? new List<string>();
        return View(viewModel);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Notifications()
    {
        ViewBag.Notifications = _notificationService.GetAll(currentUser.Id);
        ViewBag.UnreadCount = _notificationService.CountUnread(currentUser.Id);
        return View();
    }

    [HttpPost]
    [Authorize]
    public IActionResult MarkNotificationRead(int notificationId)
    {
        _notificationService.MarkRead(notificationId, currentUser.Id);
        return RedirectToAction(nameof(Notifications));
    }
}
