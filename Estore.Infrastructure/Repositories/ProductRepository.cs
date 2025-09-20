using Estore.Domain.Entities;
using Estore.Domain.Repositories;
using Estore.Domain.Utils;
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


        // paginated product
        public async Task<PageList<Product>> GetPaginatedProductAsync(ProductQueryParams queryParams)
        {
            var query = _context.Products.Include(p => p.ProductImages).AsQueryable();

            // filter on search
            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) ||
                                         p.ProductCode.ToLower().Contains(term));
            }

            // filter on cat
            if (!string.IsNullOrWhiteSpace(queryParams.Category))
            {
                query = query.Where(p=>p.Category == queryParams.Category);
            }

            // by price range
            if (queryParams.MinPrice.HasValue)
            {
                query= query.Where(p=>p.Price>=queryParams.MinPrice.Value);
            }
            if (queryParams.MaxPrice.HasValue) 
            {
                query = query.Where(p=>p.Price<=queryParams.MaxPrice.Value);
            }

            // sorting
            query = queryParams.SortBy?.ToLower() switch
            {
                "name" => queryParams.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => queryParams.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                _ => queryParams.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            };

            // get total count  
            var totalCount = await query.CountAsync();

            // apply pagination 
            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return new PageList<Product>(items, totalCount, queryParams.PageNumber, queryParams.PageSize);

        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product); // Removed 'await' as Update does not return a Task
            return product;
        }
    }
}
