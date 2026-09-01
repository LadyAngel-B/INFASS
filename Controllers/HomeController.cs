using INFASS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace INFASS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration)
        {
            _logger = logger;

            _connectionString =
                configuration.GetConnectionString("DefaultConnection");
        }


        // =========================
        // HOME
        // =========================

        public IActionResult Index()
        {
            return View();
        }


        // =========================
        // PRIVACY
        // =========================

        public IActionResult Privacy()
        {
            return View();
        }


        // =========================
        // ERROR
        // =========================

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
        }


        // =========================
        // REGISTER PAGE
        // =========================

        public IActionResult Register()
        {
            return View();
        }


        // =========================
        // REGISTER USER
        // =========================

        // =========================
        // REGISTER USER
        // =========================

        [HttpPost]
        public IActionResult Register(
            [FromBody] RegisterViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return Json(new
                {
                    success = false,
                    message = "Passwords do not match."
                });
            }

            try
            {
                var hasher = new PasswordHasher<RegisterViewModel>();
                string hashedPassword = hasher.HashPassword(model, model.Password);

                string query = @"
            INSERT INTO Users
            (Name, Email, Password)
            VALUES
            (@Name, @Email, @Password)
        ";

                using (SqlConnection connection =
                       new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command =
                           new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", model.FullName);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@Password", hashedPassword);

                        command.ExecuteNonQuery();
                    }
                }

                return Json(new
                {
                    success = true,
                    message = "User registered successfully!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =========================
        // LOGIN PAGE
        // =========================

        public IActionResult Login()
        {
            return View();
        }

        // =========================
        // LOGIN
        // =========================

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            try
            {
                string query = @"
            SELECT Id, Name, Email, Password
            FROM Users
            WHERE Email = @Email
        ";

                using (SqlConnection connection =
                       new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command =
                           new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", model.Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["Password"].ToString();

                                var hasher = new PasswordHasher<LoginViewModel>();

                                PasswordVerificationResult result =
                                    hasher.VerifyHashedPassword(
                                        model, storedHash, model.Password);

                                if (result == PasswordVerificationResult.Success ||
                                    result == PasswordVerificationResult.SuccessRehashNeeded)
                                {
                                    return RedirectToAction("Dashboard");
                                }
                            }
                        }
                    }
                }

                ViewBag.Error = "Invalid email or password.";
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }


        // =========================
        // DASHBOARD
        // =========================

        public IActionResult Dashboard()
        {
            List<User> users = new List<User>();

            string query = @"
                SELECT Id, Name, Email, Password
                FROM Users
                ORDER BY Id DESC
            ";

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command =
                           new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader =
                               command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                User user = new User();

                                user.Id =
                                    Convert.ToInt32(
                                        reader["Id"]);

                                user.FullName =
                                    reader["Name"].ToString();

                                user.Email =
                                    reader["Email"].ToString();

                                user.Password =
                                    reader["Password"].ToString();

                                users.Add(user);
                            }
                        }
                    }
                }

                return View(users);
            }

            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                return View(users);
            }
        }
            // =========================
            // EDIT USER (GET)
            // =========================

public IActionResult EditUser(int id)
        {
            User user = null;

            string query = @"
        SELECT Id, Name, Email
        FROM Users
        WHERE Id = @Id
    ";

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command =
                           new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader =
                               command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    FullName = reader["Name"].ToString(),
                                    Email = reader["Email"].ToString()
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
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(user);
            }
        }


        // =========================
        // EDIT USER (POST / UPDATE)
        // =========================

        [HttpPost]
        public IActionResult EditUser(User model)
        {
            try
            {
                string query = @"
            UPDATE Users
            SET Name = @Name,
                Email = @Email
            WHERE Id = @Id
        ";

                using (SqlConnection connection =
                       new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command =
                           new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@Name", model.FullName);
                        command.Parameters.AddWithValue("@Email", model.Email);

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }


        // =========================
        // DELETE USER
        // =========================

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                string query = @"
            DELETE FROM Users
            WHERE Id = @Id
        ";

                using (SqlConnection connection =
                       new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command =
                           new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Dashboard");
            }
        }
    }
    
}