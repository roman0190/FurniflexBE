using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FurniflexBE.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign key to User

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int ProductId { get; set; } // Foreign key to Product

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // Navigation Property
        public virtual Order Order { get; set; } // Each cart item can belong to an order

       


    }
}