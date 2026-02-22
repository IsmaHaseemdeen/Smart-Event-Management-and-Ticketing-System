using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class InquiryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InquiryController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
            => !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));

        private string? Role()
            => HttpContext.Session.GetString("UserRole");

        private bool IsMember()
            => IsLoggedIn() && string.Equals(Role(), "MEMBER", StringComparison.OrdinalIgnoreCase);

        private IActionResult RedirectToLogin()
            => RedirectToAction("Login", "User");

        private int? UserId()
            => HttpContext.Session.GetInt32("UserId");

        [HttpGet]
        public IActionResult Create()
        {
            if (IsMember())
            {
                var uid = UserId();
                if (uid.HasValue)
                {
                    var u = _context.Users.FirstOrDefault(x => x.UserId == uid.Value);
                    ViewBag.Email = u?.Email ?? "";
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string inquiryType, string email, string subject, string message)
        {
            inquiryType = (inquiryType ?? "").Trim().ToUpperInvariant();
            email = (email ?? "").Trim();
            subject = (subject ?? "").Trim();
            message = (message ?? "").Trim();

            if (string.IsNullOrWhiteSpace(inquiryType) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Error = "Please fill all required fields.";
                return View();
            }

            var allowedTypes = new[] { "GENERAL", "EVENT", "MEMBERSHIP", "TECHNICAL" };
            if (!allowedTypes.Contains(inquiryType))
                inquiryType = "GENERAL";

            int? userId = null;

            
            if (IsMember())
            {
                var uid = UserId();
                if (uid.HasValue)
                {
                    userId = uid.Value;

                    var user = _context.Users.FirstOrDefault(u => u.UserId == uid.Value);
                    if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                        email = user.Email.Trim();
                }
            }

            
            var adminId = _context.Users
                .Where(u => u.Role == "ADMIN" && u.Status == "ACTIVE")
                .OrderBy(u => u.UserId)
                .Select(u => u.UserId)
                .FirstOrDefault();

            if (adminId == 0)
            {
                adminId = _context.Users
                    .OrderBy(u => u.UserId)
                    .Select(u => u.UserId)
                    .FirstOrDefault();
            }

            if (adminId == 0)
            {
                ViewBag.Error = "No users found in the system. Please create an ADMIN user first.";
                return View();
            }

            var inquiry = new Inquiry
            {
                UserId = userId, 
                InquiryType = inquiryType,
                Email = email,
                Subject = subject,
                Message = message,
                Status = "OPEN",
                CreatedAt = DateTime.Now,
                Reply = null,
                RepliedAt = null,
                RepliedBy = adminId
            };

            _context.Inquiries.Add(inquiry);
            _context.SaveChanges();

            TempData["Msg"] = "Inquiry submitted successfully.";
            return RedirectToAction(nameof(Success));
        }

        [HttpGet]
        public IActionResult My(string? status)
        {
            if (!IsMember()) return RedirectToLogin();

            var uid = UserId();
            if (!uid.HasValue) return RedirectToLogin();

            status = (status ?? "ALL").Trim().ToUpperInvariant();
            if (status != "OPEN" && status != "CLOSED") status = "ALL";

            var q = _context.Inquiries.Where(i => i.UserId == uid.Value);

            if (status != "ALL")
                q = q.Where(i => i.Status == status);

            var list = q.OrderByDescending(i => i.CreatedAt).ToList();

            ViewBag.Filter = status;
            return View(list);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!IsMember()) return RedirectToLogin();

            var uid = UserId();
            if (!uid.HasValue) return RedirectToLogin();

            var inquiry = _context.Inquiries
                .FirstOrDefault(i => i.InquiryId == id && i.UserId == uid.Value);

            if (inquiry == null) return NotFound();

            return View(inquiry);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}