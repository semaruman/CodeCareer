using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Services.Implementations.MySqlEfServices
{
    public class CourseMySqlEfService : ICourseService
    {
        public List<CourseModel> GetPublishedCourses()
        {
            using var db = new ApplicationDbContext();
            return db.Courses
                .Include(c => c.CourseTopics)
                .ThenInclude(ct => ct.Topic)
                .AsNoTracking()
                .Where(c => c.IsPublished)
                .OrderBy(c => c.SortOrder)
                .ToList();
        }

        public CourseModel? GetById(int id)
        {
            using var db = new ApplicationDbContext();
            return db.Courses
                .Include(c => c.CourseTopics)
                .ThenInclude(ct => ct.Topic)
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);
        }

        public void Add(CourseModel course)
        {
            using var db = new ApplicationDbContext();
            db.Courses.Add(course);
            db.SaveChanges();
        }

        public void AddTopicToCourse(int courseId, int topicId, int sortOrder)
        {
            using var db = new ApplicationDbContext();
            db.CourseTopics.Add(new CourseTopicModel
            {
                CourseId = courseId,
                TopicId = topicId,
                SortOrder = sortOrder,
            });
            db.SaveChanges();
        }
    }
}
