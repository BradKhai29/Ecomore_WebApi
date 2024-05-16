namespace DTOs.Implementation.Products.Outgoings
{
    public class GetProductForGeneralDisplayDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public decimal UnitPrice { get; set; }

        public int QuantityInStock { get; set; }

        public IEnumerable<string> ImageUrls { get; set; }
    }
}
