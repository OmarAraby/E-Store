using Estore.Domain.Entities;

namespace Estore.Application.DTOS.Auth
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
