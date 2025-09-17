namespace Estore.Application.DTOS.Product
{
    public class UpdateProductDto
    {

        public string Category { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal DiscountRate { get; set; }
        //public List<IFormFile>? Images { get; set; }
    }
}
