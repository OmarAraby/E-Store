namespace Estore.Application.DTOS.Product
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public string? FileName { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
