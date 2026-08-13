using System.Diagnostics;
using INFASS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace INFASS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // HOME
        public IActionResult Index()
        {
            return View();
        }


        // PRIVACY
        public IActionResult Privacy()
        {
            return View();
        }


        // ERROR
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }

        // REGISTER PAGE
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


 
        // CREATE / INSERT USER
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            // Check password
            if (model.Password != model.ConfirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return RedirectToAction("Register");
            }

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if email already exists
                string checkQuery =
                    "SELECT COUNT(*) FROM Users WHERE Email = @Email";

                using (SqlCommand checkCommand =
                       new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue(
                        "@Email",
                        model.Email);

                    int count =
                        (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        TempData["Error"] =
                            "Email already exists.";

                        return RedirectToAction("Register");
                    }
                }


                // INSERT
                string query = @"
                    INSERT INTO Users
                    (Name, Email, Password)
                    VALUES
                    (@Name, @Email, @Password)
                ";

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Name",
                        model.FullName);

                    command.Parameters.AddWithValue(
                        "@Email",
                        model.Email);

                    command.Parameters.AddWithValue(
                        "@Password",
                        model.Password);

                    command.ExecuteNonQuery();
                }
            }

            TempData["Success"] =
                "User registered successfully!";

            return RedirectToAction("Users");
        }


        // VIEW / READ USERS
        [HttpGet]
        public IActionResult Users()
        {
            List<User> users = new List<User>();

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        Id,
                        Name,
                        Email,
                        Password
                    FROM Users
                    ORDER BY Id DESC
                ";

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                Id = Convert.ToInt32(
                                    reader["Id"]),

                                FullName = reader["Name"].ToString()
                                    ?? "",

                                Email = reader["Email"].ToString()
                                    ?? "",

                                Password = reader["Password"].ToString()
                                    ?? ""
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return View(users);
        }


        // UPDATE PAGE

        [HttpGet]
        public IActionResult Edit(int id)
        {
            User user = null;

            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        Id,
                        Name,
                        Email,
                        Password
                    FROM Users
                    WHERE Id = @Id
                ";

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Id",
                        id);

                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = Convert.ToInt32(
                                    reader["Id"]),

                               FullName =
                                    reader["Name"].ToString()
                                    ?? "",

                                Email =
                                    reader["Email"].ToString()
                                    ?? "",

                                Password =
                                    reader["Password"].ToString()
                                    ?? ""
                            };
                        }
                    }
                }
            }

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // UPDATE / SAVE CHANGES

        [HttpPost]
        public IActionResult Edit(User user)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    UPDATE Users
                    SET
                        Name = @Name,
                        Email = @Email,
                        Password = @Password
                    WHERE Id = @Id
                ";

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Id",
                        user.Id);

                    command.Parameters.AddWithValue(
                        "@Name",
                        user.FullName);

                    command.Parameters.AddWithValue(
                        "@Email",
                        user.Email);

                    command.Parameters.AddWithValue(
                        "@Password",
                        user.Password);

                    command.ExecuteNonQuery();
                }
            }

            TempData["Success"] =
                "User updated successfully!";

            return RedirectToAction("Users");
        }

        // DELETE

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    DELETE FROM Users
                    WHERE Id = @Id
                ";

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Id",
                        id);

                    command.ExecuteNonQuery();
                }
            }

            TempData["Success"] =
                "User deleted successfully!";

            return RedirectToAction("Users");
        }



        // LOGIN PAGE
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}