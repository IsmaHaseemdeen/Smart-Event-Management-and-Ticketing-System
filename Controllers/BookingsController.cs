using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;
using EventTicketingSystem.Models.ViewModels;

namespace EventTicketingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
            => !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));

        private bool IsMember()
            => string.Equals(HttpContext.Session.GetString("UserRole"), "MEMBER", StringComparison.OrdinalIgnoreCase);

        private IActionResult RedirectToLogin()
            => RedirectToAction("Login", "User");

        private int? CurrentUserId()
            => HttpContext.Session.GetInt32("UserId");

        public IActionResult MyBookings()
        {
            if (!IsLoggedIn() || !IsMember()) return RedirectToLogin();

            var userId = CurrentUserId();
            if (userId == null) return RedirectToLogin();

            var list = _context.Bookings
                .Include(b => b.Session).ThenInclude(s => s.Event)
                .Include(b => b.Payment)
                .Where(b => b.UserId == userId.Value) // ✅ changed
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int sessionId)
        {
            if (!IsLoggedIn() || !IsMember()) return RedirectToLogin();

            var session = _context.EventSessions
                .Include(s => s.Event).ThenInclude(e => e.Venue)
                .FirstOrDefault(s => s.SessionId == sessionId && s.SessionStatus == "ACTIVE");

            if (session == null) return NotFound("Session not found.");

            var ticketTypes = _context.TicketTypes
                .Where(t => t.SessionId == sessionId && t.Status == "ACTIVE")
                .OrderBy(t => t.Price)
                .ToList();

            var remainingSession = GetRemainingForSession(session);

            var vm = new BookingCreateVM
            {
                SessionId = session.SessionId,
                EventTitle = session.Event?.Title ?? "",
                VenueName = session.Event?.Venue?.VenueName ?? "",
                City = session.Event?.Venue?.City ?? "",
                StartText = session.StartDatetime.ToString("yyyy-MM-dd HH:mm"),
                Tickets = ticketTypes.Select(t => new BookingCreateVM.TicketRow
                {
                    TicketTypeId = t.TicketTypeId,
                    TypeName = t.TypeName,
                    Price = t.Price,
                    Remaining = remainingSession,
                    Qty = 0
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookingCreateVM vm)
        {
            if (!IsLoggedIn() || !IsMember()) return RedirectToLogin();

            var userId = CurrentUserId();
            if (userId == null) return RedirectToLogin();

            using var tx = _context.Database.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                var session = _context.EventSessions
                    .FirstOrDefault(s => s.SessionId == vm.SessionId && s.SessionStatus == "ACTIVE");

                if (session == null) return NotFound("Session not found.");

                var rows = (vm.Tickets ?? new()).Where(x => x.Qty > 0).ToList();
                if (rows.Count == 0)
                {
                    TempData["Err"] = "Select at least 1 ticket.";
                    return RedirectToAction(nameof(Create), new { sessionId = vm.SessionId });
                }

                // validate ticket types exist for the session
                var ids = rows.Select(r => r.TicketTypeId).Distinct().ToList();

                var tts = _context.TicketTypes
                    .Where(t => t.SessionId == vm.SessionId && ids.Contains(t.TicketTypeId) && t.Status == "ACTIVE")
                    .ToList();

                if (tts.Count != ids.Count)
                    return BadRequest("Invalid ticket type selection.");

                foreach (var r in rows)
                {
                    if (r.Qty <= 0) return BadRequest("Invalid quantity.");
                }

                // session-level capacity check
                var remainingSession = GetRemainingForSession(session);
                var totalQtyRequested = rows.Sum(r => r.Qty);

                if (totalQtyRequested > remainingSession)
                    return BadRequest($"Not enough seats remaining for this session. Remaining: {remainingSession}");

                var now = DateTime.Now;
                var bookingCode = "BK" + now.ToString("yyyyMMddHHmmss") + new Random().Next(10, 99);

                var booking = new Booking
                {
                    UserId = userId.Value, 
                    SessionId = vm.SessionId,
                    BookingCode = bookingCode,
                    BookingDate = now,
                    BookingStatus = "PENDING",
                    TotalAmount = 0m,
                    IssuedAt = now,
                    IsUsed = "N",
                    CheckedInBy = null
                };

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                decimal total = 0m;

                foreach (var r in rows)
                {
                    var tt = tts.First(x => x.TicketTypeId == r.TicketTypeId);

                    var unit = tt.Price;
                    var lineTotal = unit * r.Qty;
                    total += lineTotal;

                    _context.BookedTickets.Add(new BookedTickets
                    {
                        BookingId = booking.BookingId,
                        TicketTypeId = tt.TicketTypeId,
                        Quantity = r.Qty,
                        UnitPrice = unit,
                        LineTotal = lineTotal
                    });
                }

                booking.TotalAmount = total;

                // reserve seats at session level
                session.ReservedSeats += totalQtyRequested;

                // Not to exceed capacity
                if (session.TotalSeats > 0 && (session.ReservedSeats + session.SoldSeats) > session.TotalSeats)
                    return BadRequest("Capacity exceeded. Please try again.");

                _context.Payments.Add(new Payment
                {
                    BookingId = booking.BookingId,
                    Amount = total,
                    Method = "CARD",
                    PaymentStatus = "PENDING",
                    PaidAt = null,
                    Notes = null
                });

                _context.SaveChanges();
                tx.Commit();

                return RedirectToAction(nameof(Details), new { id = booking.BookingId });
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public IActionResult Details(int id)
        {
            if (!IsLoggedIn() || !IsMember()) return RedirectToLogin();

            var userId = CurrentUserId();
            if (userId == null) return RedirectToLogin();

            var booking = _context.Bookings
                .Include(b => b.Session).ThenInclude(s => s.Event).ThenInclude(e => e.Venue)
                .Include(b => b.BookedTickets).ThenInclude(bt => bt.TicketType)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == id && b.UserId == userId.Value); 

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Pay(int id, string method = "CARD")
        {
            if (!IsLoggedIn() || !IsMember()) return RedirectToLogin();

            var userId = CurrentUserId();
            if (userId == null) return RedirectToLogin();

            if (string.IsNullOrWhiteSpace(method))
            {
                TempData["Err"] = "Please select a payment method.";
                return RedirectToAction(nameof(Details), new { id });
            }

            using var tx = _context.Database.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                var booking = _context.Bookings
                    .Include(b => b.Payment)
                    .Include(b => b.BookedTickets)
                    .Include(b => b.Session)
                    .FirstOrDefault(b => b.BookingId == id && b.UserId == userId.Value); 

                if (booking == null) return NotFound();
                if (booking.Payment == null) return BadRequest("Payment record missing.");
                if (booking.Session == null) return BadRequest("Session missing.");

                if (booking.BookingStatus == "CONFIRMED")
                {
                    TempData["Msg"] = "Already confirmed.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                booking.Payment.PaymentStatus = "SUCCESS";
                booking.Payment.Method = method;
                booking.Payment.PaidAt = DateTime.Now;

                var totalQty = booking.BookedTickets.Sum(x => x.Quantity);

                booking.Session.ReservedSeats = Math.Max(0, booking.Session.ReservedSeats - totalQty);
                booking.Session.SoldSeats += totalQty;

                if (booking.Session.TotalSeats > 0 &&
                    (booking.Session.ReservedSeats + booking.Session.SoldSeats) > booking.Session.TotalSeats)
                {
                    return BadRequest("Capacity exceeded. Contact admin.");
                }

                booking.BookingStatus = "CONFIRMED";

                _context.SaveChanges();
                tx.Commit();

                TempData["Msg"] = "Payment successful! Booking confirmed.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private int GetRemainingForSession(EventSession session)
        {
            if (session.TotalSeats <= 0) return 0;
            var remaining = session.TotalSeats - (session.ReservedSeats + session.SoldSeats);
            return remaining < 0 ? 0 : remaining;
        }
    }
}