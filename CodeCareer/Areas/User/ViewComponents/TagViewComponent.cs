using CodeCareer.Areas.User.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CodeCareer.Areas.User.ViewComponents
{
    public class TagViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(TagModel tagP)
        {
            return View(tagP);
        }
    }
}
