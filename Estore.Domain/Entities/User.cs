using Microsoft.AspNetCore.Identity;

namespace Estore.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public DateTime? LastLoginTime { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


    }
}
