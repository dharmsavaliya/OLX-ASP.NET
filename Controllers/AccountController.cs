using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ASP.NET_OLX.Models;
using Google.Protobuf;

namespace ASP.NET_OLX.Controllers
{
    public class AccountController : Controller
    {
        private readonly string connectionString = "server=localhost;port=3306;database=olx_db;user=root;password=dharm8490;";
        private static List<Message> messages = new List<Message>();


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
            if (HttpContext.Session.GetString("Email") != null)
            {
                string email = HttpContext.Session.GetString("Email");

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Name, Email FROM Users WHERE Email = @Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ViewBag.FullName = reader.GetString("Name");
                            ViewBag.Email = reader.GetString("Email");
                            return View();
                        }
                    }
                }
            }

            return RedirectToAction("Login");
        }

        public IActionResult EditProfile()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                string email = HttpContext.Session.GetString("Email");

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Name, Email FROM Users WHERE Email = @Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ViewBag.FullName = reader.GetString("Name");
                            ViewBag.Email = reader.GetString("Email");
                            return View();
                        }
                    }
                }
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult EditProfile(string fullName, string password)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                string email = HttpContext.Session.GetString("Email");

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Users SET Name = @Name, Password = @Password WHERE Email = @Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", fullName);
                    cmd.Parameters.AddWithValue("@Password", password); // Ideally, hash this password
                    cmd.Parameters.AddWithValue("@Email", email);

                    cmd.ExecuteNonQuery();
                    HttpContext.Session.SetString("FullName", fullName);
                }

                return RedirectToAction("Profile");
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
            if (HttpContext.Session.GetString("Email") == null)
                return RedirectToAction("Login");

            string email = HttpContext.Session.GetString("Email");
            int userId = 0;
            List<Product> wishlistProducts = new List<Product>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Get UserId
                string getUserIdQuery = "SELECT ID FROM Users WHERE Email = @Email";
                MySqlCommand getUserCmd = new MySqlCommand(getUserIdQuery, conn);
                getUserCmd.Parameters.AddWithValue("@Email", email);
                var result = getUserCmd.ExecuteScalar();

                if (result == null)
                    return RedirectToAction("Login");

                userId = Convert.ToInt32(result);

                // Fetch wishlist products
                string query = @"SELECT p.ID, p.Name, p.Price, p.Image
                         FROM Wishlist w
                         JOIN Products p ON w.ProductId = p.ID
                         WHERE w.UserId = @UserId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        wishlistProducts.Add(new Product
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Price = reader.GetString("Price"),
                            Image = reader.GetString("Image")
                        });
                    }
                }
            }

            return View(wishlistProducts);
        }


        [HttpPost]
        public IActionResult AddToWishlist(int productId)
        {
            if (HttpContext.Session.GetString("Email") == null)
                return RedirectToAction("Login");

            string email = HttpContext.Session.GetString("Email");
            int userId = 0;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Get UserId from Email
                string getUserIdQuery = "SELECT ID FROM Users WHERE Email = @Email";
                MySqlCommand getUserCmd = new MySqlCommand(getUserIdQuery, conn);
                getUserCmd.Parameters.AddWithValue("@Email", email);
                var result = getUserCmd.ExecuteScalar();

                if (result == null)
                {
                    return RedirectToAction("Login");
                }

                userId = Convert.ToInt32(result);

                // Check if already in wishlist
                string checkQuery = "SELECT COUNT(*) FROM Wishlist WHERE UserId = @UserId AND ProductId = @ProductId";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                checkCmd.Parameters.AddWithValue("@ProductId", productId);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count == 0)
                {
                    string insertQuery = "INSERT INTO Wishlist (UserId, ProductId) VALUES (@UserId, @ProductId)";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@UserId", userId);
                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
                    insertCmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Wishlist");
        }
        [HttpPost]
        public IActionResult RemoveFromWishlist(int productId)
        {
            if (HttpContext.Session.GetString("Email") == null)
                return RedirectToAction("Login");

            string email = HttpContext.Session.GetString("Email");
            int userId = 0;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Get UserId
                string getUserIdQuery = "SELECT ID FROM Users WHERE Email = @Email";
                MySqlCommand getUserCmd = new MySqlCommand(getUserIdQuery, conn);
                getUserCmd.Parameters.AddWithValue("@Email", email);
                var result = getUserCmd.ExecuteScalar();

                if (result == null)
                    return RedirectToAction("Login");

                userId = Convert.ToInt32(result);

                // Remove from wishlist
                string deleteQuery = "DELETE FROM Wishlist WHERE UserId = @UserId AND ProductId = @ProductId";
                MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@UserId", userId);
                deleteCmd.Parameters.AddWithValue("@ProductId", productId);
                deleteCmd.ExecuteNonQuery();
            }

            return RedirectToAction("Wishlist");
        }



        public IActionResult Messages(string sellerName, int productId)
        {
            var product = new Product
            {
                ID = productId,
                Name = "Product " + productId,
                Seller = new Seller { Name = sellerName, Contact = "Not Available" }
            };

            var viewModel = new MessageViewModel
            {
                Product = product,
                Seller = product.Seller,
                Messages = messages
                    .Where(m => m.ProductId == productId && m.ReceiverName == sellerName)
                    .ToList()
            };

            return View(viewModel);
        }

        // Action to send a message
        [HttpPost]
        public IActionResult SendMessage(int productId, string sellerName, string messageContent)
        {
            string senderName = "CurrentUser"; // Replace with actual logged-in user's name in production
            DateTime timestamp = DateTime.Now;

            var message = new Message
            {
                SenderName = senderName,
                ReceiverName = sellerName,
                ProductId = productId,
                Content = messageContent,
                Timestamp = timestamp
            };

            messages.Add(message);

            return RedirectToAction("Messages", new { sellerName = sellerName, productId = productId });
        }

    }

}
