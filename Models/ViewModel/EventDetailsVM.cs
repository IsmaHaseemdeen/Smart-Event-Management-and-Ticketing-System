using System;
using System.Collections.Generic;

namespace EventTicketingSystem.Models.ViewModels
{
    public class EventDetailsVM
    {
        public int EventId { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }

        public string VenueName { get; set; } = "";
        public string City { get; set; } = "";

        public bool IsLoggedIn { get; set; }
        public bool IsMember { get; set; }

        public List<string> Categories { get; set; } = new();
        public bool HasUpcomingSession { get; set; }
        public bool HasFinishedSession { get; set; }
        public DateTime? NextStart { get; set; }
        public int? NextSessionId { get; set; }

        public double AvgRating { get; set; }
        public int ReviewCount { get; set; }

        // kept (not displayed)
        public List<SessionBlockVM> Sessions { get; set; } = new();
        public List<ReviewBlockVM> Reviews { get; set; } = new();
    }

    public class SessionBlockVM
    {
        public int SessionId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string SessionStatus { get; set; } = "ACTIVE";

        public int TotalSeats { get; set; }
        public int ReservedSeats { get; set; }
        public int SoldSeats { get; set; }

        public int RemainingSeats
        {
            get
            {
                var remaining = TotalSeats - (ReservedSeats + SoldSeats);
                return remaining < 0 ? 0 : remaining;
            }
        }

        public string Availability
        {
            get
            {
                if (TotalSeats <= 0) return "Full";
                if (RemainingSeats <= 0) return "Full";
                var ratio = (decimal)RemainingSeats / TotalSeats;
                if (ratio <= 0.20m) return "Few Left";
                return "Available";
            }
        }

        public List<TicketTypeBlockVM> TicketTypes { get; set; } = new();
        public List<ReviewBlockVM> Reviews { get; set; } = new();
    }

    public class TicketTypeBlockVM
    {
        public int TicketTypeId { get; set; }
        public string TypeName { get; set; } = "";
        public decimal Price { get; set; }
    }
}