using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(120)]
        public string FullName { get; set; } = "";

        [Required]
        [StringLength(120)]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = "";

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = "";

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "MEMBER"; // ADMIN / STAFF / MEMBER

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "ACTIVE"; // ACTIVE / INACTIVE

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation


        public ICollection<Event> CreatedEvents { get; set; } = new List<Event>();

        public ICollection<Booking> CheckedInBookings { get; set; } = new List<Booking>();

        public ICollection<Inquiry> RepliedInquiries { get; set; } = new List<Inquiry>();

        public ICollection<Review> ApprovedReviews { get; set; } = new List<Review>();
    }
}