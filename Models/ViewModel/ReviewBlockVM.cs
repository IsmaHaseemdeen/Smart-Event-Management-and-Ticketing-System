using System;

namespace EventTicketingSystem.Models.ViewModels
{
    public class ReviewBlockVM
    {
        public string MemberEmail { get; set; } = "";
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
