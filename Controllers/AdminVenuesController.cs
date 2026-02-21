using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class AdminVenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminVenuesController(ApplicationDbContext context)
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

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToLogin();
            var venues = await _context.Venues.OrderBy(v => v.VenueName).ToListAsync();
            return View(venues);
        }

        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToLogin();
            return View(new Venue());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (!IsAdmin()) return RedirectToLogin();

            if (string.IsNullOrWhiteSpace(venue.VenueName))
                ModelState.AddModelError(nameof(venue.VenueName), "Venue name is required.");

            if (string.IsNullOrWhiteSpace(venue.City))
                ModelState.AddModelError(nameof(venue.City), "City is required.");

            if (!ModelState.IsValid) return View(venue);

            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (!IsAdmin()) return RedirectToLogin();
            if (id != venue.VenueId) return NotFound();

            if (string.IsNullOrWhiteSpace(venue.VenueName))
                ModelState.AddModelError(nameof(venue.VenueName), "Venue name is required.");

            if (string.IsNullOrWhiteSpace(venue.City))
                ModelState.AddModelError(nameof(venue.City), "City is required.");

            if (!ModelState.IsValid) return View(venue);

            _context.Update(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
