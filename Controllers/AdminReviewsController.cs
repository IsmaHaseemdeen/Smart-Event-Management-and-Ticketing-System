using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;

namespace EventTicketingSystem.Controllers
{
    public class AdminReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminReviewsController(ApplicationDbContext context)
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

        public IActionResult Index(string? status)
        {
            if (!IsAdmin()) return RedirectToLogin();

            status = (status ?? "ALL").Trim().ToUpperInvariant();

            var q = _context.Reviews
                .Include(r => r.Event)
                .Include(r => r.MemberProfile)
                .AsQueryable();

            if (status == "APPROVED")
                q = q.Where(r => r.IsApproved == "Y");
            else if (status == "PENDING")
                q = q.Where(r => r.IsApproved == "N");

            var list = q.OrderByDescending(r => r.ReviewDate).ToList();
            ViewBag.Filter = status;
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int reviewId)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var r = _context.Reviews.FirstOrDefault(x => x.ReviewId == reviewId);
            if (r == null) return NotFound();

            var adminUserId = HttpContext.Session.GetInt32("UserId");
            if (!adminUserId.HasValue) return RedirectToLogin();

            r.IsApproved = "Y";
            r.IsApprovedBy = adminUserId.Value;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}