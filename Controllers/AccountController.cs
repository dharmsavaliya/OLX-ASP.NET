using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_OLX.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (email == "test@olx.com" && password == "123456")
            {
                HttpContext.Session.SetString("FullName", "Test User");
                HttpContext.Session.SetString("Email", email);
                return RedirectToAction("Profile");
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
            HttpContext.Session.SetString("FullName", fullName);
            HttpContext.Session.SetString("Email", email);
            return RedirectToAction("Profile");
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

        public IActionResult EditProfile()
        {
            if (HttpContext.Session.GetString("FullName") != null)
            {
                ViewBag.FullName = HttpContext.Session.GetString("FullName");
                ViewBag.Email = HttpContext.Session.GetString("Email");
                return View();
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult EditProfile(string fullName, string email)
        {
            HttpContext.Session.SetString("FullName", fullName);
            HttpContext.Session.SetString("Email", email);
            return RedirectToAction("Profile");
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
