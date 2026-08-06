using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.DistrictModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DistrictsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DistrictsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<DistrictListDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? cityId,
        CancellationToken cancellationToken)
    {
        search = search?.Trim();

        var query = _context.Districts
            .AsNoTracking()
            .AsQueryable();

        if (cityId.HasValue && cityId.Value > 0)
        {
            query = query.Where(x => x.CityId == cityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.DistrictCode.Contains(search) ||
                x.Name.Contains(search) ||
                x.City.CityCode.Contains(search) ||
                x.City.Name.Contains(search));
        }

        var districts = await query
            .OrderBy(x => x.City.CityCode)
            .ThenBy(x => x.Name)
            .Select(x => new DistrictListDto
            {
                Id = x.Id,
                DistrictCode = x.DistrictCode,
                Name = x.Name,
                CityId = x.CityId,
                CityCode = x.City.CityCode,
                CityName = x.City.Name
            })
            .ToListAsync(cancellationToken);

        return Ok(districts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DistrictDetailDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var district = await _context.Districts
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DistrictDetailDto
            {
                Id = x.Id,
                DistrictCode = x.DistrictCode,
                Name = x.Name,
                CityId = x.CityId,
                CityCode = x.City.CityCode,
                CityName = x.City.Name,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (district is null)
        {
            return NotFound(new
            {
                message = "İlçe bulunamadı."
            });
        }

        return Ok(district);
    }

    [HttpPost]
    public async Task<ActionResult<DistrictDetailDto>> Create(
        [FromBody] CreateDistrictRequest request,
        CancellationToken cancellationToken)
    {
        var districtCode = request.DistrictCode
            .Trim();

        var districtName = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(districtCode))
        {
            ModelState.AddModelError(
                nameof(request.DistrictCode),
                "İlçe kodu zorunludur.");

            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(districtName))
        {
            ModelState.AddModelError(
                nameof(request.Name),
                "İlçe adı zorunludur.");

            return ValidationProblem(ModelState);
        }

        var city = await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.CityId,
                cancellationToken);

        if (city is null)
        {
            return BadRequest(new
            {
                message = "Seçilen il bulunamadı."
            });
        }

        var duplicateExists = await _context.Districts
            .AnyAsync(
                x => x.DistrictCode == districtCode ||
                     (
                         x.CityId == request.CityId &&
                         x.Name == districtName
                     ),
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message = "Bu ilçe kodu kullanılıyor veya seçilen ilde aynı isimde bir ilçe bulunuyor."
            });
        }

        var district = new District
        {
            DistrictCode = districtCode,
            Name = districtName,
            CityId = request.CityId
        };

        _context.Districts.Add(district);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "İlçe eklenirken bir hata oluştu. Lütfen tekrar deneyin."
            });
        }

        var response = new DistrictDetailDto
        {
            Id = district.Id,
            DistrictCode = district.DistrictCode,
            Name = district.Name,
            CityId = city.Id,
            CityCode = city.CityCode,
            CityName = city.Name,
            CreatedDate = district.CreatedDate,
            ModifiedDate = district.ModifiedDate
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = district.Id },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DistrictDetailDto>> Update(
        int id,
        [FromBody] UpdateDistrictRequest request,
        CancellationToken cancellationToken)
    {
        var district = await _context.Districts
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (district is null)
        {
            return NotFound(new
            {
                message = "Güncellenecek ilçe bulunamadı."
            });
        }

        var districtCode = request.DistrictCode.Trim();
        var districtName = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(districtCode))
        {
            ModelState.AddModelError(
                nameof(request.DistrictCode),
                "İlçe kodu zorunludur.");

            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(districtName))
        {
            ModelState.AddModelError(
                nameof(request.Name),
                "İlçe adı zorunludur.");

            return ValidationProblem(ModelState);
        }

        var city = await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.CityId,
                cancellationToken);

        if (city is null)
        {
            return BadRequest(new
            {
                message = "Seçilen il bulunamadı."
            });
        }

        var duplicateExists = await _context.Districts
            .AnyAsync(
                x => x.Id != id &&
                     (
                         x.DistrictCode == districtCode ||
                         (
                             x.CityId == request.CityId &&
                             x.Name == districtName
                         )
                     ),
                cancellationToken);

        if (duplicateExists)
        {
            return Conflict(new
            {
                message = "Bu ilçe kodu kullanılıyor veya seçilen ilde aynı isimde bir ilçe bulunuyor."
            });
        }

        district.DistrictCode = districtCode;
        district.Name = districtName;
        district.CityId = request.CityId;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "İlçe güncellenirken bir hata oluştu. Lütfen tekrar deneyin.."
            });
        }

        var response = new DistrictDetailDto
        {
            Id = district.Id,
            DistrictCode = district.DistrictCode,
            Name = district.Name,
            CityId = city.Id,
            CityCode = city.CityCode,
            CityName = city.Name,
            CreatedDate = district.CreatedDate,
            ModifiedDate = district.ModifiedDate
        };

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        [FromBody] DeleteDistrictRequest request,
        CancellationToken cancellationToken)
    {
        var district = await _context.Districts
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (district is null)
        {
            return NotFound(new
            {
                message = "Silinecek ilçe bulunamadı."
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

        district.IsDeleted = true;
        district.DeleteReason = deleteReason;

        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}