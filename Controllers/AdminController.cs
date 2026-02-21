using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;

namespace EventTicketingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
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
            => IsLoggedIn() &&
               string.Equals(Role(), "ADMIN", StringComparison.OrdinalIgnoreCase);

        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var now = DateTime.Now;

            // Events
            ViewBag.TotalEvents = _context.Events.Count();
            ViewBag.ActiveEvents = _context.Events.Count(e => e.Status == "ACTIVE");
            ViewBag.FinishedEvents = _context.Events.Count(e => e.Status == "FINISHED");
            ViewBag.CancelledEvents = _context.Events.Count(e => e.Status == "CANCELLED");

            // Upcoming sessions
            ViewBag.UpcomingSessions = _context.EventSessions
                .Count(s => s.SessionStatus == "ACTIVE" &&
                            s.StartDatetime >= now);

            // Bookings
            ViewBag.TotalBookings = _context.Bookings.Count();
            ViewBag.PendingBookings = _context.Bookings
                .Count(b => b.BookingStatus == "PENDING");
            ViewBag.ConfirmedBookings = _context.Bookings
                .Count(b => b.BookingStatus == "CONFIRMED");

            // Reviews (approval-based)
            ViewBag.TotalReviews = _context.Reviews.Count();
            ViewBag.ApprovedReviews = _context.Reviews
                .Count(r => r.IsApproved == "Y");
            ViewBag.PendingReviews = _context.Reviews
                .Count(r => r.IsApproved == "N");

            // Inquiries (OPEN / CLOSED only)
            ViewBag.TotalInquiries = _context.Inquiries.Count();
            ViewBag.OpenInquiries = _context.Inquiries
                .Count(i => i.Status == "OPEN");
            ViewBag.ClosedInquiries = _context.Inquiries
                .Count(i => i.Status == "CLOSED");

            return View();
        }
    }
}