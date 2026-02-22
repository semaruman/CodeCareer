using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
