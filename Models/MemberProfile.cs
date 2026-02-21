using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class MemberProfile
    {
        [Key]
        public int MemberProfileId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(120)]
        public string FullName { get; set; } = "";

        [Required]
        [StringLength(20)]
        public string ContactNumber { get; set; } = "";

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = "";

        public DateTime? DateOfBirth { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        [StringLength(60)]
        public string City { get; set; } = "";

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "ACTIVE";

        [Required]
        public DateTime CreatedAt { get; set; }

        // navigation
        public Users? User { get; set; }

        public ICollection<MemberPreference> Preferences { get; set; } = new List<MemberPreference>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}