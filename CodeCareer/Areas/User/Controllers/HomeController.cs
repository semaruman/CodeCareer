using CodeCareer.Areas.User.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.User.Controllers
{
    [Area("User")]
    [Route("")]
    [Route("{controller}")]
    [Route("{area}/{controller}")]
    public class HomeController : Controller
    {
        public UserModel currentUser { get; set; } = null;
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Authorizate()
        {
            return View(currentUser);
        }

        [HttpPost]
        public IActionResult Authorizate(UserModel user)
        {
            if (ModelState.IsValid)
            {
                currentUser = user;
                return RedirectToAction("SuccessAuthorizate");
            }
            else
            {
                return View(currentUser);
            }
        }

        public IActionResult SuccessAuthorizate()
        {
            return View(currentUser);
        }
    }
}
