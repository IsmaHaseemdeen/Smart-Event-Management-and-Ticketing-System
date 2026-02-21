using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class AdminCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCategoriesController(ApplicationDbContext context)
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
            var list = await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToLogin();
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!IsAdmin()) return RedirectToLogin();

            category.CategoryName = (category.CategoryName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(category.CategoryName))
                ModelState.AddModelError(nameof(category.CategoryName), "Category name is required.");

            var exists = await _context.Categories.AnyAsync(c => c.CategoryName == category.CategoryName);
            if (exists)
                ModelState.AddModelError(nameof(category.CategoryName), "This category name already exists.");

            if (!ModelState.IsValid) return View(category);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (!IsAdmin()) return RedirectToLogin();
            if (id != category.CategoryId) return NotFound();

            category.CategoryName = (category.CategoryName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(category.CategoryName))
                ModelState.AddModelError(nameof(category.CategoryName), "Category name is required.");

            var exists = await _context.Categories.AnyAsync(c =>
                c.CategoryId != category.CategoryId && c.CategoryName == category.CategoryName);

            if (exists)
                ModelState.AddModelError(nameof(category.CategoryName), "This category name already exists.");

            if (!ModelState.IsValid) return View(category);

            _context.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
