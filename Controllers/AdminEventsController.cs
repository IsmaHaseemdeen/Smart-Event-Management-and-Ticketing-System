using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class AdminEventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminEventsController(ApplicationDbContext context)
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

        private async Task LoadVenuesDropDown(int? selectedVenueId = null)
        {
            var venues = await _context.Venues
                .OrderBy(v => v.VenueName)
                .ToListAsync();

            ViewBag.VenueId = new SelectList(venues, "VenueId", "VenueName", selectedVenueId);
        }

   
        private async Task LoadCategoriesDropDown(int? selectedCategoryId = null)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName", selectedCategoryId);
        }

        private async Task LoadCategoriesForCheckList(IEnumerable<int>? selectedCategoryIds = null)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            ViewBag.AllCategories = categories;
            ViewBag.SelectedCategoryIds = (selectedCategoryIds ?? Enumerable.Empty<int>()).ToHashSet();
        }

        
        public async Task<IActionResult> Index(int? venueId, int? categoryId)
        {
            if (!IsAdmin()) return RedirectToLogin();

            await LoadVenuesDropDown(venueId);
            await LoadCategoriesDropDown(categoryId);

            var q = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventCategories).ThenInclude(ec => ec.Category)
                .AsQueryable();

            if (venueId.HasValue)
                q = q.Where(e => e.VenueId == venueId.Value);

            if (categoryId.HasValue)
                q = q.Where(e => e.EventCategories.Any(ec => ec.CategoryId == categoryId.Value));

            var list = await q
                .OrderByDescending(e => e.EventId)
                .ToListAsync();

            ViewBag.SelectedVenueId = venueId;
            ViewBag.SelectedCategoryId = categoryId;

            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            if (!IsAdmin()) return RedirectToLogin();

            await LoadVenuesDropDown();
            await LoadCategoriesForCheckList();

            return View(new Event
            {
                Status = "ACTIVE",
                CreatedAt = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt, int[] categoryIds)
        {
            if (!IsAdmin()) return RedirectToLogin();

            evt.Title = (evt.Title ?? "").Trim();
            evt.Status = string.IsNullOrWhiteSpace(evt.Status) ? "ACTIVE" : evt.Status.Trim();
            if (evt.CreatedAt == default) evt.CreatedAt = DateTime.Now;

            if (string.IsNullOrWhiteSpace(evt.Title))
                ModelState.AddModelError(nameof(evt.Title), "Title is required.");

            var adminUserId = HttpContext.Session.GetInt32("UserId");
            if (adminUserId.HasValue)
                evt.CreatedBy = adminUserId.Value;

            if (categoryIds == null || categoryIds.Length == 0)
                ModelState.AddModelError("Categories", "Select at least one category.");

            if (!ModelState.IsValid)
            {
                await LoadVenuesDropDown(evt.VenueId);
                await LoadCategoriesForCheckList(categoryIds);
                return View(evt);
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            _context.Events.Add(evt);
            await _context.SaveChangesAsync(); 

           
            foreach (var cid in categoryIds.Distinct())
            {
                _context.EventCategories.Add(new EventCategory
                {
                    EventId = evt.EventId,
                    CategoryId = cid
                });
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var evt = await _context.Events
                .Include(e => e.EventCategories)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (evt == null) return NotFound();

            await LoadVenuesDropDown(evt.VenueId);

            var selected = evt.EventCategories.Select(ec => ec.CategoryId).ToList();
            await LoadCategoriesForCheckList(selected);

            return View(evt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event evt, int[] categoryIds)
        {
            if (!IsAdmin()) return RedirectToLogin();
            if (id != evt.EventId) return NotFound();

            evt.Title = (evt.Title ?? "").Trim();
            evt.Status = string.IsNullOrWhiteSpace(evt.Status) ? "ACTIVE" : evt.Status.Trim();
            if (evt.CreatedAt == default) evt.CreatedAt = DateTime.Now;

            if (string.IsNullOrWhiteSpace(evt.Title))
                ModelState.AddModelError(nameof(evt.Title), "Title is required.");

            if (categoryIds == null || categoryIds.Length == 0)
                ModelState.AddModelError("Categories", "Select at least one category.");

            if (!ModelState.IsValid)
            {
                await LoadVenuesDropDown(evt.VenueId);
                await LoadCategoriesForCheckList(categoryIds);
                return View(evt);
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            _context.Update(evt);
            await _context.SaveChangesAsync();

            var existing = await _context.EventCategories
                .Where(ec => ec.EventId == evt.EventId)
                .ToListAsync();

            _context.EventCategories.RemoveRange(existing);

            foreach (var cid in categoryIds.Distinct())
            {
                _context.EventCategories.Add(new EventCategory
                {
                    EventId = evt.EventId,
                    CategoryId = cid
                });
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var evt = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (evt == null) return NotFound();
            return View(evt);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            using var tx = await _context.Database.BeginTransactionAsync();

            var links = await _context.EventCategories.Where(ec => ec.EventId == id).ToListAsync();
            _context.EventCategories.RemoveRange(links);

            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();

            _context.Events.Remove(evt);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}