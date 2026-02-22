using System;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Inquiry
    {
        [Key]
        public int InquiryId { get; set; }

        public int? UserId { get; set; } // nullable (guest inquiry)

        [Required]
        [StringLength(120)]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(150)]
        public string Subject { get; set; } = "";

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = "";

        [Required]
        [StringLength(30)]
        public string InquiryType { get; set; } = "GENERAL";

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "OPEN";

        [StringLength(1000)]
        public string? Reply { get; set; }

        public DateTime? RepliedAt { get; set; }

        public int? RepliedBy { get; set; }

        // navigation
        public Users? User { get; set; }
        public Users? RepliedByUser { get; set; }
    }
}