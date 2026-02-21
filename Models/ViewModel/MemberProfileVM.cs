using System;
using System.Collections.Generic;

namespace EventTicketingSystem.Models.ViewModels
{
    public class MemberProfileVM
    {
        public int MemberProfileId { get; set; }
        public int UserId { get; set; }

        public string FullName { get; set; } = "";
        public string? Phone { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public List<CategoryRow> Categories { get; set; } = new();

        public Dictionary<int, int> PreferenceLevels { get; set; } = new();

        public class CategoryRow
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = "";
        }
    }
}
