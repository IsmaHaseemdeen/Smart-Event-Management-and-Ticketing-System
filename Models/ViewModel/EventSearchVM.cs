using System.Collections.Generic;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Models.ViewModels
{
    public class EventSearchVM
    {
        // Filters
        public int? CategoryId { get; set; }
        public string? City { get; set; }
        public int? VenueId { get; set; }

        // yyyy-MM (month only)
        public string? Month { get; set; }

        // Dropdown data
        public List<Category> Categories { get; set; } = new();
        public List<string> Cities { get; set; } = new();
        public List<Venue> Venues { get; set; } = new();

        // Results
        public List<EventCardVM> Results { get; set; } = new();

        public bool IsMember { get; set; }
    }
}