using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;

namespace EventTicketingSystem.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
            => !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));

        private string Role()
            => (HttpContext.Session.GetString("UserRole") ?? "").Trim();

        private IActionResult RedirectToLogin()
            => RedirectToAction("Login", "User");

        private bool IsStaff()
            => IsLoggedIn() && string.Equals(Role(), "STAFF", StringComparison.OrdinalIgnoreCase);

        [HttpGet]
        public IActionResult Index()
        {
            if (!IsStaff()) return RedirectToLogin();
            return RedirectToAction(nameof(Verify));
        }

        [HttpGet]
        public IActionResult Verify()
        {
            if (!IsStaff()) return RedirectToLogin();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Verify(string bookingCode, string phoneLast2)
        {
            if (!IsStaff()) return RedirectToLogin();

            bookingCode = (bookingCode ?? "").Trim();
            phoneLast2 = (phoneLast2 ?? "").Trim();

            if (string.IsNullOrWhiteSpace(bookingCode) || phoneLast2.Length != 2)
            {
                ViewBag.Result = "INVALID_INPUT";
                return View();
            }

            var booking = _context.Bookings
                .Include(b => b.User) // 
                .Include(b => b.Session).ThenInclude(s => s.Event).ThenInclude(e => e.Venue)
                .Include(b => b.BookedTickets).ThenInclude(bt => bt.TicketType)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingCode == bookingCode);

            if (booking == null)
            {
                ViewBag.Result = "NOT_FOUND";
                return View();
            }

            // must be confirmed
            if (!string.Equals(booking.BookingStatus, "CONFIRMED", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Result = "NOT_CONFIRMED";
                return View();
            }

            // must be PAID/SUCCESS
            if (booking.Payment == null || !string.Equals(booking.Payment.PaymentStatus, "SUCCESS", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Result = "NOT_PAID";
                return View();
            }

            // already used
            if (string.Equals(booking.IsUsed, "Y", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Result = "ALREADY_USED";
                ViewBag.BookingCode = booking.BookingCode;
                return View();
            }

            // phone last 2 digits match 
            var phone = (booking.User?.Phone ?? "").Trim();
            if (phone.Length < 2 || !phone.EndsWith(phoneLast2))
            {
                ViewBag.Result = "PHONE_MISMATCH";
                return View();
            }

            // mark used (check-in)
            booking.IsUsed = "Y";

            var staffUserId = HttpContext.Session.GetInt32("UserId");
            if (staffUserId.HasValue)
                booking.CheckedInBy = staffUserId.Value;

            _context.SaveChanges();

            ViewBag.Result = "VALID";
            ViewBag.BookingCode = booking.BookingCode;
            ViewBag.MemberName = booking.User?.FullName ?? ""; 
            ViewBag.Qty = booking.BookedTickets?.Sum(x => x.Quantity) ?? 0;

            return View();
        }
    }
}