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
        public IActionResult Register(string fullName, string email, string password, string phone)
        {
            if (IsLoggedIn()) return RedirectToAction("Index", "Home");

            fullName = (fullName ?? "").Trim();
            email = (email ?? "").Trim();
            password = password ?? "";
            phone = (phone ?? "").Trim();

            // basic validation
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phone))
            {
                TempData["ToastType"] = "danger";
                TempData["ToastMessage"] = "Please fill all required fields.";
                return RedirectToAction(nameof(Register));
            }

            // email exists
            if (_context.Users.Any(u => u.Email == email))
            {
                TempData["ToastType"] = "danger";
                TempData["ToastMessage"] = "Email already exists. Please login instead.";
                return RedirectToAction(nameof(Register));
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

            
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "Account created successfully!";
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
            if (IsLoggedIn()) return RedirectToAction("Index", "Home");

            email = (email ?? "").Trim();
            password = password ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ToastType"] = "danger";
                TempData["ToastMessage"] = "Please enter email and password.";
                return RedirectToAction(nameof(Login));
            }

            // Find user by email first
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            // Wrong email OR wrong password
            if (user == null || user.Password != password)
            {
                TempData["ToastType"] = "danger";
                TempData["ToastMessage"] = "Incorrect email or password.";
                return RedirectToAction(nameof(Login));
            }

            // Inactive account
            if (!string.Equals(user.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ToastType"] = "warning";
                TempData["ToastMessage"] = "Your account is inactive.";
                return RedirectToAction(nameof(Login));
            }

            // Session set
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            // redirect by role
            if (string.Equals(user.Role, "ADMIN", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Admin");

            if (string.Equals(user.Role, "STAFF", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Verify", "Staff");

            return RedirectToAction("Index", "Home");
        }

        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["ToastType"] = "info";
            TempData["ToastMessage"] = "Logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}