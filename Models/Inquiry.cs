using System;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Inquiry
    {
        [Key]
        public int InquiryId { get; set; }

        public int? MemberProfileId { get; set; } // nullable

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
        public string InquiryType { get; set; } = "GENERAL"; // General/Event/Membership/Technical

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "OPEN"; // Open/Closed

        [StringLength(1000)]
        public string? Reply { get; set; }

        public DateTime? RepliedAt { get; set; }

        [Required]
        public int RepliedBy { get; set; } // fk to users - admin

        // navigation
        public MemberProfile? MemberProfile { get; set; }
        public Users? RepliedByUser { get; set; }
    }
}