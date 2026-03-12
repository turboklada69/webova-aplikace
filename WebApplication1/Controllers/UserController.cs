using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string heslo)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Heslo == heslo);
            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.Jmeno);
                return RedirectToAction("Profile");
            }
            ViewBag.Chyba = "Špatný email nebo heslo";
            return View();
        }

        public IActionResult Profile()
        {
            var jmeno = HttpContext.Session.GetString("UserName");
            if (jmeno == null) return RedirectToAction("Login");
            ViewBag.Jmeno = jmeno;
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}