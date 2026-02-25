using CodeCareer.Areas.User.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Areas.User.ViewComponents
{
    public class PublicationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PublicationModel publicationP)
        {
            return View(publicationP);
        }
    }
}
