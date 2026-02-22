using System;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        [Required]
        [StringLength(1)]
        public string IsApproved { get; set; } = "N";

        public int? IsApprovedBy { get; set; }

        // navigation
        public Users? User { get; set; }
        public Event? Event { get; set; }
        public Users? ApprovedByUser { get; set; }
    }
}