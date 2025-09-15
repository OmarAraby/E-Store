namespace Estore.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
