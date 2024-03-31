using Microsoft.AspNetCore.Mvc;
using System.Linq;
using opdracht_1.Models;
using opdracht_1.Data;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace opdracht_1.Controllers
{
    public class UserController : Controller
    {
        private readonly ZooData _context;

        public UserController(ZooData context)
        {
            _context = context;
        }


        public IActionResult Aprove()
        {
            // Haal de gebruikersnaam op uit de sessievariabele
            var username = HttpContext.Session.GetString("UserName");
            // Zoek de gebruiker op basis van de gebruikersnaam
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            // Controleer of de gebruiker niet null is (d.w.z. ingelogd)
            if (user == null)
            {
                // Stuur de gebruiker naar de LogError actie van HomeController
                return RedirectToAction("LogError", "Home");
            }

            // Controleer of de gebruiker de rol "Admin" heeft
            if (user.Role != RoleType.Admin) // Hier ga ik ervan uit dat RoleType.Admin de Admin-rol is
            {
                // Stuur de gebruiker naar de RechtenError actie van HomeController
                return RedirectToAction("RechtenError", "Home");
            }

            var users = _context.Users.ToList();
            return View(users);
        }
        public IActionResult RemoveUser(string name)
        {
            var UserToRemove = _context.Users.FirstOrDefault(u => u.UserName == name);
            if (UserToRemove != null)
            {
                _context.Users.Remove(UserToRemove);
                _context.SaveChanges();
            }
            return RedirectToAction("Aprove");
        }

        [HttpPost]
        public IActionResult UpdateApprovalStatus(int userId, bool isApproved)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.IsApproved = isApproved;
                _context.SaveChanges();
            }
            return RedirectToAction("Aprove");
        }

        [HttpPost]
        public IActionResult UpdateRole(int userId, int role)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                if (Enum.IsDefined(typeof(RoleType), role)) // Controleer of de rolwaarde geldig is
                {
                    user.Role = (RoleType)role;
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Aprove");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string userName, string password)
        {
            // Controleer of de gebruiker al bestaat
            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Gebruiker bestaat al.");
                return View(); // Terug naar de registratiepagina
            }

            // Maak een nieuwe gebruiker aan
            var newUser = new User
            {
                UserName = userName,
                Password = password, // Opmerking: dit is geen veilige manier om wachtwoorden op te slaan, zorg ervoor dat je beveiligingsmaatregelen toevoegt zoals hashing en salting.
                IsApproved = false, // Standaard instellen op false
                Role = RoleType.Gebruiker // Standaard instellen op Gebruiker (Role = 0)
            };

            // Voeg de nieuwe gebruiker toe aan de database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Redirect naar een andere pagina, bijvoorbeeld de startpagina
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }
        private bool IsValidUser(string userName, string password)
        {
            // Zoek naar de gebruiker in de database op basis van de opgegeven gebruikersnaam en wachtwoord
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName && u.Password == password);

            // Controleer of de gebruiker is gevonden
            if (user != null)
            {
                return true; // Gebruiker is geldig
            }
            else
            {
                return false; // Gebruiker is ongeldig
            }
        }
        [HttpPost]
        public IActionResult Login(User model)
        {
            // Zoek de gebruiker op basis van de gebruikersnaam en controleer of het wachtwoord overeenkomt
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName && u.Password == model.Password);

            // Controleer of de gebruiker bestaat en is goedgekeurd
            if (user != null && user.IsApproved)
            {
                // Creëer een sessie voor de gebruiker
                HttpContext.Session.SetString("UserName", model.UserName);
                // Voeg andere gebruikersgegevens toe aan de sessie indien nodig

                return RedirectToAction("Index", "Home"); // Doorsturen naar de startpagina of een andere pagina
            }
            else
            {
                // Gebruiker bestaat niet, is niet goedgekeurd of de combinatie van gebruikersnaam/wachtwoord is ongeldig
                ModelState.AddModelError("", "Ongeldige gebruikersnaam, wachtwoord of de gebruiker is niet goedgekeurd.");
                return View(model); // Terug naar het inlogscherm met foutmelding
            }
        }
        public IActionResult Logout()
        {
            // Verwijder de sessie van de gebruiker
            HttpContext.Session.Clear();

            return View();
        }
    }
}