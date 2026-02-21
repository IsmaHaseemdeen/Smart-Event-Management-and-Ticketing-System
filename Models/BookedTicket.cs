using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class BookedTickets
    {
        [Key]
        public int BookedTicketId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int TicketTypeId { get; set; }

        [Required]
        public int Quantity { get; set; } 

        [Required]
        public decimal UnitPrice { get; set; } 

        [Required]
        public decimal LineTotal { get; set; } 

        // navigation
        public Booking? Booking { get; set; }
        public TicketType? TicketType { get; set; }
    }
}