using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SportsStore.Models
{
    public class Product
    {
        public long ProductID { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; } = 0;

        public string? ImageUrl { get; set; }

        [Required]
        public long? CategoryID { get; set; }

        public Category? Category { get; set; }
    }
}
