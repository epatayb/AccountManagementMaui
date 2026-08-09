using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.AccountCardSubGroupModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AccountManagementMaui.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountCardSubGroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccountCardSubGroupsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountCardSubGroupListDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? accountCardGroupId,
        CancellationToken cancellationToken)
    {
        search = search?.Trim();

        var query = _context.AccountCardSubGroups
            .AsNoTracking()
            .AsQueryable();

        if (accountCardGroupId.HasValue &&
            accountCardGroupId.Value > 0)
        {
            query = query.Where(x =>
                x.AccountCardGroupId ==
                accountCardGroupId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.SubGroupName.Contains(search) ||
                x.AccountCardGroup.GroupName.Contains(search));
        }

        var items = await query
            .OrderBy(x => x.AccountCardGroup.GroupName)
            .ThenBy(x => x.SubGroupName)
            .Select(x => new AccountCardSubGroupListDto
            {
                Id = x.Id,

                SubGroupName = x.SubGroupName,

                AccountCardGroupId =
                    x.AccountCardGroupId,

                AccountCardGroupName =
                    x.AccountCardGroup.GroupName,

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
    public async Task<ActionResult<AccountCardSubGroupDetailDto>> GetById(
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
                message = "Hesap kart alt grubu bulunamadı."
            });
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<AccountCardSubGroupDetailDto>> Create(
        [FromBody] CreateAccountCardSubGroupRequest request,
        CancellationToken cancellationToken)
    {
        var subGroupName =
            request.SubGroupName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(subGroupName))
        {
            ModelState.AddModelError(
                nameof(request.SubGroupName),
                "Alt grup adı boş geçilemez.");

            return ValidationProblem(ModelState);
        }

        var groupExists =
            await _context.AccountCardGroups.AnyAsync(
                x => x.Id == request.AccountCardGroupId,
                cancellationToken);

        if (!groupExists)
        {
            return BadRequest(new
            {
                message =
                    "Seçilen hesap kart grubu bulunamadı."
            });
        }

        var duplicateExists =
            await _context.AccountCardSubGroups.AnyAsync(
                x =>
                    x.AccountCardGroupId ==
                    request.AccountCardGroupId &&
                    x.SubGroupName == subGroupName,
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message =
                    "Seçilen grupta bu alt grup adı kullanılıyor."
            });
        }

        var item = new AccountCardSubGroup
        {
            AccountCardGroupId =
                request.AccountCardGroupId,

            SubGroupName =
                subGroupName
        };

        _context.AccountCardSubGroups.Add(item);

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
                    "Seçilen grupta bu alt grup adı başka bir kayıtta kullanılıyor."
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
                        "Hesap kart alt grubu oluşturuldu ancak bilgileri getirilemedi."
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
    public async Task<ActionResult<AccountCardSubGroupDetailDto>> Update(
        int id,
        [FromBody] UpdateAccountCardSubGroupRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _context.AccountCardSubGroups
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (item is null)
        {
            return NotFound(new
            {
                message =
                    "Güncellenecek hesap kart alt grubu bulunamadı."
            });
        }

        var subGroupName =
            request.SubGroupName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(subGroupName))
        {
            ModelState.AddModelError(
                nameof(request.SubGroupName),
                "Alt grup adı boş geçilemez.");

            return ValidationProblem(ModelState);
        }

        var groupExists =
            await _context.AccountCardGroups.AnyAsync(
                x => x.Id == request.AccountCardGroupId,
                cancellationToken);

        if (!groupExists)
        {
            return BadRequest(new
            {
                message =
                    "Seçilen hesap kart grubu bulunamadı."
            });
        }

        var duplicateExists =
            await _context.AccountCardSubGroups.AnyAsync(
                x =>
                    x.Id != id &&
                    x.AccountCardGroupId ==
                    request.AccountCardGroupId &&
                    x.SubGroupName == subGroupName,
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message =
                    "Seçilen grupta bu alt grup adı kullanılıyor."
            });
        }

        item.AccountCardGroupId =
            request.AccountCardGroupId;

        item.SubGroupName =
            subGroupName;

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
                    "Seçilen grupta bu alt grup adı başka bir kayıtta kullanılıyor."
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
                        "Hesap kart alt grubu güncellendi ancak bilgileri getirilemedi."
                });
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        [FromBody] DeleteAccountCardSubGroupRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _context.AccountCardSubGroups
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (item is null)
        {
            return NotFound(new
            {
                message =
                    "Silinecek hesap kart alt grubu bulunamadı."
            });
        }

        var hasSubGroups =
            await _context.AccountCardSubGroups.AnyAsync(
                x => x.AccountCardGroupId == id,
                cancellationToken);

        if (hasSubGroups)
        {
            return Conflict(new
            {
                message =
                    "Bu gruba bağlı alt gruplar bulunduğu için kayıt silinemez."
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

    private async Task<AccountCardSubGroupDetailDto?> GetDetailAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.AccountCardSubGroups
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AccountCardSubGroupDetailDto
            {
                Id = x.Id,

                SubGroupName =
                    x.SubGroupName,

                AccountCardGroupId =
                    x.AccountCardGroupId,

                AccountCardGroupName =
                    x.AccountCardGroup.GroupName,

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
}