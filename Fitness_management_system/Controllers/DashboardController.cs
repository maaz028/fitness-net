
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_management_system.Controllers
{
    [Route("")]
    [Authorize(Roles = "Administration, Member")]
    public class DashboardController : Controller
    {
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            return View("./dashboard");
        }

    }
}

