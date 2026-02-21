using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class AdminSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSessionsController(ApplicationDbContext context)
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

        private async Task LoadEventsDropDown(int? selectedEventId = null)
        {
            var events = await _context.Events
                .OrderByDescending(e => e.EventId)
                .Select(e => new { e.EventId, e.Title })
                .ToListAsync();

            ViewBag.EventId = new SelectList(events, "EventId", "Title", selectedEventId);
        }

        public async Task<IActionResult> Index(int eventId)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var evt = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (evt == null) return NotFound();

            ViewBag.EventTitle = evt.Title;
            ViewBag.EventId = eventId;

            var list = await _context.EventSessions
                .Where(s => s.EventId == eventId)
                .OrderBy(s => s.StartDatetime)
                .ToListAsync();

            return View(list);
        }

        public async Task<IActionResult> Create(int? eventId)
        {
            if (!IsAdmin()) return RedirectToLogin();

            await LoadEventsDropDown(eventId);

            var start = DateTime.Now.AddDays(1);

            var model = new EventSession
            {
                EventId = eventId ?? 0,
                SessionDate = start.Date,
                StartDatetime = start,
                EndDatetime = start.AddHours(2),
                SessionStatus = "ACTIVE",
                Notes = null,

                // ✅ new
                TotalSeats = 0,

                // system tracked
                ReservedSeats = 0,
                SoldSeats = 0
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventSession session)
        {
            if (!IsAdmin()) return RedirectToLogin();

            if (session.EventId <= 0)
                ModelState.AddModelError(nameof(session.EventId), "Event is required.");

            if (session.StartDatetime == default)
                ModelState.AddModelError(nameof(session.StartDatetime), "Start date/time is required.");

            if (session.SessionDate == default)
                session.SessionDate = session.StartDatetime.Date;

            if (session.EndDatetime.HasValue && session.EndDatetime.Value < session.StartDatetime)
                ModelState.AddModelError(nameof(session.EndDatetime), "End date/time cannot be earlier than start date/time.");

            if (string.IsNullOrWhiteSpace(session.SessionStatus))
                session.SessionStatus = "ACTIVE";

            // ✅ seats validation
            if (session.TotalSeats < 0) session.TotalSeats = 0;
            if (session.ReservedSeats < 0) session.ReservedSeats = 0;
            if (session.SoldSeats < 0) session.SoldSeats = 0;

            if ((session.ReservedSeats + session.SoldSeats) > session.TotalSeats)
                ModelState.AddModelError(nameof(session.TotalSeats), "Total Seats must be >= Reserved Seats + Sold Seats.");

            if (!ModelState.IsValid)
            {
                await LoadEventsDropDown(session.EventId);
                return View(session);
            }

            _context.EventSessions.Add(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { eventId = session.EventId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var session = await _context.EventSessions.FindAsync(id);
            if (session == null) return NotFound();

            await LoadEventsDropDown(session.EventId);
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventSession session)
        {
            if (!IsAdmin()) return RedirectToLogin();
            if (id != session.SessionId) return NotFound();

            if (session.EventId <= 0)
                ModelState.AddModelError(nameof(session.EventId), "Event is required.");

            if (session.StartDatetime == default)
                ModelState.AddModelError(nameof(session.StartDatetime), "Start date/time is required.");

            if (session.SessionDate == default)
                session.SessionDate = session.StartDatetime.Date;

            if (session.EndDatetime.HasValue && session.EndDatetime.Value < session.StartDatetime)
                ModelState.AddModelError(nameof(session.EndDatetime), "End date/time cannot be earlier than start date/time.");

            if (string.IsNullOrWhiteSpace(session.SessionStatus))
                session.SessionStatus = "ACTIVE";

            // seats validation
            if (session.TotalSeats < 0) session.TotalSeats = 0;
            if (session.ReservedSeats < 0) session.ReservedSeats = 0;
            if (session.SoldSeats < 0) session.SoldSeats = 0;

            if ((session.ReservedSeats + session.SoldSeats) > session.TotalSeats)
                ModelState.AddModelError(nameof(session.TotalSeats), "Total Seats must be >= Reserved Seats + Sold Seats.");

            if (!ModelState.IsValid)
            {
                await LoadEventsDropDown(session.EventId);
                return View(session);
            }

            _context.Update(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { eventId = session.EventId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var session = await _context.EventSessions
                .Include(s => s.Event)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null) return NotFound();
            return View(session);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var session = await _context.EventSessions.FindAsync(id);
            if (session == null) return NotFound();

            var eventId = session.EventId;

            _context.EventSessions.Remove(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}