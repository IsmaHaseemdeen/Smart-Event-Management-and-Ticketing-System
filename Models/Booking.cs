using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int MemberProfileId { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        [StringLength(30)]
        public string BookingCode { get; set; } = "";

        [Required]
        public DateTime BookingDate { get; set; } // timestamp

        [Required]
        [StringLength(20)]
        public string BookingStatus { get; set; } = "PENDING"; // PENDING / CONFIRMED

        [Required]
        public decimal TotalAmount { get; set; } // number(10)

        [Required]
        public DateTime IssuedAt { get; set; } // timestamp

        [Required]
        [StringLength(1)]
        public string IsUsed { get; set; } = "N"; // char(1) like Y/N

        public int? CheckedInBy { get; set; } // nullable fk to users

        // navigation
        public MemberProfile? MemberProfile { get; set; }
        public EventSession? Session { get; set; }

        public Users? CheckedInByUser { get; set; }

        public Payment? Payment { get; set; }

        public ICollection<BookedTickets> BookedTickets { get; set; } = new List<BookedTickets>();
    }
}