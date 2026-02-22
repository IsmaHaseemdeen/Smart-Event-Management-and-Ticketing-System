using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTicketingSystem.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; } = "";

        [StringLength(200)]
        public string? Description { get; set; }

        // navigation
        public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();

    }
}