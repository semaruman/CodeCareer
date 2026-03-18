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
        [Column("name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Выберите тип задачи")]
        [Column("type")]
        public string Type {get;set;}

        [Required(ErrorMessage = "Введите содержимое задачи")]
        [Column("content")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Опишите входные данные задачи")]
        [Column("input_content")]
        public string InputContent { get; set; }

        [Required(ErrorMessage = "Опишите выходные данные задачи")]
        [Column("output_content")]
        public string OutputContent { get; set; }


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
