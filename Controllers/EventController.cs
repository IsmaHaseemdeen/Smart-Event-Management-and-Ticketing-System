using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models.ViewModels;

namespace EventTicketingSystem.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? Role() => HttpContext.Session.GetString("UserRole");
        private bool IsLoggedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));
        private bool IsMember() => IsLoggedIn() && string.Equals(Role(), "MEMBER", StringComparison.OrdinalIgnoreCase);

        // Event list
        public async Task<IActionResult> Index(int? categoryId, string? city, int? venueId, string? month)
        {
            var now = DateTime.Now;

            // Parse month (yyyy-MM)
            int? filterYear = null, filterMonth = null;
            if (!string.IsNullOrWhiteSpace(month) && month!.Length >= 7)
            {
                if (int.TryParse(month.Substring(0, 4), out var y) &&
                    int.TryParse(month.Substring(5, 2), out var m) &&
                    m >= 1 && m <= 12)
                {
                    filterYear = y;
                    filterMonth = m;
                }
            }

     
            var eventsQ = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventCategories).ThenInclude(ec => ec.Category)
                .AsNoTracking()
                .Where(e => e.Status == "ACTIVE" || e.Status == "FINISHED");

  
            if (!string.IsNullOrWhiteSpace(city))
                eventsQ = eventsQ.Where(e => e.Venue != null && e.Venue.City == city);

            if (venueId.HasValue)
                eventsQ = eventsQ.Where(e => e.VenueId == venueId.Value);

            if (categoryId.HasValue)
                eventsQ = eventsQ.Where(e => e.EventCategories.Any(ec => ec.CategoryId == categoryId.Value));

            var events = await eventsQ.ToListAsync();

            
            var sessions = await _context.EventSessions
                .AsNoTracking()
                .Where(s =>
                    (s.SessionStatus == "ACTIVE" && s.StartDatetime >= now) ||
                    s.SessionStatus == "FINISHED")
                .ToListAsync();

            
            if (filterYear.HasValue && filterMonth.HasValue)
            {
                events = events
                    .Where(e =>
                    {
                        var es = sessions.Where(s => s.EventId == e.EventId);
                        return es.Any(s => s.StartDatetime.Year == filterYear.Value && s.StartDatetime.Month == filterMonth.Value);
                    })
                    .ToList();
            }

            var ticketTypes = await _context.TicketTypes
                .AsNoTracking()
                .Where(t => t.Status == "ACTIVE")
                .ToListAsync();

            var approvedReviews = await _context.Reviews
                .Include(r => r.MemberProfile)
                .AsNoTracking()
                .Where(r => r.IsApproved == "Y" && events.Select(e => e.EventId).Contains(r.EventId))
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            var reviewAgg = approvedReviews
                .GroupBy(r => r.EventId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Count = g.Count(),
                        Avg = g.Average(x => (double)x.Rating),
                        Top = g.Take(3).Select(x => new MiniReviewVM
                        {
                            Name = x.MemberProfile?.FullName ?? ("Member#" + x.MemberProfileId),
                            Rating = x.Rating,
                            Comment = x.Comment,
                            ReviewDate = x.ReviewDate
                        }).ToList()
                    }
                );

            var cards = new List<EventCardVM>();

            foreach (var e in events)
            {
                var eventSessions = sessions.Where(s => s.EventId == e.EventId).ToList();

                var upcoming = eventSessions
                    .Where(s => s.SessionStatus == "ACTIVE" && s.StartDatetime >= now)
                    .OrderBy(s => s.StartDatetime)
                    .FirstOrDefault();

                var lastFinished = eventSessions
                    .Where(s => s.SessionStatus == "FINISHED")
                    .OrderByDescending(s => s.StartDatetime)
                    .FirstOrDefault();

                var hasUpcoming = upcoming != null;
                var hasFinished = lastFinished != null;

                // Categories
                var cats = e.EventCategories
                    .Where(x => x.Category != null)
                    .Select(x => x.Category!.CategoryName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                // Reviews
                double avg = 0;
                int count = 0;
                var top = new List<MiniReviewVM>();
                if (reviewAgg.TryGetValue(e.EventId, out var ra))
                {
                    avg = ra.Avg;
                    count = ra.Count;
                    top = ra.Top;
                }

                // Pricing + availability based 
                decimal? startingPrice = null;
                string availability = "N/A";

                if (upcoming != null)
                {
                    var types = ticketTypes.Where(t => t.SessionId == upcoming.SessionId).ToList();

                    if (types.Any() && IsMember())
                        startingPrice = types.Min(t => t.Price);

                   
                    if (upcoming.TotalSeats > 0)
                    {
                        var remaining = upcoming.TotalSeats - (upcoming.ReservedSeats + upcoming.SoldSeats);
                        if (remaining < 0) remaining = 0;

                        if (remaining <= 0) availability = "Full";
                        else if (((decimal)remaining / upcoming.TotalSeats) <= 0.20m) availability = "Few Left";
                        else availability = "Available";
                    }
                    else
                    {
                        // not configured
                        availability = "N/A";
                    }
                }

                // Badge
                string badgeLabel, badgeClass;
                if (!hasUpcoming && hasFinished)
                {
                    badgeLabel = "Finished";
                    badgeClass = "bg-dark";
                }
                else if (hasUpcoming && hasFinished)
                {
                    badgeLabel = "Booking Open";
                    badgeClass = "bg-success";
                }
                else
                {
                    badgeLabel = "Upcoming";
                    badgeClass = "bg-success";
                }

                cards.Add(new EventCardVM
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    VenueName = e.Venue?.VenueName ?? "",
                    City = e.Venue?.City ?? "",

                    UpcomingStart = upcoming?.StartDatetime,

                    LastFinishedStart = lastFinished?.StartDatetime,
                    HasFinishedSession = hasFinished,

                    StartingPrice = startingPrice,
                    Availability = availability,

                    Categories = cats,

                    BadgeLabel = badgeLabel,
                    BadgeClass = badgeClass,

                    AvgRating = avg,
                    ReviewCount = count,
                    TopReviews = hasFinished ? top : new List<MiniReviewVM>()
                });
            }

            cards = cards
                .OrderBy(c => c.UpcomingStart ?? DateTime.MaxValue)
                .ThenBy(c => c.Title)
                .ToList();

            var vm = new EventSearchVM
            {
                CategoryId = categoryId,
                City = city,
                VenueId = venueId,
                Month = month,

                Categories = await _context.Categories.AsNoTracking().OrderBy(c => c.CategoryName).ToListAsync(),
                Cities = await _context.Venues.AsNoTracking().Select(v => v.City).Distinct().OrderBy(x => x).ToListAsync(),
                Venues = await _context.Venues.AsNoTracking().OrderBy(v => v.VenueName).ToListAsync(),

                Results = cards,
                IsMember = IsMember()
            };

            return View(vm);
        }

        // Event Details
        public async Task<IActionResult> Details(int id)
        {
            var now = DateTime.Now;

            var evt = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventCategories).ThenInclude(ec => ec.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (evt == null) return NotFound();

            var upcoming = await _context.EventSessions
                .AsNoTracking()
                .Where(s => s.EventId == id && s.SessionStatus == "ACTIVE" && s.StartDatetime >= now)
                .OrderBy(s => s.StartDatetime)
                .FirstOrDefaultAsync();

            var lastFinished = await _context.EventSessions
                .AsNoTracking()
                .Where(s => s.EventId == id && s.SessionStatus == "FINISHED")
                .OrderByDescending(s => s.StartDatetime)
                .FirstOrDefaultAsync();

            var displaySession = upcoming ?? lastFinished;

            var hasFinishedSession = (lastFinished != null);

            var reviews = await _context.Reviews
                .Include(r => r.MemberProfile)
                .AsNoTracking()
                .Where(r => r.EventId == id && r.IsApproved == "Y")
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            var vm = new EventDetailsVM
            {
                EventId = evt.EventId,
                Title = evt.Title,
                Description = evt.Description,
                PosterUrl = evt.PosterUrl,
                VenueName = evt.Venue?.VenueName ?? "",
                City = evt.Venue?.City ?? "",
                IsLoggedIn = IsLoggedIn(),
                IsMember = IsMember()
            };

            if (displaySession != null)
            {
                vm.Sessions.Add(new SessionBlockVM
                {
                    SessionId = displaySession.SessionId,
                    StartDateTime = displaySession.StartDatetime,
                    EndDateTime = displaySession.EndDatetime,
                    SessionStatus = displaySession.SessionStatus
                });
            }

            ViewBag.HasFinishedSession = hasFinishedSession;
            ViewBag.HasUpcomingSession = (upcoming != null);

            if (vm.IsMember || hasFinishedSession)
            {
                foreach (var r in reviews)
                {
                    vm.Reviews.Add(new ReviewBlockVM
                    {
                        MemberEmail = r.MemberProfile?.FullName ?? ("Member#" + r.MemberProfileId),
                        Rating = r.Rating,
                        Comment = r.Comment,
                        ReviewDate = r.ReviewDate
                    });
                }
            }

            return View(vm);
        }
    }
}