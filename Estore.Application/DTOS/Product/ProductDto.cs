using Estore.Domain.Entities;

namespace Estore.Application.DTOS.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();


       
    }
}
