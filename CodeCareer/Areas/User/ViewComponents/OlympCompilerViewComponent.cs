using CodeCareer.Areas.User.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.User.ViewComponents
{
    public class OlympCompilerViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
