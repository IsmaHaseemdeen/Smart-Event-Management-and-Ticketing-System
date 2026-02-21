using System.Collections.Generic;

namespace EventTicketingSystem.Models.ViewModels
{
    public class HomeVM
    {
        public bool IsMember { get; set; }
        public List<HomeEventCardVM> TopEvents { get; set; } = new();
    }
}