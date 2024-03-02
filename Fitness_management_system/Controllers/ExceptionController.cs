using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_management_system.Controllers
{
    [Route("Exception")]
    public class ExceptionController : Controller
    {

        [Route("Error/{statusCode}")]
        [AllowAnonymous]
        public IActionResult Error(int statusCode)
        {
            ViewBag.Status = statusCode;

            return View("./404");
        }


        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var ExceptioHandler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.ExceptionPath = ExceptioHandler.Path;
            ViewBag.ExceptionDetails = ExceptioHandler.Error.Message;
            ViewBag.StackTrace = ExceptioHandler.Error.StackTrace;

            return View("./Error");
        }
    }
}
