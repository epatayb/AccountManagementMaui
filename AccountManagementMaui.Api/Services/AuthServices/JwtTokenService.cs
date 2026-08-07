using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.AuthModels;
using AccountManagementMaui.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AccountManagementMaui.Api.Services.AuthServices;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;

    private readonly UserManager<AppUser> _userManager;


    public JwtTokenService(
        IOptions<JwtOptions> jwtOptions,
        UserManager<AppUser> userManager)
    {
        _jwtOptions =
            jwtOptions.Value;

        _userManager =
            userManager;
    }


    // =========================================================
    // ACCESS TOKEN
    // =========================================================

    public async Task<(string Token, DateTime ExpiresAtUtc)>
        CreateAccessTokenAsync(
            AppUser user)
    {
        var roles =
            await _userManager.GetRolesAsync(user);


        var nowUtc =
            DateTime.UtcNow;

        var expiresAtUtc =
            nowUtc.AddMinutes(
                _jwtOptions.AccessTokenMinutes);


        var claims =
            new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString(
                        CultureInfo.InvariantCulture)),

                new(
                    ClaimTypes.Name,
                    user.UserName ?? string.Empty),

                new(
                    ClaimTypes.Email,
                    user.Email ?? string.Empty),

                new(
                    ClaimTypes.GivenName,
                    user.FirstName),

                new(
                    ClaimTypes.Surname,
                    user.LastName),

                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString("N"))
            };


        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }


        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _jwtOptions.Key));


        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);


        var token =
            new JwtSecurityToken(
                issuer:
                    _jwtOptions.Issuer,

                audience:
                    _jwtOptions.Audience,

                claims:
                    claims,

                notBefore:
                    nowUtc,

                expires:
                    expiresAtUtc,

                signingCredentials:
                    credentials);


        var tokenValue =
            new JwtSecurityTokenHandler()
                .WriteToken(token);


        return (
            tokenValue,
            expiresAtUtc);
    }


    // =========================================================
    // AUTH USER DTO
    // =========================================================

    public async Task<AuthUserDto>
        CreateUserDtoAsync(
            AppUser user)
    {
        var roles =
            await _userManager.GetRolesAsync(user);


        return new AuthUserDto
        {
            Id =
                user.Id,

            FirstName =
                user.FirstName,

            LastName =
                user.LastName,

            FullName =
                user.FullName,

            UserName =
                user.UserName ?? string.Empty,

            Email =
                user.Email ?? string.Empty,

            PhoneNumber =
                user.PhoneNumber,

            Roles =
                roles.ToList()
        };
    }


    // =========================================================
    // REFRESH TOKEN
    // =========================================================

    public string GenerateRefreshToken()
    {
        var randomBytes =
            RandomNumberGenerator.GetBytes(64);


        return Base64UrlEncoder.Encode(
            randomBytes);
    }


    // =========================================================
    // REFRESH TOKEN HASH
    // =========================================================

    public string HashRefreshToken(
        string refreshToken)
    {
        var bytes =
            Encoding.UTF8.GetBytes(
                refreshToken);


        var hash =
            SHA256.HashData(bytes);


        return Convert.ToHexString(hash);
    }
}