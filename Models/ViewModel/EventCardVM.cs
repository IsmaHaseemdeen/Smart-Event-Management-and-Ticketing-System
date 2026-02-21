using System;
using System.Collections.Generic;

namespace EventTicketingSystem.Models.ViewModels
{
    public class EventCardVM
    {
        public int EventId { get; set; }
        public string Title { get; set; } = "";
        public string VenueName { get; set; } = "";
        public string City { get; set; } = "";

        public DateTime? UpcomingStart { get; set; }
        public DateTime? LastFinishedStart { get; set; }

        // Member only
        public decimal? StartingPrice { get; set; }

        public string Availability { get; set; } = "N/A"; // Available/Few Left/Full/N/A

        public List<string> Categories { get; set; } = new();

        // Status badges
        public bool HasUpcomingSession { get; set; }
        public bool HasFinishedSession { get; set; }
        public string BadgeLabel { get; set; } = "Upcoming";
        public string BadgeClass { get; set; } = "bg-success";

        // Rating + few reviews
        public double AvgRating { get; set; }
        public int ReviewCount { get; set; }
        public List<MiniReviewVM> TopReviews { get; set; } = new();
    }

    public class MiniReviewVM
    {
        public string Name { get; set; } = "Anonymous";
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}