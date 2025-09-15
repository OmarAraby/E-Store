using Estore.Domain.Entities;

namespace Estore.Domain.Repositories
{
    public interface IProductImageRepository
    {
        Task<ProductImage> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId);
        Task AddAsync(ProductImage image);
        Task DeleteAsync(ProductImage image);
    }
}
