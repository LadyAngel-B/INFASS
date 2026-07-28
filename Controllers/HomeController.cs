using System.Diagnostics;
using INFASS.Models;
using Microsoft.AspNetCore.Mvc;

namespace INFASS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

       

        [HttpPost]
        public IActionResult Register([FromBody] RegisterViewModel model)
        {
            User user = new User(model.FullName, model.Email, model.Password);

            string[] fields = { "Name", "Email", "Password" };
            object[] values = { model.FullName, model.Email, model.Password };

            string query = user.GenerateInsertQuery("Users", fields, values);

            return Json(new { query = query });
        }
    }
}
