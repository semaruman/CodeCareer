namespace CodeCareer.Areas.User.Models
{
    public class SkillTagModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SkillTagModel(string name)
        {
            Name = name;
        }
    }
}
