using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.AuthModels;

namespace AccountManagementMaui.Api.Services.AuthServices;

public interface IJwtTokenService
{
    Task<(string Token, DateTime ExpiresAtUtc)>
        CreateAccessTokenAsync(AppUser user);

    Task<AuthUserDto> CreateUserDtoAsync(
        AppUser user);

    string GenerateRefreshToken();

    string HashRefreshToken(
        string refreshToken);
}