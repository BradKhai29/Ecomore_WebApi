﻿using DTOs.Implementation.Categories.Outgoings;

namespace DTOs.Implementation.Products.Outgoings
{
    public class GetProductByIdForDetailDisplayDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public GetCategoryByIdForDetailDisplayDto Category { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public decimal UnitPrice { get; set; }

        public int QuantityInStock { get; set; }

        public IEnumerable<string> ImageUrls { get; set; }
    }
}
