using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FurniflexBE.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        [StringLength(255)]
        public string ImgUrl { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; } // Foreign key to Category

        // Navigation Property
        /*[Required(ErrorMessage = "Category is required")]*/
        public virtual Category Category { get; set; } // Navigation property to Category

        [Required(ErrorMessage = "Discounted price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Discounted price must be a positive value")]
        public decimal DiscountedPrice { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal Discount { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number")]
        public int Quantity { get; set; }

        // Navigation Property for Reviews
        public virtual ICollection<Review> Reviews { get; set; }
    }
}