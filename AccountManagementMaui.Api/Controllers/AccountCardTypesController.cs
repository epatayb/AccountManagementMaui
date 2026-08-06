using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.AccountCardTypeModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountCardTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccountCardTypesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountCardTypeListDto>>> GetAll(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        search = search?.Trim();

        var query = _context.AccountCardTypes
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.TypeCode.Contains(search) ||
                x.TypeName.Contains(search));
        }

        var items = await query
            .OrderBy(x => x.TypeCode)
            .Select(x => new AccountCardTypeListDto
            {
                Id = x.Id,
                TypeCode = x.TypeCode,
                TypeName = x.TypeName,

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
    public async Task<ActionResult<AccountCardTypeDetailDto>> GetById(
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
                message = "Ana hesap tipi bulunamadı."
            });
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<AccountCardTypeDetailDto>> Create(
        [FromBody] CreateAccountCardTypeRequest request,
        CancellationToken cancellationToken)
    {
        var typeCode = request.TypeCode
            .Trim()
            .ToUpperInvariant();

        var typeName = request.TypeName.Trim();

        if (string.IsNullOrWhiteSpace(typeCode))
        {
            ModelState.AddModelError(
                nameof(request.TypeCode),
                "Kart tip kodu boş geçilemez.");
        }

        if (string.IsNullOrWhiteSpace(typeName))
        {
            ModelState.AddModelError(
                nameof(request.TypeName),
                "Kart tip adı boş geçilemez.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var duplicateExists = await _context.AccountCardTypes
            .AnyAsync(
                x => x.TypeCode == typeCode ||
                     x.TypeName == typeName,
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message = "Bu ana hesap tip kodu veya ana hesap tip adı kullanılıyor."
            });
        }

        var item = new AccountCardType
        {
            TypeCode = typeCode,
            TypeName = typeName
        };

        _context.AccountCardTypes.Add(item);

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Bu ana hesap tip kodu veya ana hesap tip adı kullanılıyor."
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
                        "Kart tipi oluşturuldu ancak bilgileri getirilemedi."
                });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = item.Id },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AccountCardTypeDetailDto>> Update(
        int id,
        [FromBody] UpdateAccountCardTypeRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _context.AccountCardTypes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (item is null)
        {
            return NotFound(new
            {
                message =
                    "Güncellenecek hesap kart tipi bulunamadı."
            });
        }

        var typeCode = request.TypeCode
            .Trim()
            .ToUpperInvariant();

        var typeName = request.TypeName.Trim();

        if (string.IsNullOrWhiteSpace(typeCode))
        {
            ModelState.AddModelError(
                nameof(request.TypeCode),
                "Kart tip kodu boş geçilemez.");
        }

        if (string.IsNullOrWhiteSpace(typeName))
        {
            ModelState.AddModelError(
                nameof(request.TypeName),
                "Kart tip adı boş geçilemez.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var duplicateExists = await _context.AccountCardTypes
            .AnyAsync(
                x => x.Id != id &&
                     (
                         x.TypeCode == typeCode ||
                         x.TypeName == typeName
                     ),
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message = "Bu ana hesap tip kodu veya ana hesap tip adı kullanılıyor."
            });
        }

        item.TypeCode = typeCode;
        item.TypeName = typeName;

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Bu ana hesap tip kodu veya ana hesap tip adı kullanılıyor."
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
                        "Kart tipi güncellendi ancak bilgileri getirilemedi."
                });
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        [FromBody] DeleteAccountCardTypeRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _context.AccountCardTypes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (item is null)
        {
            return NotFound(new
            {
                message =
                    "Silinecek hesap kart tipi bulunamadı."
            });
        }

        var hasAccountCardKinds = await _context.AccountCardKinds.AnyAsync(
            x => x.AccountCardTypeId == id, cancellationToken);

        if (hasAccountCardKinds)
        {
            return Conflict(new
            {
                message = "Bu ana hesap tipine bağlı hesap kart türleri bulunduğu için kayıt silinemez."
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

        item.IsDeleted = true;
        item.DeleteReason = deleteReason;

        await _context.SaveChangesAsync(
            cancellationToken);

        return NoContent();
    }

    private async Task<AccountCardTypeDetailDto?> GetDetailAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.AccountCardTypes
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AccountCardTypeDetailDto
            {
                Id = x.Id,
                TypeCode = x.TypeCode,
                TypeName = x.TypeName,

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