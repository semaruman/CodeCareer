using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeCareer.Areas.User.Models
{
    public class TaskModel
    {
        [BindNever]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название задачи")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Выберите тип задачи")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Введите содержимое задачи")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Опишите входные данные задачи")]
        public string InputContent { get; set; }

        [Required(ErrorMessage = "Опишите выходные данные задачи")]
        public string OutputContent { get; set; }

        [NotMapped]
        public List<string> InputStrings { get; set; } = new List<string> { "", "", "" };
        public string AllInputStrings { get; set; } //все элементы InputStrings через '; ', чтобы корректно работала БД

        [NotMapped]
        public List<string> OutputStrings { get; set; } = new List<string> { "", "", "" };
        public string AllOutputStrings { get; set; } //все элементы OutputStrings через '; ', чтобы корректно работала БД


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
