using Microsoft.AspNetCore.Mvc;

namespace CodeCareer.Controllers;

public class ErrorsController : Controller
{
    [Route("Errors/404")]
    public IActionResult NotFoundPage() => View("NotFound");

    [Route("Errors/500")]
    public IActionResult ServerError() => View("ServerError");
}
