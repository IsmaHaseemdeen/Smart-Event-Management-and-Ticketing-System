using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class AdminTicketTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminTicketTypesController(ApplicationDbContext context)
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

        public async Task<IActionResult> Index(int sessionId)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var session = await _context.EventSessions
                .Include(s => s.Event)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null) return NotFound();

            ViewBag.SessionId = sessionId;
            ViewBag.EventTitle = session.Event?.Title ?? "";
            ViewBag.SessionStart = session.StartDatetime;

            var list = await _context.TicketTypes
                .Where(t => t.SessionId == sessionId)
                .OrderBy(t => t.Price)
                .ToListAsync();

            return View(list);
        }

        public async Task<IActionResult> Create(int sessionId)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var session = await _context.EventSessions
                .Include(s => s.Event)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null) return NotFound();

            ViewBag.SessionId = sessionId;
            ViewBag.EventTitle = session.Event?.Title ?? "";
            ViewBag.SessionStart = session.StartDatetime;

            var model = new TicketType
            {
                SessionId = sessionId,
                Status = "ACTIVE",
                Price = 0m,
                TypeName = ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketType ticketType)
        {
            if (!IsAdmin()) return RedirectToLogin();

            if (ticketType.SessionId <= 0)
                return BadRequest("SessionId required.");

            ticketType.TypeName = (ticketType.TypeName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(ticketType.TypeName))
                ModelState.AddModelError(nameof(ticketType.TypeName), "Type Name is required.");

            if (ticketType.Price < 0)
                ModelState.AddModelError(nameof(ticketType.Price), "Price cannot be negative.");

            if (string.IsNullOrWhiteSpace(ticketType.Status))
                ticketType.Status = "ACTIVE";

            var exists = await _context.TicketTypes.AnyAsync(t =>
                t.SessionId == ticketType.SessionId &&
                t.TypeName == ticketType.TypeName);

            if (exists)
                ModelState.AddModelError(nameof(ticketType.TypeName), "This ticket type already exists for this session.");

            if (!ModelState.IsValid)
            {
                var s = await _context.EventSessions.Include(x => x.Event)
                    .FirstOrDefaultAsync(x => x.SessionId == ticketType.SessionId);

                ViewBag.SessionId = ticketType.SessionId;
                ViewBag.EventTitle = s?.Event?.Title ?? "";
                ViewBag.SessionStart = s?.StartDatetime;

                return View(ticketType);
            }

            _context.TicketTypes.Add(ticketType);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { sessionId = ticketType.SessionId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var tt = await _context.TicketTypes.FindAsync(id);
            if (tt == null) return NotFound();

            var s = await _context.EventSessions.Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.SessionId == tt.SessionId);

            ViewBag.SessionId = tt.SessionId;
            ViewBag.EventTitle = s?.Event?.Title ?? "";
            ViewBag.SessionStart = s?.StartDatetime;

            return View(tt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketType ticketType)
        {
            if (!IsAdmin()) return RedirectToLogin();
            if (id != ticketType.TicketTypeId) return NotFound();

            ticketType.TypeName = (ticketType.TypeName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(ticketType.TypeName))
                ModelState.AddModelError(nameof(ticketType.TypeName), "Type Name is required.");

            if (ticketType.Price < 0)
                ModelState.AddModelError(nameof(ticketType.Price), "Price cannot be negative.");

            if (string.IsNullOrWhiteSpace(ticketType.Status))
                ticketType.Status = "ACTIVE";

            var exists = await _context.TicketTypes.AnyAsync(t =>
                t.TicketTypeId != ticketType.TicketTypeId &&
                t.SessionId == ticketType.SessionId &&
                t.TypeName == ticketType.TypeName);

            if (exists)
                ModelState.AddModelError(nameof(ticketType.TypeName), "This ticket type already exists for this session.");

            if (!ModelState.IsValid)
            {
                var s = await _context.EventSessions.Include(x => x.Event)
                    .FirstOrDefaultAsync(x => x.SessionId == ticketType.SessionId);

                ViewBag.SessionId = ticketType.SessionId;
                ViewBag.EventTitle = s?.Event?.Title ?? "";
                ViewBag.SessionStart = s?.StartDatetime;

                return View(ticketType);
            }

            _context.Update(ticketType);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { sessionId = ticketType.SessionId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var tt = await _context.TicketTypes
                .Include(t => t.Session)
                .ThenInclude(s => s.Event)
                .FirstOrDefaultAsync(t => t.TicketTypeId == id);

            if (tt == null) return NotFound();

            return View(tt);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var tt = await _context.TicketTypes.FindAsync(id);
            if (tt == null) return NotFound();

            var sessionId = tt.SessionId;

            _context.TicketTypes.Remove(tt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { sessionId });
        }
    }
}