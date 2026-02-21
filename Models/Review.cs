using System;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int MemberProfileId { get; set; }

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

        [Required]
        public int IsApprovedBy { get; set; } // fk to users - admin

        // navigation
        public MemberProfile? MemberProfile { get; set; }
        public Event? Event { get; set; }
        public Users? ApprovedByUser { get; set; }
    }
}