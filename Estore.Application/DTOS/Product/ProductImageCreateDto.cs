namespace Estore.Application.DTOS.Product
{
    public class ProductImageCreateDto
    {
        public Guid ProductId { get; set; }
        public string FileUrl { get; set; } // URL of the uploaded file
        public string FileName { get; set; } // Name of the file
    }
}
