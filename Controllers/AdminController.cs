using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using ASP.NET_OLX.Models;

namespace ASP.NET_OLX.Controllers
{
    public class AdminController : Controller
    {
        private readonly string connectionString = "server=localhost;port=3306;database=olx_db;user=root;password=dharm8490;";

        public IActionResult AdminPanel()
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        public IActionResult UserManagement()
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                List<User> users = new List<User>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ID, Name, Email, Role FROM Users";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                ID = reader.GetInt32("ID"),
                                Name = reader.GetString("Name"),
                                Email = reader.GetString("Email"),
                                Role = reader.GetString("Role")
                            });
                        }
                    }
                }

                return View(users); // ✅ Now returning List<User> instead of List<object>
            }
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public IActionResult AddUser(string name, string email, string password, string role)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Users (Name, Email, Password, Role) VALUES (@Name, @Email, @Password, @Role)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password); // Ideally, hash this password
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("UserManagement");
            }
            return RedirectToAction("Login", "Account");
        }

        // Product Management
        public IActionResult ProductManagement()
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                List<Product> products = new List<Product>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ID, Name, Price, Image, Category, Description, Location FROM Products";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ID = reader.GetInt32("ID"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetString("Price"),
                                Image = reader.GetString("Image"),
                                Category = reader.GetString("Category"),
                                Description = reader.GetString("Description"),
                                Location = reader.GetString("Location")
                            });
                        }
                    }
                }

                return View(products); // ✅ Ensure this returns List<Product>
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public IActionResult AddProduct(string name, string price, IFormFile ImageFile, string category, string description, string location)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                string imagePath = null; // Default to null

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Generate a unique filename and save the file
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    imagePath = Path.Combine("uploads", uniqueFileName);  // Store relative path
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }
                }
                else
                {
                    imagePath = "uploads/default-image.jpg"; // Use a default image
                }

                // Save product details to the database
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Products (Name, Price, Image, Category, Description, Location) VALUES (@Name, @Price, @Image, @Category, @Description, @Location)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Image", imagePath);  // Store the relative path
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("ProductManagement");
            }
            return RedirectToAction("Login", "Account");
        }


        public IActionResult CategoryManagement()
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                List<Category> categories = new List<Category>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ID, Name, Quantity FROM Categories";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                ID = reader.GetInt32("ID"),
                                Name = reader.GetString("Name"),
                                Quantity = reader.GetInt32("Quantity")
                            });
                        }
                    }
                }

                return View(categories);
            }
            return RedirectToAction("Login", "Account");
        }



        public IActionResult SellerManagement()
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                List<dynamic> sellers = new List<dynamic>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ID, Name, Contact, Rating FROM Sellers";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sellers.Add(new
                            {
                                ID = reader.GetInt32("ID"),
                                Name = reader.GetString("Name"),
                                Contact = reader.GetString("Contact"),
                                Rating = reader.GetDecimal("Rating").ToString("0.0")
                            });
                        }
                    }
                }

                return View(sellers);
            }
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Users WHERE ID = @UserID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", id);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("UserManagement");
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Products WHERE ID = @ProductID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", id);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("ProductManagement");
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public IActionResult AddCategory(string categoryName, int quantity)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Categories (Name, Quantity) VALUES (@CategoryName, @Quantity)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("CategoryManagement");
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult UpdateCategory(int id, string categoryName, int quantity)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Categories SET Name = @CategoryName, Quantity = @Quantity WHERE ID = @CategoryID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CategoryID", id);
                    cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("CategoryManagement");
            }
            return RedirectToAction("Login", "Account");
        }



        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Categories WHERE ID = @CategoryID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CategoryID", id);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("CategoryManagement");
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public IActionResult AddSeller(string name, string contact, decimal rating)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Sellers (Name, Contact, Rating) VALUES (@Name, @Contact, @Rating)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Contact", contact);
                    cmd.Parameters.AddWithValue("@Rating", rating);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("SellerManagement");
            }
            return RedirectToAction("Login", "Account");
        }
        // GET: Admin/EditProduct/5
        public IActionResult EditProduct(int id)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                Product product = null;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Products WHERE ID = @ID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Product
                            {
                                ID = reader.GetInt32("ID"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetString("Price"),
                                Image = reader.GetString("Image"),
                                Category = reader.GetString("Category"),
                                Description = reader.GetString("Description"),
                                Location = reader.GetString("Location")
                            };
                        }
                    }
                }

                return View(product);
            }

            return RedirectToAction("Login", "Account");
        }

        // POST: Admin/UpdateProduct
        [HttpPost]
        public IActionResult UpdateProduct(int id, string name, string price, IFormFile ImageFile, string category, string description, string location)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                string imagePath = null;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    imagePath = Path.Combine("uploads", uniqueFileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = imagePath != null
                        ? "UPDATE Products SET Name = @Name, Price = @Price, Image = @Image, Category = @Category, Description = @Description, Location = @Location WHERE ID = @ID"
                        : "UPDATE Products SET Name = @Name, Price = @Price, Category = @Category, Description = @Description, Location = @Location WHERE ID = @ID";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Price", price);
                    if (imagePath != null) cmd.Parameters.AddWithValue("@Image", imagePath);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Location", location);

                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("ProductManagement");
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult UpdateSeller(int id, string name, string contact, decimal rating)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Sellers SET Name = @Name, Contact = @Contact, Rating = @Rating WHERE ID = @SellerID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SellerID", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Contact", contact);
                    cmd.Parameters.AddWithValue("@Rating", rating);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("SellerManagement");
            }
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public IActionResult DeleteSeller(int id)
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Sellers WHERE ID = @SellerID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SellerID", id);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("SellerManagement");
            }
            return RedirectToAction("Login", "Account");
        }

    }
}
