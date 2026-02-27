using System.ComponentModel.DataAnnotations;

namespace CodeCareer.Areas.User.Models
{
    public class TagModel
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        public string Type { get; set; }

        public TagModel()
        {

        }

        public TagModel(string name)
        {
            Name = name;
        }

    }
}
