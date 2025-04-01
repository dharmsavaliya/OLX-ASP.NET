using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace ASP.NET_OLX.Controllers
{
    public class AccountController : Controller
    {
        private readonly string connectionString = "server=localhost;port=3306;database=olx_db;user=root;password=dharm8490;";

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Name, Role FROM Users WHERE Email = @Email AND Password = @Password";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string name = reader.GetString("Name");
                        string role = reader.GetString("Role");

                        HttpContext.Session.SetString("FullName", name);
                        HttpContext.Session.SetString("Email", email);
                        HttpContext.Session.SetString("Role", role);

                        if (role == "Admin")
                        {
                            return RedirectToAction("AdminPanel", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Profile");
                        }
                    }
                }
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string fullName, string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Users (Name, Email, Password, Role) VALUES (@Name, @Email, @Password, 'User')";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", fullName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password); // Ideally, hash this password
                cmd.ExecuteNonQuery();
            }

            HttpContext.Session.SetString("FullName", fullName);
            HttpContext.Session.SetString("Email", email);
            HttpContext.Session.SetString("Role", "User");

            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("FullName") != null)
            {
                ViewBag.FullName = HttpContext.Session.GetString("FullName");
                ViewBag.Email = HttpContext.Session.GetString("Email");
                return View();
            }
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    
        // Wishlist Page
        public IActionResult Wishlist()
        {
            var wishlistItems = new List<string> { "Car", "iPhone", "Laptop" };
            return View(wishlistItems);
        }

        // Messages Page
        public IActionResult Messages()
        {
            var messages = new List<string> { "Hello, is this available?", "Can you negotiate the price?", "Where can I pick it up?" };
            return View(messages);
        }
    }
}
