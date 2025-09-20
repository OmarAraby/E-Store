using Estore.Domain.Entities;
using Estore.Domain.Utils;

namespace Estore.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> GetByProductCodeAsync(string productCode);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        // adding paginated method
        Task<PageList<Product>> GetPaginatedProductAsync(ProductQueryParams queryParams);
    }
}
