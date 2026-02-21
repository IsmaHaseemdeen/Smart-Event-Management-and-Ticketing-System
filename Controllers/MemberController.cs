using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
            => !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));

        private string? Role()
            => HttpContext.Session.GetString("UserRole");

        private IActionResult RedirectToLogin()
            => RedirectToAction("Login", "User");

        private bool IsMember()
            => IsLoggedIn() && string.Equals(Role(), "MEMBER", StringComparison.OrdinalIgnoreCase);

        private int? UserId()
            => HttpContext.Session.GetInt32("UserId");

        [HttpGet]
        public IActionResult Profile()
        {
            if (!IsMember()) return RedirectToLogin();

            var userId = UserId();
            if (userId == null) return RedirectToLogin();

            var profile = _context.MemberProfiles
                .Include(p => p.Preferences)
                .FirstOrDefault(p => p.UserId == userId.Value);

            if (profile == null)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
                if (user == null) return RedirectToLogin();

                profile = new MemberProfile
                {
                    UserId = user.UserId,
                    FullName = user.FullName ?? "",
                    ContactNumber = user.Phone ?? "",
                    PasswordHash = user.Password ?? "",
                    DateOfBirth = null,
                    Address = null,
                    City = "unknown",
                    Status = "ACTIVE",
                    CreatedAt = DateTime.Now
                };

                _context.MemberProfiles.Add(profile);
                _context.SaveChanges();
            }

            ViewBag.Categories = _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToList();

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(
            int memberProfileId,
            string fullName,
            string contactNumber,
            DateTime? dateOfBirth,
            string? address,
            string city,
            string? status,
            int[] selectedCategoryIds
        )
        {
            if (!IsMember()) return RedirectToLogin();

            var userId = UserId();
            if (userId == null) return RedirectToLogin();

            fullName = (fullName ?? "").Trim();
            contactNumber = (contactNumber ?? "").Trim();
            address = (address ?? "").Trim();
            city = (city ?? "").Trim();
            status = (status ?? "").Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                ViewBag.Error = "Full name is required.";
                return ReloadProfileForThisUser(userId.Value);
            }

            if (string.IsNullOrWhiteSpace(contactNumber))
            {
                ViewBag.Error = "Contact number is required.";
                return ReloadProfileForThisUser(userId.Value);
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Error = "City is required.";
                return ReloadProfileForThisUser(userId.Value);
            }

            if (status != "ACTIVE" && status != "INACTIVE")
                status = "ACTIVE";

            var profile = _context.MemberProfiles
                .Include(p => p.Preferences)
                .FirstOrDefault(p => p.MemberProfileId == memberProfileId && p.UserId == userId.Value);

            if (profile == null) return NotFound();

            profile.FullName = fullName;
            profile.ContactNumber = contactNumber;
            profile.DateOfBirth = dateOfBirth;
            profile.Address = string.IsNullOrWhiteSpace(address) ? null : address;
            profile.City = city;
            profile.Status = status;

            var levelDict = Request.Form
                .Where(kvp => kvp.Key.StartsWith("preferenceLevels[", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    kvp =>
                    {
                        var key = kvp.Key;
                        var start = key.IndexOf('[') + 1;
                        var end = key.IndexOf(']');
                        var idStr = (start > 0 && end > start) ? key.Substring(start, end - start) : "";
                        return int.TryParse(idStr, out var id) ? id : -1;
                    },
                    kvp => int.TryParse(kvp.Value.ToString(), out var lvl) ? lvl : 2
                );

            var existing = _context.MemberPreferences
                .Where(x => x.MemberProfileId == profile.MemberProfileId);

            _context.MemberPreferences.RemoveRange(existing);

            selectedCategoryIds = selectedCategoryIds ?? Array.Empty<int>();

            foreach (var catId in selectedCategoryIds.Distinct())
            {
                if (!_context.Categories.Any(c => c.CategoryId == catId)) continue;

                var lvl = levelDict.TryGetValue(catId, out var v) ? v : 2;
                if (lvl < 0) lvl = 0;
                if (lvl > 5) lvl = 5;

                _context.MemberPreferences.Add(new MemberPreference
                {
                    MemberProfileId = profile.MemberProfileId,
                    CategoryId = catId,
                    PreferenceLevel = lvl.ToString(),
                    CreatedAt = DateTime.Now
                });
            }

            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile));
        }

        private IActionResult ReloadProfileForThisUser(int userId)
        {
            var profile = _context.MemberProfiles
                .Include(p => p.Preferences)
                .FirstOrDefault(p => p.UserId == userId);

            ViewBag.Categories = _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToList();

            return View("Profile", profile);
        }
    }
}