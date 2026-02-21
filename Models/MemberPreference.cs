using System;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class MemberPreference
    {
        public int MemberProfileId { get; set; }
        public int CategoryId { get; set; }

        // Expected values: VeryLow, Low, Normal, Medium, High, VeryHigh
        [StringLength(20)]
        public string PreferenceLevel { get; set; } = "Normal";

        [Required]
        public DateTime CreatedAt { get; set; }

        // navigation
        public MemberProfile? MemberProfile { get; set; }
        public Category? Category { get; set; }
    }
}