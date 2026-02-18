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
        [HttpPost]
        public IActionResult Profile(UserModel user)
        {
            return View(user);
        }
    }
}
