﻿using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Implementation.ShoppingCarts.Outgoings
{
    public sealed class OutputCartItemDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public string ProductName { get; set; }

        public string ImageUrl { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal SubTotal => UnitPrice * Quantity;
    }
}
