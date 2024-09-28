using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurniflexBE.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign key to User

        [Required(ErrorMessage = "Total price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Total price must be a positive value")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Order status is required")]
        [StringLength(50, ErrorMessage = "Order status cannot exceed 50 characters")]
        public string OrderStatus { get; set; } // e.g., "Pending", "Shipped", "Delivered"

        [Required]
        [StringLength(255, ErrorMessage = "Payment status cannot exceed 255 characters")]
        public string PaymentStatus { get; set; } // e.g., "Paid", "Unpaid"

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Property for User
        public virtual User User { get; set; }

        // Navigation Property for OrderItems
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
