using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public int VenueId { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = "";

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? PosterUrl { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "ACTIVE"; // Active/Finished/Cancelled

        [Required]
        public DateTime CreatedAt { get; set; }

        // navigation
        public Venue? Venue { get; set; }
        public Users? CreatedByUser { get; set; }

        public ICollection<EventSession> Sessions { get; set; } = new List<EventSession>();

        public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}