using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class EventSession
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }  

        [Required]
        public DateTime StartDatetime { get; set; } 

        public DateTime? EndDatetime { get; set; } 

        [Required]
        [StringLength(20)]
        public string SessionStatus { get; set; } = "ACTIVE"; // Active/Cancelled/Finished
        [StringLength(200)]
        public string? Notes { get; set; }

        [Required]
        public int TotalSeats { get; set; } = 0;

        [Required]
        public int ReservedSeats { get; set; }

        [Required]
        public int SoldSeats { get; set; }

        // navigation
        public Event? Event { get; set; }

        public ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}