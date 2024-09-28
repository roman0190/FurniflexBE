using FurniflexBE.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurniflexBE.DTOModels
{
    public class OrderDTO
    {
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }

        [Required]
        public string Postal { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string ContactPhone { get; set; }

        [Required]
        public int SubTotal { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string CVV { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        public List<CartItemsDTO> CartItems { get; set; } // List of order items
    }

    public class CartItemsDTO
    {
        public int ProductId { get; set; }
        public int CartId { get; set; }

        public int Quantity { get; set; }
    }
}
