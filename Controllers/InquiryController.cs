using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

            int? memberProfileId = null;

            if (IsMember())
            {
                var userId = UserId();
                if (userId.HasValue)
                {
                    var mp = _context.MemberProfiles.FirstOrDefault(x => x.UserId == userId.Value);
                    if (mp != null) memberProfileId = mp.MemberProfileId;

                    var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
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
                MemberProfileId = memberProfileId,
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

            return RedirectToAction(nameof(Success));
        }

        [HttpGet]
        public IActionResult My(string? status)
        {
            if (!IsMember()) return RedirectToLogin();

            var userId = UserId();
            if (userId == null) return RedirectToLogin();

            var profile = _context.MemberProfiles.FirstOrDefault(p => p.UserId == userId.Value);
            if (profile == null) return RedirectToAction("Profile", "Member");

            status = (status ?? "ALL").Trim().ToUpperInvariant();
            if (status != "OPEN" && status != "CLOSED") status = "ALL";

            var q = _context.Inquiries.Where(i => i.MemberProfileId == profile.MemberProfileId);

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

            var userId = UserId();
            if (userId == null) return RedirectToLogin();

            var profile = _context.MemberProfiles.FirstOrDefault(p => p.UserId == userId.Value);
            if (profile == null) return RedirectToAction("Profile", "Member");

            var inquiry = _context.Inquiries
                .FirstOrDefault(i => i.InquiryId == id && i.MemberProfileId == profile.MemberProfileId);

            if (inquiry == null) return NotFound();

            return View(inquiry);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}