using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.UserModels;
using AccountManagementMaui.Api.Services.CurrentUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AccountManagementMaui.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(
        AppDbContext context,
        UserManager<AppUser> userManager,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserListDto>>> GetAll(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        search = search?.Trim();

        var query = _context.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.FirstName.Contains(search) ||
                x.LastName.Contains(search) ||
                (x.FirstName + " " + x.LastName).Contains(search) ||
                (x.UserName != null &&
                 x.UserName.Contains(search)) ||
                (x.Email != null &&
                 x.Email.Contains(search)) ||
                (x.PhoneNumber != null &&
                 x.PhoneNumber.Contains(search)));
        }

        var users = await query
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Select(x => new UserListDto
            {
                Id = x.Id,

                FullName =
                    x.FirstName + " " + x.LastName,

                UserName =
                    x.UserName ?? string.Empty,

                Email =
                    x.Email ?? string.Empty,

                PhoneNumber =
                    x.PhoneNumber,

                CreatedDate =
                    x.CreatedDate,

                CreatedByUserId =
                    x.CreatedByUserId,

                CreatedByUserFullName =
                    x.CreatedByUser == null
                        ? null
                        : x.CreatedByUser.FirstName +
                          " " +
                          x.CreatedByUser.LastName,

                ModifiedDate =
                    x.ModifiedDate,

                ModifiedByUserId =
                    x.ModifiedByUserId,

                ModifiedByUserFullName =
                    x.ModifiedByUser == null
                        ? null
                        : x.ModifiedByUser.FirstName +
                          " " +
                          x.ModifiedByUser.LastName
            })
            .ToListAsync(cancellationToken);

        return Ok(users);
    }
 
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDetailDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var user = await GetUserDetailAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            return NotFound(new
            {
                message = "Kullanıcı bulunamadı."
            });
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDetailDto>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var userName = request.UserName.Trim();
        var email = request.Email.Trim();
        var phoneNumber = request.PhoneNumber.Trim();

        var existingUserName =
            await _userManager.FindByNameAsync(userName);

        if (existingUserName is not null)
        {
            return Conflict(new
            {
                message = "Bu kullanıcı adı kullanılıyor."
            });
        }

        var existingEmail =
            await _userManager.FindByEmailAsync(email);

        if (existingEmail is not null)
        {
            return Conflict(new
            {
                message = "Bu e-posta adresi kullanılıyor."
            });
        }

        var user = new AppUser
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = userName,
            Email = email,
            PhoneNumber = phoneNumber,

            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            LockoutEnabled = true
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            AddIdentityErrors(result);

            return ValidationProblem(ModelState);
        }

        var response = await GetUserDetailAsync(
            user.Id,
            cancellationToken);

        if (response is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "Kullanıcı oluşturuldu ancak kullanıcı bilgileri getirilemedi."
                });
        }

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = user.Id
            },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDetailDto>> Update(
        int id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     !x.IsDeleted,
                cancellationToken);

        if (user is null)
        {
            return NotFound(new
            {
                message = "Güncellenecek kullanıcı bulunamadı."
            });
        }

        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var userName = request.UserName.Trim();
        var email = request.Email.Trim();
        var phoneNumber = request.PhoneNumber.Trim();

        var existingUserName =
            await _userManager.FindByNameAsync(userName);

        if (existingUserName is not null &&
            existingUserName.Id != id)
        {
            return Conflict(new
            {
                message = "Bu kullanıcı adı kullanılıyor."
            });
        }

        var existingEmail =
            await _userManager.FindByEmailAsync(email);

        if (existingEmail is not null &&
            existingEmail.Id != id)
        {
            return Conflict(new
            {
                message = "Bu e-posta adresi kullanılıyor."
            });
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            user.FirstName = firstName;
            user.LastName = lastName;
            user.UserName = userName;
            user.Email = email;
            user.PhoneNumber = phoneNumber;

            var updateResult =
                await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                AddIdentityErrors(updateResult);

                return ValidationProblem(ModelState);
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var resetToken =
                    await _userManager
                        .GeneratePasswordResetTokenAsync(user);

                var passwordResult =
                    await _userManager.ResetPasswordAsync(
                        user,
                        resetToken,
                        request.Password);

                if (!passwordResult.Succeeded)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    AddIdentityErrors(passwordResult);

                    return ValidationProblem(ModelState);
                }
            }

            await transaction.CommitAsync(
                cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }

        var response = await GetUserDetailAsync(
            user.Id,
            cancellationToken);

        if (response is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "Kullanıcı güncellendi ancak kullanıcı bilgileri getirilemedi."
                });
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        [FromBody] DeleteUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     !x.IsDeleted,
                cancellationToken);

        if (user is null)
        {
            return NotFound(new
            {
                message = "Silinecek kullanıcı bulunamadı."
            });
        }

        if (_currentUserService.UserId == id)
        {
            return BadRequest(new
            {
                message =
                    "Oturum açtığınız kullanıcı hesabını silemezsiniz."
            });
        }

        var deleteReason = request.DeleteReason.Trim();

        if (string.IsNullOrWhiteSpace(deleteReason))
        {
            ModelState.AddModelError(
                nameof(request.DeleteReason),
                "Silme açıklaması zorunludur.");

            return ValidationProblem(ModelState);
        }

        user.IsDeleted = true;
        user.DeleteReason = deleteReason;

        // Kullanıcının sisteme giriş yapmasını engeller.
        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;

        var result =
            await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            AddIdentityErrors(result);

            return ValidationProblem(ModelState);
        }

        return NoContent();
    }

    private async Task<UserDetailDto?> GetUserDetailAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x =>
                x.Id == id &&
                !x.IsDeleted)
            .Select(x => new UserDetailDto
            {
                Id = x.Id,

                FirstName =
                    x.FirstName,

                LastName =
                    x.LastName,

                FullName =
                    x.FirstName + " " + x.LastName,

                UserName =
                    x.UserName ?? string.Empty,

                Email =
                    x.Email ?? string.Empty,

                PhoneNumber =
                    x.PhoneNumber,

                CreatedDate =
                    x.CreatedDate,

                CreatedByUserId =
                    x.CreatedByUserId,

                CreatedByUserFullName =
                    x.CreatedByUser == null
                        ? null
                        : x.CreatedByUser.FirstName +
                          " " +
                          x.CreatedByUser.LastName,

                ModifiedDate =
                    x.ModifiedDate,

                ModifiedByUserId =
                    x.ModifiedByUserId,

                ModifiedByUserFullName =
                    x.ModifiedByUser == null
                        ? null
                        : x.ModifiedByUser.FirstName +
                          " " +
                          x.ModifiedByUser.LastName
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private void AddIdentityErrors(
        IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(
                error.Code,
                GetIdentityErrorMessage(error));
        }
    }

    private static string GetIdentityErrorMessage(
        IdentityError error)
    {
        return error.Code switch
        {
            "DuplicateUserName" =>
                "Bu kullanıcı adı kullanılıyor.",

            "DuplicateEmail" =>
                "Bu e-posta adresi kullanılıyor.",

            "InvalidUserName" =>
                "Kullanıcı adı geçerli değil.",

            "InvalidEmail" =>
                "E-posta adresi geçerli değil.",

            "PasswordTooShort" =>
                "Parola yeterince uzun değil.",

            "PasswordRequiresDigit" =>
                "Parola en az bir rakam içermelidir.",

            "PasswordRequiresLower" =>
                "Parola en az bir küçük harf içermelidir.",

            "PasswordRequiresUpper" =>
                "Parola en az bir büyük harf içermelidir.",

            "PasswordRequiresNonAlphanumeric" =>
                "Parola en az bir özel karakter içermelidir.",

            "PasswordRequiresUniqueChars" =>
                "Parola yeterli sayıda farklı karakter içermelidir.",

            "PasswordMismatch" =>
                "Parola değiştirilemedi.",

            _ => error.Description
        };
    }
}