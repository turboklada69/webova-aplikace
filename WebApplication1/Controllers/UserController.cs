using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using BCrypt.Net;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // ──────────────── REGISTRACE ────────────────

        public IActionResult Register()
        {
            // Pokud je už přihlášen, přesměruj na profil
            if (HttpContext.Session.GetString("UserId") != null)
                return RedirectToAction("Profile");

            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Zkontroluj jestli email už existuje
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ViewBag.Chyba = "Tento email je již zaregistrován.";
                    return View(user);
                }

                // Zahashuj heslo pomocí BCrypt
                user.Heslo = BCrypt.Net.BCrypt.HashPassword(user.Heslo);

                _context.Users.Add(user);
                _context.SaveChanges();

                ViewBag.Uspech = "Registrace proběhla úspěšně! Nyní se přihlaste.";
                return View("Login");
            }
            return View(user);
        }

        // ──────────────── PŘIHLÁŠENÍ ────────────────

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
                return RedirectToAction("Profile");

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string heslo)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            // Ověř heslo pomocí BCrypt (bezpečné porovnání)
            if (user != null && BCrypt.Net.BCrypt.Verify(heslo, user.Heslo))
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.Jmeno);
                HttpContext.Session.SetString("UserEmail", user.Email);
                return RedirectToAction("Profile");
            }

            ViewBag.Chyba = "Špatný email nebo heslo.";
            return View();
        }

        // ──────────────── PROFIL ────────────────

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            ViewBag.Jmeno = HttpContext.Session.GetString("UserName");
            ViewBag.Email = HttpContext.Session.GetString("UserEmail");
            return View();
        }

        // ──────────────── ODHLÁŠENÍ ────────────────

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
