using Estore.Domain.Entities;

namespace Estore.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user, string password);
        Task UpdateLastLoginAsync(Guid userId);
        Task DeleteAsync(Guid userId);
        Task<User> UpdateAsync(User user);
        Task<bool> CheckPasswordAsync(User user, string password);

    }
}
