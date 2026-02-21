using System;
using System.Collections.Generic;

namespace EventTicketingSystem.Models.ViewModels
{
    public class HomeEventCardVM
    {
        public int EventId { get; set; }

        public string Title { get; set; } = "";
        public string VenueName { get; set; } = "";
        public string City { get; set; } = "";

        public DateTime? NextStart { get; set; }

        // Member-only display
        public decimal? StartingPrice { get; set; }

        // Available/Few Left/ Full
        public string Availability { get; set; } = "Available";

        public List<string> Categories { get; set; } = new();
    }
}