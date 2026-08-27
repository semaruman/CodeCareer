using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;

public class CourseMySqlEfService : ICourseService
{
    private readonly ApplicationDbContext _db;

    public CourseMySqlEfService(ApplicationDbContext db) => _db = db;

    public List<CourseModel> GetPublishedCourses() =>
        _db.Courses.Include(c => c.CourseTopics).ThenInclude(ct => ct.Topic).AsNoTracking()
            .Where(c => c.IsPublished).OrderBy(c => c.SortOrder).ToList();

    public CourseModel? GetById(int id) =>
        _db.Courses.Include(c => c.CourseTopics).ThenInclude(ct => ct.Topic).AsNoTracking()
            .FirstOrDefault(c => c.Id == id);

    public void Add(CourseModel course)
    {
        _db.Courses.Add(course);
        _db.SaveChanges();
    }

    public void AddTopicToCourse(int courseId, int topicId, int sortOrder)
    {
        _db.CourseTopics.Add(new CourseTopicModel { CourseId = courseId, TopicId = topicId, SortOrder = sortOrder });
        _db.SaveChanges();
    }
}
