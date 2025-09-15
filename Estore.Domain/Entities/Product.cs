namespace Estore.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal DiscountRate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // navigation prop
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new HashSet<ProductImage>();


    }
}
