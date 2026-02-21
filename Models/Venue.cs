using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        [StringLength(120)]
        public string VenueName { get; set; } = "";

        [StringLength(120)]
        public string? AddressLine1 { get; set; }

        [StringLength(120)]
        public string? AddressLine2 { get; set; }

        [Required]
        [StringLength(60)]
        public string City { get; set; } = "";

        public int? Capacity { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        // navigation
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}