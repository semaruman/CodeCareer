using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        public static UserModel currentUser { get; set; } = new UserModel();

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
                UserModelDb db = new UserModelDb();

                var dbUser = db.GetUserModels().Where(u => u.Email == user.Email).FirstOrDefault();

                

                // Если пользователь не найден в БД
                if (dbUser == null)
                {
                    currentUser.CreateUser(user);
                    db.AddUserModel(currentUser);
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
            UserModelDb db = new UserModelDb();
            UserModel user = db.GetUserModels().FirstOrDefault(u => u.Email == userEmail);
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
                UserModelDb db = new UserModelDb();
                db.RemoveUserModel(currentUser);
                currentUser.Info = user.Info;
                db.AddUserModel(currentUser);

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
                PublicationModelDb db = new PublicationModelDb();
                UserModelDb uDb = new UserModelDb();
                publication.User = currentUser;
                currentUser.Rating += Constants.PlUS_RATING_FOR_POST;
                db.AddPublicationModel(publication);

                uDb.RemoveUserModel(currentUser);
                uDb.AddUserModel(currentUser);

                return RedirectToAction("Profile");
            }
            return View(publication);
        }

        public IActionResult PublicationFeed()
        {
            return View();
        }

        public IActionResult Top100Users()
        {
            UserModelDb db = new UserModelDb();
            var users = db.GetUserModels().OrderByDescending(u => u.Rating).Take(100).ToList();
            return View(users);
        }
    }
}
