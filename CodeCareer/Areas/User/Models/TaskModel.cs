using System.ComponentModel.DataAnnotations;

namespace CodeCareer.Areas.User.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }


        public override bool Equals(object? obj)
        {
            if (obj is TaskModel)
            {
                return Name == ((TaskModel)obj).Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
