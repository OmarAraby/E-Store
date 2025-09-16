using Estore.Domain.Repositories;
using Estore.Infrastructure.Context;

namespace Estore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        public IUserRepository UserRepository { get; }
        public IProductRepository ProductRepository {  get; }
        public IProductImageRepository ProductImageRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository {  get; }

        public UnitOfWork(ApplicationDbContext context , IUserRepository userRepository,IProductRepository productRepository 
            , IProductImageRepository productImageRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _context = context;
            UserRepository = userRepository;
            ProductRepository = productRepository;
            ProductImageRepository = productImageRepository;
            RefreshTokenRepository = refreshTokenRepository;
        }
     
      
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
