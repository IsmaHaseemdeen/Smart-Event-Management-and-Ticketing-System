using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? UserId() => HttpContext.Session.GetInt32("UserId");
        private string? Role() => HttpContext.Session.GetString("UserRole");

        private bool IsMember()
            => UserId() != null && string.Equals(Role(), "MEMBER", StringComparison.OrdinalIgnoreCase);

        private IActionResult RedirectToLogin()
            => RedirectToAction("Login", "User");

        // Shows approved reviews for an event.
        public IActionResult Index(int eventId)
        {
            var list = _context.Reviews
                .Include(r => r.MemberProfile)
                .Where(r => r.EventId == eventId && r.IsApproved == "Y")
                .OrderByDescending(r => r.ReviewDate)
                .ToList();

            ViewBag.EventId = eventId;
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int eventId)
        {
            if (!IsMember()) return RedirectToLogin();

            var userId = UserId()!.Value;
            var profile = _context.MemberProfiles.FirstOrDefault(p => p.UserId == userId);
            if (profile == null) return RedirectToAction("Profile", "Member");

            var now = DateTime.Now;

            var attended = _context.Bookings
                .Include(b => b.Session)
                .Any(b =>
                    b.MemberProfileId == profile.MemberProfileId &&
                    b.BookingStatus == "CONFIRMED" &&
                    b.Session != null &&
                    b.Session.EventId == eventId &&
                    b.Session.StartDatetime < now);

            if (!attended)
            {
                TempData["Err"] = "You can write a review only after attending the event.";
                return RedirectToAction(nameof(Index), new { eventId });
            }

            ViewBag.EventId = eventId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int eventId, int rating, string? comment)
        {
            if (!IsMember()) return RedirectToLogin();

            if (rating < 1 || rating > 5)
            {
                ViewBag.EventId = eventId;
                ViewBag.Error = "Rating must be between 1 and 5.";
                return View();
            }

            var userId = UserId()!.Value;
            var profile = _context.MemberProfiles.FirstOrDefault(p => p.UserId == userId);
            if (profile == null) return RedirectToAction("Profile", "Member");

            var now = DateTime.Now;

            var attended = _context.Bookings
                .Include(b => b.Session)
                .Any(b =>
                    b.MemberProfileId == profile.MemberProfileId &&
                    b.BookingStatus == "CONFIRMED" &&
                    b.Session != null &&
                    b.Session.EventId == eventId &&
                    b.Session.StartDatetime < now);

            if (!attended)
            {
                TempData["Err"] = "You can write a review only after attending the event.";
                return RedirectToAction(nameof(Index), new { eventId });
            }

            var exists = _context.Reviews.Any(r => r.EventId == eventId && r.MemberProfileId == profile.MemberProfileId);
            if (exists)
                return RedirectToAction(nameof(Index), new { eventId });

            var review = new Review
            {
                EventId = eventId,
                MemberProfileId = profile.MemberProfileId,
                Rating = rating,
                Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
                ReviewDate = DateTime.Now,
                IsApproved = "N",
                IsApprovedBy = 0
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            TempData["Msg"] = "Review submitted. It will be visible after admin approval.";
            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}