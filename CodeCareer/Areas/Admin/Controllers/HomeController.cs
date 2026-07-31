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
        private readonly ISectionService _sectionService;
        private readonly ITopicService _topicService;
        private readonly INoteService _noteService;
        private readonly ICourseService _courseService;

        public HomeController(
            ITagService tagService,
            ITaskService taskService,
            ISectionService sectionService,
            ITopicService topicService,
            INoteService noteService,
            ICourseService courseService)
        {
            _tagService = tagService;
            _taskService = taskService;
            _sectionService = sectionService;
            _topicService = topicService;
            _noteService = noteService;
            _courseService = courseService;
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

            ViewBag.WrongPassword = "Неверный пароль!";
            return View(admin);
        }

        public IActionResult AuthorizationSuccess() => View();

        public IActionResult LogoutAdmin()
        {
            admin = new AdminModel();
            return View();
        }

        [HttpGet]
        public IActionResult AddTags()
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            return View(new TagModel());
        }

        [HttpPost]
        public IActionResult AddTags(TagModel tag)
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            if (tag.Name != string.Empty)
            {
                _tagService.AddTagModel(tag);
                return View(new TagModel());
            }
            return View(tag);
        }

        [HttpGet]
        public IActionResult AddTasks()
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
            return View(new TaskModel());
        }

        [HttpPost]
        public IActionResult AddTasks(TaskModel task)
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
            if (!string.IsNullOrEmpty(task.Name))
            {
                _taskService.AddTaskModel(task);
                return RedirectToAction("AddTasksSuccess");
            }
            return View(task);
        }

        [HttpGet]
        public IActionResult AddTasksSuccess() => View();

        [HttpGet]
        public IActionResult ManageTopics()
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            ViewBag.Sections = _sectionService.GetSectionsWithTopics(onlyPublishedTopics: false);
            return View(new TopicModel());
        }

        [HttpPost]
        public IActionResult ManageTopics(TopicModel topic)
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            if (!string.IsNullOrWhiteSpace(topic.Title) && !string.IsNullOrWhiteSpace(topic.Slug) && topic.SectionId > 0)
            {
                topic.Slug = topic.Slug.Trim().ToLowerInvariant().Replace(' ', '-');
                _topicService.Add(topic);
                return RedirectToAction("ManageTopics");
            }
            ViewBag.Sections = _sectionService.GetSectionsWithTopics(onlyPublishedTopics: false);
            return View(topic);
        }

        [HttpGet]
        public IActionResult AddSection()
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            return View(new SectionModel());
        }

        [HttpPost]
        public IActionResult AddSection(SectionModel section)
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            if (!string.IsNullOrWhiteSpace(section.Title))
            {
                _sectionService.Add(section);
                return RedirectToAction("ManageTopics");
            }
            return View(section);
        }

        [HttpGet]
        public IActionResult ManageNotes()
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
            return View(new NoteModel());
        }

        [HttpPost]
        public IActionResult ManageNotes(NoteModel note)
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
            if (!string.IsNullOrWhiteSpace(note.Title) && !string.IsNullOrWhiteSpace(note.BodyMarkdown) && note.TopicId > 0)
            {
                _noteService.Add(note);
                return RedirectToAction("ManageNotes");
            }
            return View(note);
        }

        [HttpGet]
        public IActionResult ManageCourses()
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            ViewBag.Courses = _courseService.GetPublishedCourses();
            ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
            return View(new CourseModel());
        }

        [HttpPost]
        public IActionResult ManageCourses(CourseModel course)
        {
            if (!admin.IsAuthorizate) return RedirectToAction("Authorization");
            if (!string.IsNullOrWhiteSpace(course.Title))
            {
                _courseService.Add(course);
                return RedirectToAction("ManageCourses");
            }
            ViewBag.Courses = _courseService.GetPublishedCourses();
            ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
            return View(course);
        }
    }
}
