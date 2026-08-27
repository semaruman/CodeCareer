using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.Admin.Controllers;

[Area("Admin")]
[Route("{area}/{action}")]
[Authorize(Policy = "AdminOnly")]
public class HomeController : Controller
{
    private readonly ITagService _tagService;
    private readonly ITaskService _taskService;
    private readonly ISectionService _sectionService;
    private readonly ITopicService _topicService;
    private readonly INoteService _noteService;
    private readonly ICourseService _courseService;
    private readonly ICommentService _commentService;
    private readonly IPublicationService _publicationService;

    public HomeController(
        ITagService tagService,
        ITaskService taskService,
        ISectionService sectionService,
        ITopicService topicService,
        INoteService noteService,
        ICourseService courseService,
        ICommentService commentService,
        IPublicationService publicationService)
    {
        _tagService = tagService;
        _taskService = taskService;
        _sectionService = sectionService;
        _topicService = topicService;
        _noteService = noteService;
        _courseService = courseService;
        _commentService = commentService;
        _publicationService = publicationService;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult AddTags() => View(new TagModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTags(TagModel tag)
    {
        if (!string.IsNullOrWhiteSpace(tag.Name))
        {
            _tagService.AddTagModel(tag);
            return View(new TagModel());
        }
        return View(tag);
    }

    [HttpGet]
    public IActionResult AddTasks()
    {
        ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
        return View(new TaskModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTasks(TaskModel task)
    {
        ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
        if (!string.IsNullOrEmpty(task.Name))
        {
            _taskService.AddTaskModel(task);
            return RedirectToAction(nameof(AddTasksSuccess));
        }
        return View(task);
    }

    [HttpGet]
    public IActionResult AddTasksSuccess() => View();

    [HttpGet]
    public IActionResult ManageTopics()
    {
        ViewBag.Sections = _sectionService.GetSectionsWithTopics(onlyPublishedTopics: false);
        return View(new TopicModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ManageTopics(TopicModel topic)
    {
        if (!string.IsNullOrWhiteSpace(topic.Title) && !string.IsNullOrWhiteSpace(topic.Slug) && topic.SectionId > 0)
        {
            topic.Slug = topic.Slug.Trim().ToLowerInvariant().Replace(' ', '-');
            _topicService.Add(topic);
            return RedirectToAction(nameof(ManageTopics));
        }
        ViewBag.Sections = _sectionService.GetSectionsWithTopics(onlyPublishedTopics: false);
        return View(topic);
    }

    [HttpGet]
    public IActionResult AddSection() => View(new SectionModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddSection(SectionModel section)
    {
        if (!string.IsNullOrWhiteSpace(section.Title))
        {
            _sectionService.Add(section);
            return RedirectToAction(nameof(ManageTopics));
        }
        return View(section);
    }

    [HttpGet]
    public IActionResult ManageNotes()
    {
        ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
        return View(new NoteModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ManageNotes(NoteModel note)
    {
        ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
        if (!string.IsNullOrWhiteSpace(note.Title) && !string.IsNullOrWhiteSpace(note.BodyMarkdown) && note.TopicId > 0)
        {
            _noteService.Add(note);
            return RedirectToAction(nameof(ManageNotes));
        }
        return View(note);
    }

    [HttpGet]
    public IActionResult ManageCourses()
    {
        ViewBag.Courses = _courseService.GetPublishedCourses();
        ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
        return View(new CourseModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ManageCourses(CourseModel course)
    {
        if (!string.IsNullOrWhiteSpace(course.Title))
        {
            _courseService.Add(course);
            return RedirectToAction(nameof(ManageCourses));
        }
        ViewBag.Courses = _courseService.GetPublishedCourses();
        ViewBag.Topics = _topicService.GetAll(onlyPublished: false);
        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteComment(int commentId)
    {
        _commentService.DeleteAsAdmin(commentId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePublication(int publicationId)
    {
        _publicationService.RemovePublicationModel(publicationId);
        return RedirectToAction(nameof(Index));
    }
}
