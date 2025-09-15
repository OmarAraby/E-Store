using Estore.Domain.Entities;

namespace Estore.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {

        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
        Task<RefreshToken> UpdateAsync(RefreshToken refreshToken);
        Task DeleteExpiredTokensAsync();
        Task RevokeTokenAsync(string token);
        Task RevokeAllUserTokensAsync(Guid userId);
    }
}
