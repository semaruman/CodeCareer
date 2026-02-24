using CodeCareer.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{area}")]
    [Route("{area}/{action}")]
    public class HomeController : Controller
    {
        public static AdminModel admin = new AdminModel();

        [Route("")]
        public IActionResult Index()
        {
            if (!admin.IsAuthorizate)
            {
                return RedirectToAction("Authorization");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Authorization()
        {
            ViewBag.WrongPassword = string.Empty;
            return View(admin);
        }

        [HttpPost]
        public IActionResult Authorization(AdminModel adminP)
        {
            if (adminP.Password == AdminModel.PASSWORD)
            {
                admin = adminP;
                admin.IsAuthorizate = true;
                return RedirectToAction("AuthorizationSuccess");
            }
            else
            {
                ViewBag.WrongPassword = "Неверный пароль!";
                return View(admin);
            }
        }

        public IActionResult AuthorizationSuccess()
        {
            return View();
        }

        public IActionResult LogoutAdmin()
        {
            admin = new AdminModel();
            return View();
        }
    }
}
