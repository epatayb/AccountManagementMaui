using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.TaxOfficeModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxOfficesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaxOfficesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/taxoffices
    // GET: api/taxoffices?search=kadikoy
    // GET: api/taxoffices?cityId=34
    [HttpGet]
    public async Task<ActionResult<List<TaxOfficeListDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? cityId,
        CancellationToken cancellationToken)
    {
        search = search?.Trim();

        var query = _context.TaxOffices
            .AsNoTracking()
            .AsQueryable();

        if (cityId.HasValue && cityId.Value > 0)
        {
            query = query.Where(x =>
                x.CityId == cityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.TaxOfficeCode.Contains(search) ||
                x.Name.Contains(search) ||
                x.City.CityCode.Contains(search) ||
                x.City.Name.Contains(search));
        }

        var taxOffices = await query
            .OrderBy(x => x.City.CityCode)
            .ThenBy(x => x.Name)
            .Select(x => new TaxOfficeListDto
            {
                Id = x.Id,
                TaxOfficeCode = x.TaxOfficeCode,
                Name = x.Name,

                CityId = x.CityId,
                CityCode = x.City.CityCode,
                CityName = x.City.Name,

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

        return Ok(taxOffices);
    }

    // GET: api/taxoffices/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaxOfficeDetailDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var taxOffice = await GetDetailAsync(
            id,
            cancellationToken);

        if (taxOffice is null)
        {
            return NotFound(new
            {
                message = "Vergi dairesi bulunamadı."
            });
        }

        return Ok(taxOffice);
    }

    // POST: api/taxoffices
    [HttpPost]
    public async Task<ActionResult<TaxOfficeDetailDto>> Create(
        [FromBody] CreateTaxOfficeRequest request,
        CancellationToken cancellationToken)
    {
        var taxOfficeCode =
            request.TaxOfficeCode.Trim();

        var taxOfficeName =
            request.Name.Trim();

        if (string.IsNullOrWhiteSpace(taxOfficeCode))
        {
            ModelState.AddModelError(
                nameof(request.TaxOfficeCode),
                "Vergi dairesi kodu boş geçilemez.");
        }

        if (string.IsNullOrWhiteSpace(taxOfficeName))
        {
            ModelState.AddModelError(
                nameof(request.Name),
                "Vergi dairesi adı boş geçilemez.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var cityExists = await _context.Cities
            .AnyAsync(
                x => x.Id == request.CityId,
                cancellationToken);

        if (!cityExists)
        {
            return BadRequest(new
            {
                message = "Seçilen il bulunamadı."
            });
        }

        var duplicateExists = await _context.TaxOffices
            .AnyAsync(
                x =>
                    x.TaxOfficeCode == taxOfficeCode ||
                    (
                        x.CityId == request.CityId &&
                        x.Name == taxOfficeName
                    ),
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message =
                    "Bu vergi dairesi kodu veya seçilen ilde bu vergi dairesi adı kullanılıyor."
            });
        }

        var taxOffice = new TaxOffice
        {
            TaxOfficeCode = taxOfficeCode,
            Name = taxOfficeName,
            CityId = request.CityId
        };

        _context.TaxOffices.Add(taxOffice);

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
                    "Bu vergi dairesi kodu veya adı başka bir kayıtta kullanılıyor."
            });
        }

        var response = await GetDetailAsync(
            taxOffice.Id,
            cancellationToken);

        if (response is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "Vergi dairesi oluşturuldu ancak bilgileri getirilemedi."
                });
        }

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = taxOffice.Id
            },
            response);
    }

    // PUT: api/taxoffices/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaxOfficeDetailDto>> Update(
        int id,
        [FromBody] UpdateTaxOfficeRequest request,
        CancellationToken cancellationToken)
    {
        var taxOffice = await _context.TaxOffices
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (taxOffice is null)
        {
            return NotFound(new
            {
                message =
                    "Güncellenecek vergi dairesi bulunamadı."
            });
        }

        var taxOfficeCode =
            request.TaxOfficeCode.Trim();

        var taxOfficeName =
            request.Name.Trim();

        if (string.IsNullOrWhiteSpace(taxOfficeCode))
        {
            ModelState.AddModelError(
                nameof(request.TaxOfficeCode),
                "Vergi dairesi kodu boş geçilemez.");
        }

        if (string.IsNullOrWhiteSpace(taxOfficeName))
        {
            ModelState.AddModelError(
                nameof(request.Name),
                "Vergi dairesi adı boş geçilemez.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var cityExists = await _context.Cities
            .AnyAsync(
                x => x.Id == request.CityId,
                cancellationToken);

        if (!cityExists)
        {
            return BadRequest(new
            {
                message = "Seçilen il bulunamadı."
            });
        }

        var duplicateExists = await _context.TaxOffices
            .AnyAsync(
                x =>
                    x.Id != id &&
                    (
                        x.TaxOfficeCode == taxOfficeCode ||
                        (
                            x.CityId == request.CityId &&
                            x.Name == taxOfficeName
                        )
                    ),
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message =
                    "Bu vergi dairesi kodu veya seçilen ilde bu vergi dairesi adı kullanılıyor."
            });
        }

        taxOffice.TaxOfficeCode = taxOfficeCode;
        taxOffice.Name = taxOfficeName;
        taxOffice.CityId = request.CityId;

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
                    "Bu vergi dairesi kodu veya adı başka bir kayıtta kullanılıyor."
            });
        }

        var response = await GetDetailAsync(
            taxOffice.Id,
            cancellationToken);

        if (response is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "Vergi dairesi güncellendi ancak bilgileri getirilemedi."
                });
        }

        return Ok(response);
    }

    // DELETE: api/taxoffices/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        [FromBody] DeleteTaxOfficeRequest request,
        CancellationToken cancellationToken)
    {
        var taxOffice = await _context.TaxOffices
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (taxOffice is null)
        {
            return NotFound(new
            {
                message =
                    "Silinecek vergi dairesi bulunamadı."
            });
        }

        var deleteReason =
            request.DeleteReason.Trim();

        if (string.IsNullOrWhiteSpace(deleteReason))
        {
            ModelState.AddModelError(
                nameof(request.DeleteReason),
                "Silme açıklaması zorunludur.");

            return ValidationProblem(ModelState);
        }

        taxOffice.IsDeleted = true;
        taxOffice.DeleteReason = deleteReason;

        await _context.SaveChangesAsync(
            cancellationToken);

        return NoContent();
    }

    private async Task<TaxOfficeDetailDto?> GetDetailAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.TaxOffices
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new TaxOfficeDetailDto
            {
                Id = x.Id,
                TaxOfficeCode = x.TaxOfficeCode,
                Name = x.Name,

                CityId = x.CityId,
                CityCode = x.City.CityCode,
                CityName = x.City.Name,

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