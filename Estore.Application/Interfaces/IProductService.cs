using Estore.Application.DTOS.Product;
using Estore.Domain.Utils;

namespace Estore.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(Guid id);
        Task<ProductDto> GetByProductCodeAsync(string productCode);
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);
        Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateProductDto);
        Task<bool> DeleteAsync(Guid id);

        //  paginated products
        Task<PageList<ProductDto>> GetPaginatedAsync(ProductQueryParams queryParams);
    }
}
