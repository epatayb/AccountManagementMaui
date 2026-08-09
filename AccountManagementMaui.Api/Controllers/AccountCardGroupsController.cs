using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.AccountCardGroupModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AccountManagementMaui.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountCardGroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccountCardGroupsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountCardGroupListDto>>> GetAll(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        search = search?.Trim();

        var query = _context.AccountCardGroups
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.GroupName.Contains(search));
        }

        var items = await query
            .OrderBy(x => x.GroupName)
            .Select(x => new AccountCardGroupListDto
            {
                Id = x.Id,

                GroupName = x.GroupName,

                CreatedDate = x.CreatedDate,
                CreatedByUserId = x.CreatedByUserId,

                CreatedByUserFullName =
                    x.CreatedByUser == null
                        ? null
                        : x.CreatedByUser.FirstName +
                          " " +
                          x.CreatedByUser.LastName,

                ModifiedDate = x.ModifiedDate,
                ModifiedByUserId = x.ModifiedByUserId,

                ModifiedByUserFullName =
                    x.ModifiedByUser == null
                        ? null
                        : x.ModifiedByUser.FirstName +
                          " " +
                          x.ModifiedByUser.LastName
            })
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccountCardGroupDetailDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var item = await GetDetailAsync(
            id,
            cancellationToken);

        if (item is null)
        {
            return NotFound(new
            {
                message = "Hesap kart grubu bulunamadı."
            });
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<AccountCardGroupDetailDto>> Create(
        [FromBody] CreateAccountCardGroupRequest request,
        CancellationToken cancellationToken)
    {
        var groupName =
            request.GroupName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(groupName))
        {
            ModelState.AddModelError(
                nameof(request.GroupName),
                "Hesap kart grup adı boş geçilemez.");

            return ValidationProblem(ModelState);
        }

        var duplicateExists =
            await _context.AccountCardGroups.AnyAsync(
                x => x.GroupName == groupName,
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message =
                    "Bu hesap kart grup adı kullanılıyor."
            });
        }

        var item = new AccountCardGroup
        {
            GroupName = groupName
        };

        _context.AccountCardGroups.Add(item);

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message =
                    "Bu hesap kart grup adı başka bir kayıtta kullanılıyor."
            });
        }

        var response = await GetDetailAsync(
            item.Id,
            cancellationToken);

        if (response is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "Hesap kart grubu oluşturuldu ancak bilgileri getirilemedi."
                });
        }

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = item.Id
            },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AccountCardGroupDetailDto>> Update(
        int id,
        [FromBody] UpdateAccountCardGroupRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _context.AccountCardGroups
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (item is null)
        {
            return NotFound(new
            {
                message =
                    "Güncellenecek hesap kart grubu bulunamadı."
            });
        }

        var groupName =
            request.GroupName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(groupName))
        {
            ModelState.AddModelError(
                nameof(request.GroupName),
                "Hesap kart grup adı boş geçilemez.");

            return ValidationProblem(ModelState);
        }

        var duplicateExists =
            await _context.AccountCardGroups.AnyAsync(
                x =>
                    x.Id != id &&
                    x.GroupName == groupName,
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message =
                    "Bu hesap kart grup adı kullanılıyor."
            });
        }

        item.GroupName = groupName;

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message =
                    "Bu hesap kart grup adı başka bir kayıtta kullanılıyor."
            });
        }

        var response = await GetDetailAsync(
            item.Id,
            cancellationToken);

        if (response is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "Hesap kart grubu güncellendi ancak bilgileri getirilemedi."
                });
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        [FromBody] DeleteAccountCardGroupRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _context.AccountCardGroups
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (item is null)
        {
            return NotFound(new
            {
                message =
                    "Silinecek hesap kart grubu bulunamadı."
            });
        }

        var deleteReason =
            request.DeleteReason?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(deleteReason))
        {
            ModelState.AddModelError(
                nameof(request.DeleteReason),
                "Silme açıklaması zorunludur.");

            return ValidationProblem(ModelState);
        }

        item.IsDeleted = true;
        item.DeleteReason = deleteReason;

        await _context.SaveChangesAsync(
            cancellationToken);

        return NoContent();
    }

    private async Task<AccountCardGroupDetailDto?> GetDetailAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.AccountCardGroups
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AccountCardGroupDetailDto
            {
                Id = x.Id,

                GroupName = x.GroupName,

                CreatedDate = x.CreatedDate,
                CreatedByUserId = x.CreatedByUserId,

                CreatedByUserFullName =
                    x.CreatedByUser == null
                        ? null
                        : x.CreatedByUser.FirstName +
                          " " +
                          x.CreatedByUser.LastName,

                ModifiedDate = x.ModifiedDate,
                ModifiedByUserId = x.ModifiedByUserId,

                ModifiedByUserFullName =
                    x.ModifiedByUser == null
                        ? null
                        : x.ModifiedByUser.FirstName +
                          " " +
                          x.ModifiedByUser.LastName
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}