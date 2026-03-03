using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.User.Controllers
{
    [Area("User")]
    public class TasksSolvingController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OlympSolving()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FrontendSolving()
        {
            return View();
        }
    }
}
