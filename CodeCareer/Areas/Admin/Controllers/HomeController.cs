using CodeCareer.Areas.Admin.Models;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{area}/{action}")]
    public class HomeController : Controller
    {
        public static AdminModel admin = new AdminModel();

        private readonly ITagService _tagService;
        private readonly ITaskService _taskService;

        public HomeController(ITagService tagService,ITaskService taskService)
        {
            _tagService = tagService;
            _taskService = taskService;
        }

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

        [HttpGet]
        public IActionResult AddTags()
        {
            if (admin.IsAuthorizate)
            {
                return View(new TagModel());
            }
            else
            {
                return RedirectToAction("Authorization");
            }
        }

        [HttpPost]
        public IActionResult AddTags(TagModel tag)
        {
            if (ModelState.IsValid)
            {
                _tagService.AddTagModel(tag);
                return View(new TagModel());
            }
            else
            {
                return View(tag);
            }
        }


        [HttpGet]
        public IActionResult AddTasks()
        {
            if (admin.IsAuthorizate)
            {
                return View(new TaskModel());
            }
            else
            {
                return RedirectToAction("Authorization");
            }
        }

        [HttpPost]
        public IActionResult AddTasks(TaskModel task)
        {
            if (ModelState.IsValid)
            {
                _taskService.AddTaskModel(task);
                return RedirectToAction("AddTasksSuccess");
            }
            else
            {
                return View(task);
            }
        }

        [HttpGet]
        public IActionResult AddTasksSuccess()
        {
            return View();
        }
    }
}
