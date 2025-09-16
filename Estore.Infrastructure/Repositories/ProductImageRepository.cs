using Estore.Domain.Entities;
using Estore.Domain.Repositories;
using Estore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Estore.Infrastructure.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image); 
        }

        public async Task DeleteAsync(ProductImage image)
        {
            _context.ProductImages.Remove(image);
           
        }

        public async Task<ProductImage> GetByIdAsync(Guid id)
        {
            return await _context.ProductImages
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.Id == id);
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }
    }
}
