using System.Diagnostics;
using ASP.NET_OLX.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_OLX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string category)
        {
            ViewBag.SelectedCategory = category ?? "ALL CATEGORIES";

            // Sample Hardcoded Products with Correct Image Paths
            var products = new List<dynamic>
            {
                new { Id = 1, Name = "Honda City 2020", Price = "₹12,00,000", Image = "/images/download (2).jpeg", Category = "Cars", Description = "Well-maintained Honda City 2020 with single ownership. Regularly serviced and driven only 20,000 km.", Location = "Bangalore, Karnataka", Seller = new { Name = "John Doe", Contact = "+91 98765 43210" } },
                new { Id = 2, Name = "Samsung Galaxy S22", Price = "₹80,000", Image = "/images/OIP (2).jpeg", Category = "Mobile Phones", Description = "Brand new Samsung Galaxy S22 with warranty.", Location = "Chennai, Tamil Nadu", Seller = new { Name = "Jane Smith", Contact = "+91 98765 43211" } },
                new { Id = 3, Name = "Yamaha R15", Price = "₹1,50,000", Image = "/images/OIP (1).jpeg", Category = "Motorcycles", Description = "Yamaha R15 2021 model, excellent condition.", Location = "Hyderabad, Telangana", Seller = new { Name = "Alice Williams", Contact = "+91 98765 43212" } },
                new { Id = 4, Name = "For Rent: Luxury Villa", Price = "₹1,00,000/month", Image = "/images/download (1).jpeg", Category = "For Rent: Houses & Apartments", Description = "Spacious luxury villa in a prime location.", Location = "Mumbai, Maharashtra", Seller = new { Name = "Bob Brown", Contact = "+91 98765 43213" } },
                new { Id = 5, Name = "Scooty Pep+", Price = "₹50,000", Image = "/images/download.jpeg", Category = "Scooters", Description = "Scooty Pep+ for sale, well-maintained.", Location = "Delhi, India", Seller = new { Name = "Charlie Green", Contact = "+91 98765 43214" } },
                new { Id = 6, Name = "Sony Bravia TV", Price = "₹70,000", Image = "/images/OIP.jpeg", Category = "Electronics", Description = "Brand new Sony Bravia TV, 40 inches.", Location = "Bangalore, Karnataka", Seller = new { Name = "David King", Contact = "+91 98765 43215" } }
            };

            var filteredProducts = category == "ALL CATEGORIES"
                ? products
                : products.Where(p => p.Category == category).ToList();

            return View(filteredProducts);
        }

        // Product details action
        public IActionResult ProductDetails(int id)
        {
            // Sample Hardcoded Products with Rating added
            var products = new List<dynamic>
    {
        new { Id = 1, Name = "Honda City 2020", Price = "₹12,00,000", Image = "/images/download (2).jpeg", Category = "Cars", Description = "Well-maintained Honda City 2020 with single ownership. Regularly serviced and driven only 20,000 km.", Location = "Bangalore, Karnataka", Seller = new { Name = "John Doe", Contact = "+91 98765 43210", Rating = 4.5 } },
        new { Id = 2, Name = "Samsung Galaxy S22", Price = "₹80,000", Image = "/images/OIP (2).jpeg", Category = "Mobile Phones", Description = "Brand new Samsung Galaxy S22 with warranty.", Location = "Chennai, Tamil Nadu", Seller = new { Name = "Jane Smith", Contact = "+91 98765 43211", Rating = 4.7 } },
        new { Id = 3, Name = "Yamaha R15", Price = "₹1,50,000", Image = "/images/OIP (1).jpeg", Category = "Motorcycles", Description = "Yamaha R15 2021 model, excellent condition.", Location = "Hyderabad, Telangana", Seller = new { Name = "Alice Williams", Contact = "+91 98765 43212", Rating = 4.3 } },
        new { Id = 4, Name = "For Rent: Luxury Villa", Price = "₹1,00,000/month", Image = "/images/download (1).jpeg", Category = "For Rent: Houses & Apartments", Description = "Spacious luxury villa in a prime location.", Location = "Mumbai, Maharashtra", Seller = new { Name = "Bob Brown", Contact = "+91 98765 43213", Rating = 4.8 } },
        new { Id = 5, Name = "Scooty Pep+", Price = "₹50,000", Image = "/images/download.jpeg", Category = "Scooters", Description = "Scooty Pep+ for sale, well-maintained.", Location = "Delhi, India", Seller = new { Name = "Charlie Green", Contact = "+91 98765 43214", Rating = 4.2 } },
        new { Id = 6, Name = "Sony Bravia TV", Price = "₹70,000", Image = "/images/OIP.jpeg", Category = "Electronics", Description = "Brand new Sony Bravia TV, 40 inches.", Location = "Bangalore, Karnataka", Seller = new { Name = "David King", Contact = "+91 98765 43215", Rating = 4.6 } }
    };

            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // **Fetch related ads**: Products from the same category, excluding the current product
            var relatedAds = products
                .Where(p => p.Category == product.Category && p.Id != id)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.Image,
                    Price = p.Price
                })
                .ToList();

            var viewModel = new
            {
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                Description = product.Description,
                Location = product.Location,
                Seller = new
                {
                    Name = product.Seller.Name,
                    Contact = product.Seller.Contact,
                    Rating = product.Seller.Rating
                },
                RelatedAds = relatedAds
            };

            return View(viewModel);
        }


        public IActionResult SendMessageToSeller(int id, string userName, string userEmail, string userPhone, string userMessage)
        {
            // Sample Hardcoded Products (the same list you already have)
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

            // Simulate sending the message (you can use an actual email service like SMTP or API here)
            // For now, let's use TempData to show the success message
            TempData["MessageSent"] = "Your message has been sent to the seller!";

            return RedirectToAction("ProductDetails", new { id = id });
        }
        public IActionResult Sell()
        {
            // You can send data to the view (e.g., categories) if needed
            var categories = new List<string>
    {
        "Cars", "Motorcycles", "Mobile Phones", "For Sale: Houses & Apartments",
        "Scooters", "Commercial & Other Vehicles", "For Rent: Houses & Apartments"
    };

            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public IActionResult Sell(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Save the product details, including image
                // For simplicity, let's assume the product is saved in a list or database
                string imagePath = "/images/" + imageFile.FileName;

                // Simulate saving the product (you would ideally save to a database here)
                product.Image = imagePath;

                // For now, you can return a success message
                TempData["Message"] = "Product added successfully!";

                // Redirect to a confirmation page or back to the product listing page
                return RedirectToAction("Index");
            }

            // If validation failed, return the form with the existing data
            return View(product);
        }
        public IActionResult SendMessageToSeller(int id, string userName, string userEmail, string userMessage)
        {
            // Save message logic here (Database or Email notification)
            return RedirectToAction("ProductDetails", new { id = id });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}