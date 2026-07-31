using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Markdig;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.User.Controllers
{
    [Area("User")]
    public class LearningController : Controller
    {
        private readonly ISectionService _sectionService;
        private readonly ITopicService _topicService;
        private readonly INoteService _noteService;
        private readonly ITaskService _taskService;
        private readonly IProgressService _progressService;
        private readonly ICourseService _courseService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IChatHistoryService _chatHistoryService;
        private readonly IConfiguration _configuration;

        private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        public LearningController(
            ISectionService sectionService,
            ITopicService topicService,
            INoteService noteService,
            ITaskService taskService,
            IProgressService progressService,
            ICourseService courseService,
            ICurrentUserService currentUserService,
            IChatHistoryService chatHistoryService,
            IConfiguration configuration)
        {
            _sectionService = sectionService;
            _topicService = topicService;
            _noteService = noteService;
            _taskService = taskService;
            _progressService = progressService;
            _courseService = courseService;
            _currentUserService = currentUserService;
            _chatHistoryService = chatHistoryService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sections = _sectionService.GetSectionsWithTopics(onlyPublishedTopics: true);
            ViewBag.Courses = _courseService.GetPublishedCourses();
            var email = _currentUserService.CurrentUser.Email;
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.UserEmail = email;
                ViewBag.ProgressService = _progressService;
                ViewBag.TaskService = _taskService;
                ViewBag.NoteService = _noteService;
            }
            return View(sections);
        }

        [HttpGet]
        public IActionResult Topic(string slug)
        {
            var topic = _topicService.GetBySlug(slug);
            if (topic == null) return NotFound();
            var notes = _noteService.GetByTopicId(topic.Id);
            var tasks = _taskService.GetByTopicId(topic.Id);
            ViewBag.Notes = notes;
            ViewBag.Tasks = tasks;
            var email = _currentUserService.CurrentUser.Email;
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.Completion = _progressService.GetTopicCompletionPercent(email, topic.Id, notes.Count, tasks.Count);
            }
            return View(topic);
        }

        [HttpGet]
        public IActionResult Note(int id)
        {
            var note = _noteService.GetById(id);
            if (note == null) return NotFound();
            ViewBag.HtmlBody = Markdown.ToHtml(note.BodyMarkdown ?? string.Empty, MarkdownPipeline);
            ViewBag.AiChatBaseUrl = _configuration["AiChat:BaseUrl"] ?? "https://localhost:7301";
            ViewBag.AiChatPath = _configuration["AiChat:ChatPath"] ?? "/api/chat";
            var email = _currentUserService.CurrentUser.Email;
            ViewBag.IsRead = !string.IsNullOrEmpty(email) && _progressService.IsNoteRead(email, note.Id);
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.ChatHistory = _chatHistoryService.GetByNote(email, note.Id);
            }
            return View(note);
        }

        [HttpPost]
        public IActionResult MarkNoteRead(int noteId, int topicId)
        {
            var email = _currentUserService.CurrentUser.Email;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Authorizate", "Home");
            }
            _progressService.MarkNoteRead(email, topicId, noteId);
            return RedirectToAction(nameof(Note), new { id = noteId });
        }

        [HttpGet]
        public IActionResult Tasks(string slug)
        {
            var topic = _topicService.GetBySlug(slug);
            if (topic == null) return NotFound();
            var tasks = _taskService.GetByTopicId(topic.Id);
            ViewBag.Topic = topic;
            var email = _currentUserService.CurrentUser.Email;
            if (!string.IsNullOrEmpty(email))
            {
                ViewBag.SolvedIds = _progressService.GetTaskProgress(email)
                    .Where(p => p.Status == "solved")
                    .Select(p => p.TaskId)
                    .ToHashSet();
            }
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Solve(string taskName)
        {
            var task = _taskService.GetByName(taskName);
            if (task == null) return NotFound();
            TopicModel? topic = null;
            if (task.TopicId.HasValue)
            {
                topic = _topicService.GetById(task.TopicId.Value);
            }
            ViewBag.Topic = topic;
            ViewBag.Task = task;
            var email = _currentUserService.CurrentUser.Email;
            ViewBag.IsSolved = !string.IsNullOrEmpty(email) && _progressService.IsTaskSolved(email, task.Id);
            ViewBag.CheckResult = TempData["CheckResult"];
            return View(task);
        }

        [HttpPost]
        public IActionResult MarkTaskSolved(int taskId, string taskName)
        {
            var email = _currentUserService.CurrentUser.Email;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Authorizate", "Home");
            }
            _progressService.MarkTaskSolved(email, taskId);
            return RedirectToAction(nameof(Solve), new { taskName });
        }

        [HttpPost]
        public IActionResult CheckSample(int taskId, int sampleIndex, string actualOutput, string taskName)
        {
            var ok = _taskService.CheckSampleOutput(taskId, sampleIndex, actualOutput);
            TempData["CheckResult"] = ok ? "Верно: вывод совпал с примером." : "Неверно: вывод не совпал с ожидаемым.";
            if (ok)
            {
                var email = _currentUserService.CurrentUser.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    _progressService.MarkTaskSolved(email, taskId);
                }
            }
            return RedirectToAction(nameof(Solve), new { taskName });
        }

        [HttpGet]
        public IActionResult Search(string q)
        {
            ViewBag.Query = q ?? string.Empty;
            ViewBag.Notes = _noteService.Search(q ?? string.Empty);
            ViewBag.Tasks = _taskService.Search(q ?? string.Empty);
            return View();
        }

        [HttpGet]
        public IActionResult Course(int id)
        {
            var course = _courseService.GetById(id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult SaveChatMessage(int noteId, string role, string content)
        {
            var email = _currentUserService.CurrentUser.Email;
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(content))
            {
                _chatHistoryService.AddMessage(email, noteId, role, content);
            }
            return Ok();
        }
    }
}
