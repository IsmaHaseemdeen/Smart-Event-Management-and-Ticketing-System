using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;
using EventTicketingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        private bool IsMember()
            => string.Equals(HttpContext.Session.GetString("UserRole"), "MEMBER", StringComparison.OrdinalIgnoreCase);

        private static int ReadInt(object obj, params string[] possibleNames)
        {
            if (obj == null) return 0;

            var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var name in possibleNames)
            {
                var wanted = Normalize(name);

                var p = props.FirstOrDefault(x => Normalize(x.Name) == wanted);
                if (p == null) continue;

                var val = p.GetValue(obj);
                if (val == null) return 0;

                try { return Convert.ToInt32(val); }
                catch { return 0; }
            }

            return 0;
        }

        private static DateTime ReadDateTime(object obj, params string[] possibleNames)
        {
            if (obj == null) return default;

            var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var name in possibleNames)
            {
                var wanted = Normalize(name);

                var p = props.FirstOrDefault(x => Normalize(x.Name) == wanted);
                if (p == null) continue;

                var val = p.GetValue(obj);
                if (val == null) return default;

                if (val is DateTime dt) return dt;

                try { return Convert.ToDateTime(val); }
                catch { return default; }
            }

            return default;
        }

        private static string Normalize(string s)
            => new string((s ?? "").Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

        public async Task<IActionResult> Index()
        {
            bool isMember = IsMember();
            var now = DateTime.Now;

            var sessions = await _db.EventSessions
                .Where(s => s.SessionStatus == "ACTIVE" && s.StartDatetime >= now)
                .OrderBy(s => s.StartDatetime)
                .Include(s => s.Event)
                    .ThenInclude(e => e!.Venue)
                .Include(s => s.Event)
                    .ThenInclude(e => e!.EventCategories)
                        .ThenInclude(ec => ec.Category)
                .Include(s => s.TicketTypes)
                .AsNoTracking()
                .ToListAsync();

            var cards = new List<HomeEventCardVM>();
            var seenEventIds = new HashSet<int>();

            foreach (var s in sessions)
            {
                if (s.Event == null) continue;
                if (seenEventIds.Contains(s.EventId)) continue;

                seenEventIds.Add(s.EventId);

                var activeTypes = (s.TicketTypes ?? new List<TicketType>())
                    .Where(tt => tt.Status == "ACTIVE")
                    .ToList();

                //seat equation: Remaining = Total - (Reserved + Sold)
                int totalSeats = activeTypes.Sum(tt => ReadInt(tt, "TotalSeats", "Total_Seats"));
                int reservedSeats = activeTypes.Sum(tt => ReadInt(tt, "ReservedSeats", "Reserved_Seats"));
                int soldSeats = activeTypes.Sum(tt => ReadInt(tt, "SoldSeats", "Sold_Seats"));

                int remainingSeats = totalSeats - (reservedSeats + soldSeats);
                if (remainingSeats < 0) remainingSeats = 0;

                string availability;
                if (totalSeats <= 0)
                    availability = "Available";
                else if (remainingSeats <= 0)
                    availability = "Full";
                else if (remainingSeats <= (int)Math.Ceiling(totalSeats * 0.2))
                    availability = "Few Left";
                else
                    availability = "Available";

                decimal? startingPrice = activeTypes.Count > 0
                    ? activeTypes.Min(tt => tt.Price)
                    : null;

                var categories = (s.Event.EventCategories ?? new List<EventCategory>())
                    .Select(ec => ec.Category?.CategoryName)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct()
                    .ToList();

                cards.Add(new HomeEventCardVM
                {
                    EventId = s.Event.EventId,
                    Title = s.Event.Title,
                    VenueName = s.Event.Venue?.VenueName ?? "N/A",
                    City = s.Event.Venue?.City ?? "N/A",
                    NextStart = ReadDateTime(s, "StartDateTime", "StartDatetime", "Start_DateTime"),
                    StartingPrice = startingPrice,
                    Availability = availability,
                    Categories = categories
                });

                if (cards.Count == 6) break;
            }

            var vm = new HomeVM
            {
                IsMember = isMember,
                TopEvents = cards
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}