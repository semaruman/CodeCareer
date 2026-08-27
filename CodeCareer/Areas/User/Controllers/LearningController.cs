using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using CodeCareer.Infrastructure;
using CodeCareer.Judge;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CodeCareer.Areas.User.Controllers;

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
    private readonly ISubmissionService _submissionService;
    private readonly ICodeJudge _codeJudge;
    private readonly IAchievementService _achievementService;
    private readonly IMarkdownSanitizer _markdownSanitizer;
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
        ISubmissionService submissionService,
        ICodeJudge codeJudge,
        IAchievementService achievementService,
        IMarkdownSanitizer markdownSanitizer,
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
        _submissionService = submissionService;
        _codeJudge = codeJudge;
        _achievementService = achievementService;
        _markdownSanitizer = markdownSanitizer;
        _configuration = configuration;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        ViewBag.Courses = _courseService.GetPublishedCourses();
        var email = _currentUserService.CurrentUser.Email;
        if (!string.IsNullOrEmpty(email))
        {
            ViewBag.UserEmail = email;
            ViewBag.ProgressService = _progressService;
            ViewBag.TaskService = _taskService;
            ViewBag.NoteService = _noteService;
        }
        return View(_sectionService.GetSectionsWithTopics(onlyPublishedTopics: true));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Topic(string slug)
    {
        var topic = _topicService.GetBySlug(slug);
        if (topic == null) return NotFound();
        ViewBag.Notes = _noteService.GetByTopicId(topic.Id);
        ViewBag.Tasks = _taskService.GetByTopicId(topic.Id);
        var email = _currentUserService.CurrentUser.Email;
        if (!string.IsNullOrEmpty(email))
        {
            ViewBag.Completion = _progressService.GetTopicCompletionPercent(email, topic.Id,
                ViewBag.Notes is List<NoteModel> notes ? notes.Count : 0,
                ViewBag.Tasks is List<TaskModel> tasks ? tasks.Count : 0);
        }
        return View(topic);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Note(int id)
    {
        var note = _noteService.GetById(id);
        if (note == null) return NotFound();
        var html = Markdown.ToHtml(note.BodyMarkdown ?? string.Empty, MarkdownPipeline);
        ViewBag.HtmlBody = _markdownSanitizer.SanitizeHtml(html);
        ViewBag.AiChatBaseUrl = _configuration["AiChat:BaseUrl"] ?? "http://localhost:7300";
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
    [Authorize]
    public IActionResult MarkNoteRead(int noteId, int topicId)
    {
        _progressService.MarkNoteRead(_currentUserService.CurrentUser.Email, topicId, noteId);
        return RedirectToAction(nameof(Note), new { id = noteId });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Tasks(string slug)
    {
        var topic = _topicService.GetBySlug(slug);
        if (topic == null) return NotFound();
        ViewBag.Topic = topic;
        var email = _currentUserService.CurrentUser.Email;
        if (!string.IsNullOrEmpty(email))
        {
            ViewBag.SolvedIds = _progressService.GetTaskProgress(email)
                .Where(p => p.Status == "solved").Select(p => p.TaskId).ToHashSet();
        }
        return View(_taskService.GetByTopicId(topic.Id));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Solve(string taskName)
    {
        var task = _taskService.GetByName(taskName);
        if (task == null) return NotFound();
        ViewBag.Topic = task.TopicId.HasValue ? _topicService.GetById(task.TopicId.Value) : null;
        ViewBag.Task = task;
        var user = _currentUserService.CurrentUser;
        ViewBag.IsSolved = user.Id > 0 && _progressService.IsTaskSolved(user.Email, task.Id);
        ViewBag.Submissions = user.Id > 0 ? _submissionService.GetByUserAndTask(user.Id, task.Id) : new List<SubmissionModel>();
        ViewBag.JudgeResult = TempData["JudgeResult"];
        return View(task);
    }

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("submit-code")]
    public async Task<IActionResult> SubmitSolution(int taskId, string taskName, string language, string sourceCode, CancellationToken cancellationToken)
    {
        var user = _currentUserService.CurrentUser;
        if (user.Id <= 0)
        {
            return Challenge();
        }

        if (string.IsNullOrWhiteSpace(sourceCode) || sourceCode.Length > 65536)
        {
            TempData["JudgeResult"] = "Код не может быть пустым и не должен превышать 64 KB.";
            return RedirectToAction(nameof(Solve), new { taskName });
        }

        var result = await _codeJudge.ExecuteAsync(new CodeSubmission
        {
            TaskId = taskId,
            UserId = user.Id,
            Language = language ?? "csharp",
            SourceCode = sourceCode,
        }, cancellationToken);

        _submissionService.Save(new SubmissionModel
        {
            UserId = user.Id,
            TaskId = taskId,
            Language = language ?? "csharp",
            SourceCode = sourceCode,
            Status = result.Status.ToString(),
            Score = result.Score,
            ExecutionTime = result.ExecutionTime,
            MemoryUsed = result.MemoryUsed,
        });

        if (result.Status == SubmissionStatus.Accepted)
        {
            _progressService.MarkTaskSolved(user.Email, taskId);
            _achievementService.TryGrant(user.Id, AchievementKeys.FirstTaskSolved);
            var solvedCount = _progressService.CountSolvedTasks(user.Email);
            if (solvedCount >= 10)
            {
                _achievementService.TryGrant(user.Id, AchievementKeys.TenTasksSolved);
            }
        }

        TempData["JudgeResult"] = FormatJudgeResult(result);
        return RedirectToAction(nameof(Solve), new { taskName });
    }

    [HttpGet]
    [Authorize]
    public IActionResult Submissions()
    {
        var user = _currentUserService.CurrentUser;
        return View(_submissionService.GetByUserId(user.Id));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Search(string? q)
    {
        var query = (q ?? string.Empty).Trim();
        if (query.Length > 200) query = query[..200];
        ViewBag.Query = query;
        ViewBag.Notes = _noteService.Search(query);
        ViewBag.Tasks = _taskService.Search(query);
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Course(int id)
    {
        var course = _courseService.GetById(id);
        if (course == null) return NotFound();
        return View(course);
    }

    [HttpPost]
    [Authorize]
    [EnableRateLimiting("ai-chat")]
    public IActionResult SaveChatMessage(int noteId, string role, string content)
    {
        var email = _currentUserService.CurrentUser.Email;
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(content))
        {
            return BadRequest();
        }

        if (content.Length > 4000)
        {
            content = content[..4000];
        }

        _chatHistoryService.AddMessage(email, noteId, role, content);
        return Ok();
    }

    private static string FormatJudgeResult(JudgeResult result)
    {
        var visible = result.TestResults.Where(t => !t.IsHidden).ToList();
        var details = visible.Count == 0
            ? string.Empty
            : " | " + string.Join("; ", visible.Select(t => $"#{t.Index + 1}: {t.Status}"));
        return $"{result.Status} (score {result.Score}%){details}. {result.Message}".Trim();
    }
}
