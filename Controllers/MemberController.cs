using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
            => !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));

        private string? Role()
            => HttpContext.Session.GetString("UserRole");

        private IActionResult RedirectToLogin()
            => RedirectToAction("Login", "User");

        private bool IsMember()
            => IsLoggedIn() && string.Equals(Role(), "MEMBER", StringComparison.OrdinalIgnoreCase);

        private int? UserId()
            => HttpContext.Session.GetInt32("UserId");

        [HttpGet]
        public IActionResult Profile()
        {
            if (!IsMember()) return RedirectToLogin();

            var userId = UserId();
            if (!userId.HasValue) return RedirectToLogin();

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
            if (user == null) return RedirectToLogin();

            return View(user); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(string fullName, string phone)
        {
            if (!IsMember()) return RedirectToLogin();

            var userId = UserId();
            if (!userId.HasValue) return RedirectToLogin();

            fullName = (fullName ?? "").Trim();
            phone = (phone ?? "").Trim();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                TempData["Err"] = "Full name is required.";
                return RedirectToAction(nameof(Profile));
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                TempData["Err"] = "Phone number is required.";
                return RedirectToAction(nameof(Profile));
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
            if (user == null) return RedirectToLogin();

            user.FullName = fullName;
            user.Phone = phone;

            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile));
        }
    }
}