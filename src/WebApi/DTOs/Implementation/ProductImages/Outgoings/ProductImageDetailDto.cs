namespace WebApi.DTOs.Implementation.ProductImages.Outgoings
{
    public sealed class ProductImageDetailDto
    {
        public Guid ImageId { get; set; }

        public int UploadOrder { get; set; }

        public string StorageUrl { get; set; }
    }
}
