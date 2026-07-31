using CodeCareer.Areas.User.Models;

namespace CodeCareer.Areas.User.Services.Interfaces
{
    public interface ICourseService
    {
        List<CourseModel> GetPublishedCourses();
        CourseModel? GetById(int id);
        void Add(CourseModel course);
        void AddTopicToCourse(int courseId, int topicId, int sortOrder);
    }
}
