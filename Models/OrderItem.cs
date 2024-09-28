using System;
using System.ComponentModel.DataAnnotations;

namespace FurniflexBE.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; } // Foreign key to Order

        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; } // Foreign key to Product

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal Price { get; set; } // Price at the time of order

        // Navigation Properties
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
