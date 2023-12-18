using Microsoft.AspNetCore.Mvc;

namespace Reversi.Controllers
{
    public class UserController : Controller
    {
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult LoginUser()
        {
            return View();
        }


        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }
    }
}
