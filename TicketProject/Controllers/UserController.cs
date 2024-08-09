using Microsoft.AspNetCore.Mvc;

namespace TicketProject.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
