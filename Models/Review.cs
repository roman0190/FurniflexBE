using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FurniflexBE.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign key to User

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int ProductId { get; set; } // Foreign key to Product

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Rating { get; set; } // Rating from 1 to 5

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string Comment { get; set; } // Optional comment

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Auto-updated when the review is edited
    }
}