using Microsoft.AspNetCore.Mvc;

namespace SkyPathWebApp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
