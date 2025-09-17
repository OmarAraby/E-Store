using Estore.Application.Common;
using Estore.Application.Common.GeneralResult;
using Estore.Application.DTOS.Auth;
using Estore.Application.Exceptions;
using Estore.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Estore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<RefreshTokenDto> _refreshTokenValidator;


        public AuthController(IAuthService authService, IValidator<LoginDto> loginValiadtor, IValidator<RegisterDto> registerValidator, IValidator<RefreshTokenDto> refreshTokenValidator)
        {
            _authService = authService;
            _loginValidator = loginValiadtor;
            _registerValidator = registerValidator;
            _refreshTokenValidator = refreshTokenValidator;
        }

        // register
        [HttpPost("register")]
        public async Task<Results<Ok<ApiResponse<TokenResponseDto>>, BadRequest<ApiResponse<object>>, Conflict<ApiResponse<object>>>> Register([FromBody] RegisterDto registerDto)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
            }

            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return TypedResults.Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result, "Registration successful"));
            }
            catch (ConflictException ex)
            {
                return TypedResults.Conflict(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        // login
        [HttpPost("login")]
        public async Task<Results<Ok<ApiResponse<TokenResponseDto>>,BadRequest<ApiResponse<object>>,UnauthorizedHttpResult>> Login([FromBody] LoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
            }
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return TypedResults.Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result, "Login successful"));
            }
            catch (UnauthorizedException)
            {
                return TypedResults.Unauthorized();
            }
        }

        [HttpPost("refresh-token")]
        public async Task<Results<Ok<ApiResponse<TokenResponseDto>>, BadRequest<ApiResponse<object>>, UnauthorizedHttpResult, NotFound<ApiResponse<object>>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var validationResult = await _refreshTokenValidator.ValidateAsync(refreshTokenDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
            }

            try
            {
                var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
                return TypedResults.Ok(ApiResponse<TokenResponseDto>.SuccessResponse(result, "Token refreshed successfully"));
            }
            catch (UnauthorizedException)
            {
                return TypedResults.Unauthorized();
            }
            catch (NotFoundException ex)
            {
                return TypedResults.NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<Results<Ok<ApiResponse<object>>, BadRequest<ApiResponse<object>>>> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse("Invalid user context"));
            }

            await _authService.LogoutAsync(userId);
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse("Logged out successfully"));
        }



    }
}
