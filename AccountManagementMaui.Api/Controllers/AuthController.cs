using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.AuthModels;
using AccountManagementMaui.Api.Options;
using AccountManagementMaui.Api.Services.AuthServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AccountManagementMaui.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string DefaultUserRole =
        "User";


    private readonly AppDbContext _context;

    private readonly UserManager<AppUser> _userManager;

    private readonly RoleManager<AppRole> _roleManager;

    private readonly SignInManager<AppUser> _signInManager;

    private readonly IJwtTokenService _jwtTokenService;

    private readonly JwtOptions _jwtOptions;


    public AuthController(
        AppDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        SignInManager<AppUser> signInManager,
        IJwtTokenService jwtTokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _context =
            context;

        _userManager =
            userManager;

        _roleManager =
            roleManager;

        _signInManager =
            signInManager;

        _jwtTokenService =
            jwtTokenService;

        _jwtOptions =
            jwtOptions.Value;
    }


    // =========================================================
    // REGISTER
    // POST: api/auth/register
    // =========================================================

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var firstName =
            request.FirstName.Trim();

        var lastName =
            request.LastName.Trim();

        var userName =
            request.UserName.Trim();

        var email =
            request.Email.Trim();

        var phoneNumber =
            request.PhoneNumber.Trim();


        // -----------------------------------------------------
        // USERNAME CHECK
        // -----------------------------------------------------

        var existingUserName =
            await _userManager.FindByNameAsync(
                userName);


        if (existingUserName is not null)
        {
            ModelState.AddModelError(
                nameof(request.UserName),
                "Bu kullanıcı adı zaten kullanılıyor.");
        }


        // -----------------------------------------------------
        // EMAIL CHECK
        // -----------------------------------------------------

        var existingEmail =
            await _userManager.FindByEmailAsync(
                email);


        if (existingEmail is not null)
        {
            ModelState.AddModelError(
                nameof(request.Email),
                "Bu e-posta adresi zaten kullanılıyor.");
        }


        if (!ModelState.IsValid)
        {
            return ValidationProblem(
                ModelState);
        }


        await using var transaction =
            await _context.Database
                .BeginTransactionAsync(
                    cancellationToken);


        try
        {
            // -------------------------------------------------
            // DEFAULT ROLE
            // -------------------------------------------------

            var roleResult =
                await EnsureDefaultUserRoleAsync();


            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(
                    cancellationToken);


                return BadRequest(new
                {
                    message =
                        GetIdentityErrors(roleResult)
                });
            }


            // -------------------------------------------------
            // CREATE USER
            // -------------------------------------------------

            var user =
                new AppUser
                {
                    FirstName =
                        firstName,

                    LastName =
                        lastName,

                    UserName =
                        userName,

                    Email =
                        email,

                    PhoneNumber =
                        phoneNumber,

                    IsDeleted =
                        false
                };


            var createResult =
                await _userManager.CreateAsync(
                    user,
                    request.Password);


            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync(
                    cancellationToken);


                return BadRequest(new
                {
                    message =
                        GetIdentityErrors(
                            createResult)
                });
            }


            // -------------------------------------------------
            // USER ROLE
            // -------------------------------------------------

            var addRoleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    DefaultUserRole);


            if (!addRoleResult.Succeeded)
            {
                await transaction.RollbackAsync(
                    cancellationToken);


                return BadRequest(new
                {
                    message =
                        GetIdentityErrors(
                            addRoleResult)
                });
            }


            await transaction.CommitAsync(
                cancellationToken);


            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message =
                        "Hesabınız başarıyla oluşturuldu. Giriş yapabilirsiniz."
                });
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }


    // =========================================================
    // LOGIN
    // POST: api/auth/login
    // =========================================================

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var identifier =
            request.Identifier.Trim();


        // Kullanıcı adı ile dene.
        var user =
            await _userManager.FindByNameAsync(
                identifier);


        // Bulunamazsa e-posta ile dene.
        user ??=
            await _userManager.FindByEmailAsync(
                identifier);


        // Kullanıcının varlığını dışarı açıklamıyoruz.
        if (user is null)
        {
            return Unauthorized(new
            {
                message =
                    "Kullanıcı adı/e-posta veya parola hatalı."
            });
        }


        if (user.IsDeleted)
        {
            return Unauthorized(new
            {
                message =
                    "Bu kullanıcı hesabı aktif değildir."
            });
        }


        // -----------------------------------------------------
        // PASSWORD
        // -----------------------------------------------------

        var passwordResult =
            await _signInManager
                .CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    lockoutOnFailure: true);


        if (passwordResult.IsLockedOut)
        {
            return StatusCode(
                StatusCodes.Status423Locked,
                new
                {
                    message =
                        "Çok fazla başarısız giriş denemesi yapıldı. Lütfen daha sonra tekrar deneyiniz."
                });
        }


        if (!passwordResult.Succeeded)
        {
            return Unauthorized(new
            {
                message =
                    "Kullanıcı adı/e-posta veya parola hatalı."
            });
        }


        var response =
            await CreateAuthenticationAsync(
                user,
                cancellationToken);


        return Ok(response);
    }


    // =========================================================
    // REFRESH
    // POST: api/auth/refresh
    // =========================================================

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var refreshToken =
            request.RefreshToken.Trim();


        var tokenHash =
            _jwtTokenService.HashRefreshToken(
                refreshToken);


        var storedToken =
            await _context.AppRefreshToken
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x =>
                        x.TokenHash ==
                        tokenHash,
                    cancellationToken);


        if (storedToken is null)
        {
            return Unauthorized(new
            {
                message =
                    "Oturum yenileme bilgisi geçersiz."
            });
        }


        if (storedToken.User.IsDeleted)
        {
            return Unauthorized(new
            {
                message =
                    "Bu kullanıcı hesabı aktif değildir."
            });
        }


        if (storedToken.RevokedAtUtc.HasValue)
        {
            return Unauthorized(new
            {
                message =
                    "Oturum yenileme bilgisi artık geçerli değildir."
            });
        }


        if (storedToken.ExpiresAtUtc <=
            DateTime.UtcNow)
        {
            return Unauthorized(new
            {
                message =
                    "Oturum süresi dolmuştur. Lütfen tekrar giriş yapınız."
            });
        }


        // -----------------------------------------------------
        // TOKEN ROTATION
        // -----------------------------------------------------

        var newRefreshToken =
            _jwtTokenService
                .GenerateRefreshToken();


        var newTokenHash =
            _jwtTokenService
                .HashRefreshToken(
                    newRefreshToken);


        var nowUtc =
            DateTime.UtcNow;

        var newRefreshTokenExpiresAtUtc =
            nowUtc.AddDays(
                _jwtOptions.RefreshTokenDays);


        // Eski token artık kullanılamaz.
        storedToken.RevokedAtUtc =
            nowUtc;

        storedToken.ReplacedByTokenHash =
            newTokenHash;


        var newStoredToken =
            new AppRefreshToken
            {
                UserId =
                    storedToken.UserId,

                TokenHash =
                    newTokenHash,

                CreatedAtUtc =
                    nowUtc,

                ExpiresAtUtc =
                    newRefreshTokenExpiresAtUtc
            };


        _context.AppRefreshToken.Add(
            newStoredToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        // -----------------------------------------------------
        // NEW ACCESS TOKEN
        // -----------------------------------------------------

        var accessTokenResult =
            await _jwtTokenService
                .CreateAccessTokenAsync(
                    storedToken.User);


        var userDto =
            await _jwtTokenService
                .CreateUserDtoAsync(
                    storedToken.User);


        var response =
            new AuthResponse
            {
                AccessToken =
                    accessTokenResult.Token,

                AccessTokenExpiresAtUtc =
                    accessTokenResult.ExpiresAtUtc,

                RefreshToken =
                    newRefreshToken,

                RefreshTokenExpiresAtUtc =
                    newRefreshTokenExpiresAtUtc,

                User =
                    userDto
            };


        return Ok(response);
    }


    // =========================================================
    // LOGOUT
    // POST: api/auth/logout
    // =========================================================

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var refreshToken =
            request.RefreshToken.Trim();


        var tokenHash =
            _jwtTokenService.HashRefreshToken(
                refreshToken);


        var storedToken =
            await _context.AppRefreshToken
                .FirstOrDefaultAsync(
                    x =>
                        x.TokenHash ==
                        tokenHash,
                    cancellationToken);


        if (storedToken is null)
        {
            // Logout idempotent davranabilir.
            // Token zaten yoksa kullanıcı zaten çıkmış kabul edilir.
            return NoContent();
        }


        if (!storedToken.RevokedAtUtc.HasValue)
        {
            storedToken.RevokedAtUtc =
                DateTime.UtcNow;


            await _context.SaveChangesAsync(
                cancellationToken);
        }


        return NoContent();
    }


    // =========================================================
    // CREATE AUTHENTICATION
    // =========================================================

    private async Task<AuthResponse>
        CreateAuthenticationAsync(
            AppUser user,
            CancellationToken cancellationToken)
    {
        var accessTokenResult =
            await _jwtTokenService
                .CreateAccessTokenAsync(
                    user);


        var refreshToken =
            _jwtTokenService
                .GenerateRefreshToken();


        var refreshTokenHash =
            _jwtTokenService
                .HashRefreshToken(
                    refreshToken);


        var nowUtc =
            DateTime.UtcNow;


        var refreshExpiresAtUtc =
            nowUtc.AddDays(
                _jwtOptions.RefreshTokenDays);


        var storedToken =
            new AppRefreshToken
            {
                UserId =
                    user.Id,

                TokenHash =
                    refreshTokenHash,

                CreatedAtUtc =
                    nowUtc,

                ExpiresAtUtc =
                    refreshExpiresAtUtc
            };


        _context.AppRefreshToken.Add(
            storedToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        var userDto =
            await _jwtTokenService
                .CreateUserDtoAsync(
                    user);


        return new AuthResponse
        {
            AccessToken =
                accessTokenResult.Token,

            AccessTokenExpiresAtUtc =
                accessTokenResult.ExpiresAtUtc,

            RefreshToken =
                refreshToken,

            RefreshTokenExpiresAtUtc =
                refreshExpiresAtUtc,

            User =
                userDto
        };
    }


    // =========================================================
    // DEFAULT ROLE
    // =========================================================

    private async Task<IdentityResult>
        EnsureDefaultUserRoleAsync()
    {
        var exists =
            await _roleManager.RoleExistsAsync(
                DefaultUserRole);


        if (exists)
        {
            return IdentityResult.Success;
        }


        var role =
            new AppRole
            {
                Name =
                    DefaultUserRole
            };


        return await _roleManager.CreateAsync(
            role);
    }


    // =========================================================
    // IDENTITY ERRORS
    // =========================================================

    private static string GetIdentityErrors(
        IdentityResult result)
    {
        var messages =
            result.Errors
                .Select(MapIdentityError)
                .Distinct()
                .ToList();


        return messages.Count == 0
            ? "İşlem tamamlanamadı."
            : string.Join(
                " ",
                messages);
    }


    private static string MapIdentityError(
        IdentityError error)
    {
        return error.Code switch
        {
            "DuplicateUserName" =>
                "Bu kullanıcı adı zaten kullanılıyor.",

            "DuplicateEmail" =>
                "Bu e-posta adresi zaten kullanılıyor.",

            "InvalidUserName" =>
                "Kullanıcı adı geçersiz.",

            "InvalidEmail" =>
                "E-posta adresi geçersiz.",

            "PasswordTooShort" =>
                "Parola gerekli minimum uzunluğu karşılamıyor.",

            "PasswordRequiresDigit" =>
                "Parola en az bir rakam içermelidir.",

            "PasswordRequiresLower" =>
                "Parola en az bir küçük harf içermelidir.",

            "PasswordRequiresUpper" =>
                "Parola en az bir büyük harf içermelidir.",

            "PasswordRequiresNonAlphanumeric" =>
                "Parola en az bir özel karakter içermelidir.",

            _ =>
                error.Description
        };
    }
}