using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        // GET /User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST /User/Register
        [HttpPost]
        public IActionResult Register(string username, string email, string password)
        {
            // TODO: uložit uživatele do databáze, zahashovat heslo apod.

            // Po úspěšné registraci přesměruj na přihlášení
            TempData["Success"] = "Registrace proběhla úspěšně! Nyní se přihlaste.";
            return RedirectToAction("Login");
        }

        // GET /User/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST /User/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // TODO: ověřit uživatele v databázi

            // Simulace úspěšného přihlášení – v reálu použij session/cookie/Identity
            HttpContext.Session.SetString("LoggedInUser", email);
            return RedirectToAction("Profile");
        }

        // GET /User/Profile
        public IActionResult Profile()
        {
            // Ochrana stránky – nepřihlášený uživatel bude přesměrován na Login
            var user = HttpContext.Session.GetString("LoggedInUser");
            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Email = user;
            return View();
        }

        // GET /User/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
