using INFASS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace INFASS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

        //[HttpPost]
        //public IActionResult Register([FromBody] RegisterViewModel model)
        //{
        //    User user = new User(model.FullName, model.Email, model.Password);

        //    string[] fields = { "Name", "Email", "Password" };
        //    object[] values = { model.FullName, model.Email, model.Password };

        //    string query = user.GenerateInsertQuery("Users", fields, values);

        //    return Json(new { query = query });
        //}


        [HttpPost]
        public IActionResult Register([FromBody] RegisterViewModel model)
        {
            User user = new User(model.FullName, model.Email, model.Password);

            string[] fields = { "Name", "Email", "Password" };
            object[] values = { model.FullName, model.Email, model.Password };

            string query = user.GenerateInsertQuery("Users", fields, values);

            string connectionString = "Data Source=LAB2-PC20\\LAB3PC44;Initial Catalog=INFASSDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }

            return Json(new { query = query });
            //return Json(new { message = "User registered successfully" });
        }

        // SELECT
        [HttpPost]
        public IActionResult SelectUser([FromBody] LoginViewModel model)
        {
            User user = new User(
                "",
                model.Email,
                ""
            );

            string query = user.GenerateSelectQuery(
                "Users",
                "Email",
                model.Email
            );

            return Json(new { query = query });
        }

        // UPDATE
        [HttpPost]
        public IActionResult Update([FromBody] RegisterViewModel model)
        {
            User user = new User(
                model.FullName,
                model.Email,
                model.Password
            );

            string[] fields = { "Name", "Password" };

            object[] values =
            {
        model.FullName,
        model.Password
    };

            string query = user.GenerateUpdateQuery(
                "Users",
                fields,
                values,
                "Email",
                model.Email
            );

            return Json(new { query = query });
        }


        // DELETE
        [HttpPost]
        public IActionResult Delete([FromBody] LoginViewModel model)
        {
            User user = new User(
                "",
                model.Email,
                ""
            );

            string query = user.GenerateDeleteQuery(
                "Users",
                "Email",
                model.Email
            );

            return Json(new { query = query });
        }

        // VIEW PAGE
        public IActionResult ViewUser()
        {
            return View();
        }

        // SELECT
        [HttpPost]
        public IActionResult SelectUser([FromBody] LoginViewModel model)
        {
            User user = new User("", model.Email, "");

            string query = user.GenerateSelectQuery(
                "Users",
                "Email",
                model.Email
            );

            return Json(new { query = query });
        }
    }
}