using System;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public decimal Amount { get; set; } 

        [Required]
        [StringLength(30)]
        public string Method { get; set; } = "CARD"; // Card/Bank-Transfer/Online-Transfer

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "PENDING"; // Pending/Success/Failed

        public DateTime? PaidAt { get; set; } // timestamp

        [StringLength(200)]
        public string? Notes { get; set; }

        // navigation
        public Booking? Booking { get; set; }
    }
}