using CodeCareer.Areas.User.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CodeCareer.Areas.User.ViewComponents
{
    public class TaskSViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(TaskModel taskP)
        {
            return View(taskP);
        }
    }
}
