using Microsoft.AspNetCore.Mvc;

namespace Application.Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
