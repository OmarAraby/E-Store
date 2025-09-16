using Estore.Domain.Entities;
using Estore.Domain.Repositories;
using Estore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Estore.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {

        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Product> CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product); 
            return product; 
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.Category == category)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                          .Include(p => p.ProductImages)
                          .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetByProductCodeAsync(string productCode)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductCode == productCode);
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product); // Removed 'await' as Update does not return a Task
            return product;
        }
    }
}
