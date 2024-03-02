using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_management_system.Controllers
{
    public class LandingController : Controller
    {
        [Route("~/")]
        [AllowAnonymous]
        public ViewResult Home()
        {
            return View();
        }
    }
}
