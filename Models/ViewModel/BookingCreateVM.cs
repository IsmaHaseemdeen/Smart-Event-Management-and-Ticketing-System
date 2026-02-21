using System.Collections.Generic;

namespace EventTicketingSystem.Models.ViewModels
{
    public class BookingCreateVM
    {
        public int SessionId { get; set; }
        public string EventTitle { get; set; } = "";
        public string VenueName { get; set; } = "";
        public string City { get; set; } = "";
        public string StartText { get; set; } = "";

        public List<TicketRow> Tickets { get; set; } = new();

        public class TicketRow
        {
            public int TicketTypeId { get; set; }
            public string TypeName { get; set; } = "";
            public decimal Price { get; set; }
            public int Remaining { get; set; }
            public int Qty { get; set; } 
        }
    }
}
