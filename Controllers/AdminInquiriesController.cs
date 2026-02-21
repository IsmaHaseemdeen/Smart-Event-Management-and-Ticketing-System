using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;

namespace EventTicketingSystem.Controllers
{
    public class AdminInquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminInquiriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
            => !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));

        private string? Role()
            => HttpContext.Session.GetString("UserRole");

        private IActionResult RedirectToLogin()
            => RedirectToAction("Login", "User");

        private bool IsAdmin()
            => IsLoggedIn() && string.Equals(Role(), "ADMIN", StringComparison.OrdinalIgnoreCase);

        public IActionResult Index(string? status, string? type)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var q = _context.Inquiries
                .Include(i => i.MemberProfile)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
            {
                type = type.ToUpper();
                var allowed = new[] { "GENERAL", "EVENT", "MEMBERSHIP", "TECHNICAL" };
                if (allowed.Contains(type))
                    q = q.Where(i => i.InquiryType == type);
            }

            var list = q.OrderByDescending(i => i.CreatedAt).ToList();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var inquiry = _context.Inquiries
                .Include(i => i.MemberProfile)
                .FirstOrDefault(i => i.InquiryId == id);

            if (inquiry == null) return NotFound();
            return View(inquiry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int inquiryId, string status, string? reply)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var inquiry = _context.Inquiries.FirstOrDefault(i => i.InquiryId == inquiryId);
            if (inquiry == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(reply))
            {
                inquiry.Reply = reply.Trim();
                inquiry.RepliedAt = DateTime.Now;

                var adminUserId = HttpContext.Session.GetInt32("UserId");
                if (adminUserId.HasValue)
                    inquiry.RepliedBy = adminUserId.Value;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = inquiryId });
        }
    }
}