using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class TicketType
    {
        [Key]
        public int TicketTypeId { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeName { get; set; } = "";

        [Required]
        public decimal Price { get; set; }  

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "ACTIVE";

        // navigation
        public EventSession? Session { get; set; }

        public ICollection<BookedTickets> BookedTickets { get; set; } = new List<BookedTickets>();
    }
}