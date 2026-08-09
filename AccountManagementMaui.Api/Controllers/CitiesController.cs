using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.CityModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AccountManagementMaui.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CitiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CityListDto>>> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
        {
            search = search?.Trim();

            var query = _context.Cities
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => 
                    x.Name.Contains(search) ||
                    x.CityCode.Contains(search));
            }

            var cities = await query
                .OrderBy(x => x.CityCode)
                .Select(x => new CityListDto
                {
                    Id = x.Id,
                    CityCode = x.CityCode,
                    Name = x.Name,
                    DistrictCount = x.Districts.Count
                })
                .ToListAsync(cancellationToken);

            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDetailDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var city = await _context.Cities
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CityDetailDto
                {
                    Id = x.Id,
                    CityCode = x.CityCode,
                    Name = x.Name,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,

                    Districts = x.Districts
                    .OrderBy(district => district.Name)
                    .Select(district => new CityDistrictDto
                    {
                        Id = district.Id,
                        DistrictCode = district.DistrictCode,
                        Name = district.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (city == null)
            {
                return NotFound(new
                {
                    message = "Şehir bulunamadı."
                });
            }
            return Ok(city);
        }

        [HttpPost]
        public async Task<ActionResult<CityDetailDto>> Create([FromBody] CreateCityRequest request, CancellationToken cancellationToken)
        {
            var cityCode = request.CityCode.Trim();
            var cityName = request.Name.Trim();

            var dublicateExists = await _context.Cities
                .AnyAsync(
                    x => x.Name == cityName ||
                         x.CityCode == cityCode, cancellationToken);

            if (dublicateExists)
            {
                return Conflict(new
                {
                    message = "Bu isimde bir şehir zaten mevcut."
                });
            }

            var city = new City
            {
                CityCode = cityCode,
                Name = cityName
            };

            _context.Cities.Add(city);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Şehir eklenirken bir hata oluştu. Lütfen tekrar deneyin."
                });
            }

            var response = new CityDetailDto
            {
                Id = city.Id,
                CityCode = city.CityCode,
                Name = city.Name,
                CreatedDate = city.CreatedDate,
                ModifiedDate = city.ModifiedDate,
                Districts = new List<CityDistrictDto>()
            };

            return CreatedAtAction(nameof(GetById), new { id = city.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CityDetailDto>> Update(int id, [FromBody] UpdateCityRequest request, CancellationToken cancellationToken)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (city is null)
            {
                return NotFound(new
                {
                    message = "Şehir bulunamadı."
                });
            }

            var cityCode = request.CityCode.Trim();
            var cityName = request.Name.Trim();

            var dublicateExists = await _context.Cities
                .AnyAsync(x => x.Id != id && (x.Name == cityName || x.CityCode == cityCode), cancellationToken);

            if (dublicateExists)
            {
                return Conflict(new
                {
                    message = "Bu isimde bir şehir bulunuyor."
                });
            }

            city.Name = cityName;
            city.CityCode = cityCode;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Şehir güncellenirken bir hata oluştu. Lütfen tekrar deneyin."
                });
            }

            var response = new CityDetailDto
            {
                Id = city.Id,
                Name = city.Name,
                CityCode = city.CityCode,
                CreatedDate = city.CreatedDate,
                ModifiedDate = city.ModifiedDate,

                Districts = await _context.Districts
                    .AsNoTracking()
                    .Where(d => d.CityId == city.Id)
                    .OrderBy(d => d.Name)
                    .Select(d => new CityDistrictDto
                    {
                        Id = d.Id,
                        DistrictCode = d.DistrictCode,
                        Name = d.Name
                    })
                    .ToListAsync(cancellationToken)
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromBody] DeleteCityRequest request, CancellationToken cancellationToken)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (city is null)
            {
                return NotFound(new
                {
                    message = "Silinecek şehir bulunamadı."
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

            var hasActiveDistricts = await _context.Districts
                .AnyAsync(x => x.CityId == id, cancellationToken);

            if (hasActiveDistricts)
            {
                return Conflict(new
                {
                    message = "Bu şehirde bağlı ilçeler bulunduğu için silinemez."
                });
            }

            city.IsDeleted = true;
            city.DeleteReason = request?.DeleteReason?.Trim();
            city.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
