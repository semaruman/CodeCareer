using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.User.Controllers
{
    [Area("User")]
    public class OlympTasksSolvingController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SolveTask(string taskName)
        {
            ViewBag.TaskName = taskName;
            return View();
        }

        public IActionResult LinearSearchTasks()
        {
            return View();
        }

        public IActionResult ShowTasksByType(string type)
        {
            ViewBag.Type = type;
            return View();
        }
    }
}
