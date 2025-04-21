using System.Diagnostics;
using OlxProduct = ASP.NET_OLX.Models.Product;
using RazorProduct = Razorpay.Api.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using MySql.Data.MySqlClient;
using ASP.NET_OLX.Models;
using Razorpay.Api;

namespace ASP.NET_OLX.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = "server=localhost;port=3306;database=olx_db;user=root;password=dharm8490;";
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index(string category)
        {
            var products = new List<OlxProduct>();
            string selectedCategory = string.IsNullOrEmpty(category) ? "ALL CATEGORIES" : category;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = selectedCategory == "ALL CATEGORIES"
                    ? "SELECT * FROM products"
                    : "SELECT * FROM products WHERE category = @category";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (selectedCategory != "ALL CATEGORIES")
                        cmd.Parameters.AddWithValue("@category", selectedCategory);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new OlxProduct
                            {
                                ID = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Price = reader.GetString("price"),
                                Image = reader.GetString("image"),
                                Category = reader.GetString("category"),
                                Description = reader.GetString("description"),
                                Location = reader.GetString("location")
                            });
                        }
                    }
                }
            }

            ViewBag.SelectedCategory = selectedCategory;
            return View(products);
        }

        public IActionResult ProductDetails(int id)
        {
            OlxProduct product = null;
            List<OlxProduct> relatedAds = new List<OlxProduct>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string productQuery = "SELECT * FROM products WHERE id = @id";
                using (var cmd = new MySqlCommand(productQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new OlxProduct
                            {
                                ID = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Price = reader.GetString("price"),
                                Image = reader.GetString("image"),
                                Category = reader.GetString("category"),
                                Description = reader.GetString("description"),
                                Location = reader.GetString("location")
                            };
                        }
                    }
                }

                if (product != null)
                {
                    string relatedQuery = "SELECT * FROM products WHERE category = @category AND id != @id LIMIT 4";
                    using (var relatedCmd = new MySqlCommand(relatedQuery, conn))
                    {
                        relatedCmd.Parameters.AddWithValue("@category", product.Category);
                        relatedCmd.Parameters.AddWithValue("@id", id);

                        using (var reader = relatedCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                relatedAds.Add(new OlxProduct
                                {
                                    ID = reader.GetInt32("id"),
                                    Name = reader.GetString("name"),
                                    Price = reader.GetString("price"),
                                    Image = reader.GetString("image"),
                                    Category = reader.GetString("category"),
                                    Description = reader.GetString("description"),
                                    Location = reader.GetString("location")
                                });
                            }
                        }
                    }
                }
            }

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                Seller = new SellerModel
                {
                    Name = "John Doe",
                    Contact = "9876543210",
                    Rating = 4.5
                },
                RelatedAds = relatedAds,
                RazorpayKey = _config["Razorpay:KeyId"]
            };

            return View(viewModel);
        }

        public IActionResult SendMessageToSeller(int id, string userName, string userEmail, string userPhone, string userMessage)
        {
            var products = new List<dynamic>
            {
                new { Id = 1, Name = "Honda City 2020", Price = "₹12,00,000", Image = "/images/download (2).jpeg", Category = "Cars", Description = "Well-maintained Honda City 2020 with single ownership. Regularly serviced and driven only 20,000 km.", Location = "Bangalore, Karnataka", Seller = new { Name = "John Doe", Contact = "+91 98765 43210" } },
                new { Id = 2, Name = "Samsung Galaxy S22", Price = "₹80,000", Image = "/images/OIP (2).jpeg", Category = "Mobile Phones", Description = "Brand new Samsung Galaxy S22 with warranty.", Location = "Chennai, Tamil Nadu", Seller = new { Name = "Jane Smith", Contact = "+91 98765 43211" } },
                new { Id = 3, Name = "Yamaha R15", Price = "₹1,50,000", Image = "/images/OIP (1).jpeg", Category = "Motorcycles", Description = "Yamaha R15 2021 model, excellent condition.", Location = "Hyderabad, Telangana", Seller = new { Name = "Alice Williams", Contact = "+91 98765 43212" } },
                new { Id = 4, Name = "For Rent: Luxury Villa", Price = "₹1,00,000/month", Image = "/images/download (1).jpeg", Category = "For Rent: Houses & Apartments", Description = "Spacious luxury villa in a prime location.", Location = "Mumbai, Maharashtra", Seller = new { Name = "Bob Brown", Contact = "+91 98765 43213" } },
                new { Id = 5, Name = "Scooty Pep+", Price = "₹50,000", Image = "/images/download.jpeg", Category = "Scooters", Description = "Scooty Pep+ for sale, well-maintained.", Location = "Delhi, India", Seller = new { Name = "Charlie Green", Contact = "+91 98765 43214" } },
                new { Id = 6, Name = "Sony Bravia TV", Price = "₹70,000", Image = "/images/OIP.jpeg", Category = "Electronics", Description = "Brand new Sony Bravia TV, 40 inches.", Location = "Bangalore, Karnataka", Seller = new { Name = "David King", Contact = "+91 98765 43215" } }
            };

            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            TempData["MessageSent"] = "Your message has been sent to the seller!";
            return RedirectToAction("ProductDetails", new { id = id });
        }

        [HttpGet]
        public IActionResult Sell()
        {
            List<string> categories = new List<string>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT name FROM categories"; // assuming 'name' is the column in your 'categories' table
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(reader.GetString("name"));
                        }
                    }
                }
            }

            ViewBag.Categories = categories;
            return View();
        }
        [HttpPost]
        
        public IActionResult Sell(OlxProduct product, IFormFile imageFile, string SellerName, string SellerContact)
        {
            if (ModelState.IsValid)
            {
                string imagePath = "/images/" + imageFile.FileName;
                string serverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageFile.FileName);
                using (var stream = new FileStream(serverPath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                product.Image = imagePath;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO products (name, price, image, category, description, location, seller_name, seller_contact) 
                             VALUES (@name, @price, @image, @category, @description, @location, @seller_name, @seller_contact)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", product.Name);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@image", product.Image);
                        cmd.Parameters.AddWithValue("@category", product.Category);
                        cmd.Parameters.AddWithValue("@description", product.Description);
                        cmd.Parameters.AddWithValue("@location", product.Location);
                        cmd.Parameters.AddWithValue("@seller_name", SellerName);
                        cmd.Parameters.AddWithValue("@seller_contact", SellerContact);

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Message"] = "Product added successfully!";
                return RedirectToAction("Index");
            }

            // Repopulate categories if model is invalid
            List<string> categories = new List<string>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT name FROM categories";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(reader.GetString("name"));
                        }
                    }
                }
            }
            ViewBag.Categories = categories;

            return View(product);
        }



        [HttpPost]
        public JsonResult PaymentCallback([FromBody] PaymentData payment)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO payments (product_id, amount, razorpay_payment_id, status) VALUES (@product_id, @amount, @razorpay_payment_id, @status)", conn);
                cmd.Parameters.AddWithValue("@product_id", payment.ProductId);
                cmd.Parameters.AddWithValue("@amount", payment.Amount);
                cmd.Parameters.AddWithValue("@razorpay_payment_id", payment.RazorpayPaymentId);
                cmd.Parameters.AddWithValue("@status", "Success");

                cmd.ExecuteNonQuery();
            }

            return Json(new { message = "Payment successful and recorded!" });
        }

        [HttpPost]
        public JsonResult CreateRazorpayOrder([FromBody] RazorOrderRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Amount))
                    return Json(new { error = "Invalid amount received." });

                string numericStr = new string(request.Amount.Where(char.IsDigit).ToArray());

                if (string.IsNullOrEmpty(numericStr) || !int.TryParse(numericStr, out int rupees))
                    return Json(new { error = "Amount format is invalid." });

                int amountInPaise = rupees * 100;

                var key = _config["Razorpay:KeyId"];
                var secret = _config["Razorpay:KeySecret"];
                var client = new RazorpayClient(key, secret);

                var options = new Dictionary<string, object>
        {
            { "amount", amountInPaise },
            { "currency", "INR" },
            { "receipt", $"order_rcptid_{request.ProductId}" },
            { "payment_capture", 1 }
        };

                var order = client.Order.Create(options);
                return Json(new { orderId = order["id"].ToString() });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
