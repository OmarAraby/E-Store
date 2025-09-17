using AutoMapper;
using Estore.Application.DTOS.Auth;
using Estore.Application.Exceptions;
using Estore.Application.Interfaces;
using Estore.Domain.Entities;
using Estore.Domain.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Estore.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, JwtSettings jwtSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtSettings = jwtSettings;
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
                throw new UnauthorizedException("Invalid email or password");

            var isValidPassword = await _unitOfWork.UserRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!isValidPassword)
                throw new UnauthorizedException("Invalid email or password");

            await _unitOfWork.UserRepository.UpdateLastLoginAsync(user.Id);

            // generate access and refresh toknne
            var accessToken = GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            await _unitOfWork.SaveChangesAsync();
            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                TokenType = "Bearer"
            };

        }
        public async Task<TokenResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new ConflictException($"User with email '{registerDto.Email}' already exists");

            var user = _mapper.Map<User>(registerDto);

            try
            {
                await _unitOfWork.UserRepository.CreateAsync(user, registerDto.Password);

                // if i want to go with auto logiin after register 
                var accessToken = GenerateAccessToken(user);
                var refreshToken = await GenerateRefreshTokenAsync(user.Id);

                await _unitOfWork.SaveChangesAsync();

                return new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                    TokenType = "Bearer"
                };
            }
            catch (InvalidOperationException ex)
            {
                throw new BadRequestException($"User registration failed: {ex.Message}");
            }
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            await _unitOfWork.RefreshTokenRepository.RevokeAllUserTokensAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            return true; 
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                throw new UnauthorizedException("Invalid or expired refresh token");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(storedToken.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            storedToken.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.RefreshTokenRepository.UpdateAsync(storedToken);

            var accessToken = GenerateAccessToken(user);
            var newRefreshToken = await GenerateRefreshTokenAsync(user.Id);

            await _unitOfWork.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                TokenType = "Bearer"
            };
        }


        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var storedToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                throw new NotFoundException("Token not found or already revoked");

            await _unitOfWork.RefreshTokenRepository.RevokeTokenAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        // utils
        private string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(randomBytes),
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow
            };

            return await _unitOfWork.RefreshTokenRepository.CreateAsync(refreshToken);
        }
    }
}
