using System.ComponentModel.DataAnnotations;

namespace CodeCareer.Areas.Admin.Models
{
    public class AdminModel
    {
        public static string PASSWORD = "adminPassword";

        public string Password { get; set; } = string.Empty;
        public bool IsAuthorizate { get; set;} = false;
    }
}
