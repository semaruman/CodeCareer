using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCareer.Areas.User.Models
{
    public class TagModel
    {
        public int Id { get; set; }

        [Column("name")]
        public string Name {  get; set; }

        [Column("img_path")]
        public string ImgPath { get; set; }

        public TagModel()
        {

        }

        public TagModel(string name)
        {
            Name = name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is TagModel)
            {
                return Name == ((TagModel)obj).Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
