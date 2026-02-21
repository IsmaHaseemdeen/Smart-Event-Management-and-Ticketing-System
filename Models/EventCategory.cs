namespace EventTicketingSystem.Models
{
    public class EventCategory
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }

        // navigation
        public Event? Event { get; set; }
        public Category? Category { get; set; }
    }
}