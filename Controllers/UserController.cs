using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
            => !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));

        [HttpGet]
        public IActionResult Register()
        {
            if (IsLoggedIn()) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string fullName, string email, string password, string phone, string city)
        {
            if (IsLoggedIn()) return RedirectToAction("Index", "Home");

            fullName = (fullName ?? "").Trim();
            email = (email ?? "").Trim();
            password = password ?? "";
            phone = (phone ?? "").Trim();
            city = (city ?? "").Trim();

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Error = "Please fill all required fields.";
                return View();
            }

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email already exists.";
                return View();
            }

            var now = DateTime.Now;

            var user = new Users
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                Password = password,
                Role = "MEMBER",
                Status = "ACTIVE",
                CreatedAt = now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var profile = new MemberProfile
            {
                UserId = user.UserId,
                FullName = fullName,
                ContactNumber = phone,
                PasswordHash = password,
                City = city,
                Status = "ACTIVE",
                CreatedAt = now
            };

            _context.MemberProfiles.Add(profile);
            _context.SaveChanges();

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (IsLoggedIn()) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            email = (email ?? "").Trim();
            password = password ?? "";

            var user = _context.Users.FirstOrDefault(u =>
                u.Email == email &&
                u.Password == password &&
                u.Status == "ACTIVE");

            if (user == null)
            {
                ViewBag.Error = "Invalid login attempt.";
                return View();
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            if (string.Equals(user.Role, "ADMIN", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Admin");

            if (string.Equals(user.Role, "STAFF", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Verify", "Staff");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}