namespace Estore.Domain.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // foreign key
        public Guid ProductId { get; set; }

        // naviation prop
        public virtual Product Product { get; set; }

    }
}
