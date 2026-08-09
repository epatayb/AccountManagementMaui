using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.VehicleKindModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleKindsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehicleKindsController(
            AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<VehicleKindListDto>>> GetAll(
            [FromQuery] string? search,
            CancellationToken cancellationToken)
        {
            search = search?.Trim();


            var query = _context.VehicleKinds
                .AsNoTracking()
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.KindName.Contains(search));
            }


            var items = await query
                .OrderBy(x => x.KindName)
                .Select(x => new VehicleKindListDto
                {
                    Id = x.Id,

                    KindName = x.KindName,

                    CreatedDate = x.CreatedDate,

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


            return Ok(items);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehicleKindDetailDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var item =
                await GetDetailAsync(
                    id,
                    cancellationToken);


            if (item is null)
            {
                return NotFound(new
                {
                    message =
                        "Araç türü bulunamadı."
                });
            }

            return Ok(item);
        }


        [HttpPost]
        public async Task<ActionResult<VehicleKindDetailDto>> Create(
            [FromBody] CreateVehicleKindRequest request,
            CancellationToken cancellationToken)
        {
            var kindName =
                request.KindName?.Trim()
                ?? string.Empty;


            if (string.IsNullOrWhiteSpace(kindName))
            {
                ModelState.AddModelError(
                    nameof(request.KindName),
                    "Araç türü adı boş geçilemez.");
            }


            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }


            var duplicateExists =
                await _context.VehicleKinds
                    .AnyAsync(
                        x => x.KindName == kindName,
                        cancellationToken);


            if (duplicateExists)
            {
                return Conflict(new
                {
                    message =
                        "Bu araç türü adı zaten kullanılıyor."
                });
            }


            var item =
                new VehicleKind
                {
                    KindName = kindName
                };


            _context.VehicleKinds.Add(item);


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
                        "Bu araç türü adı zaten kullanılıyor."
                });
            }


            var response =
                await GetDetailAsync(
                    item.Id,
                    cancellationToken);


            return CreatedAtAction(
                nameof(GetById),
                new { id = item.Id },
                response);
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<VehicleKindDetailDto>> Update(
            int id,
            [FromBody] UpdateVehicleKindRequest request,
            CancellationToken cancellationToken)
        {
            var item =
                await _context.VehicleKinds
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);


            if (item is null)
            {
                return NotFound(new
                {
                    message =
                        "Güncellenecek araç türü bulunamadı."
                });
            }


            var kindName =
                request.KindName?.Trim()
                ?? string.Empty;


            if (string.IsNullOrWhiteSpace(kindName))
            {
                ModelState.AddModelError(
                    nameof(request.KindName),
                    "Araç türü adı boş geçilemez.");

                return ValidationProblem(ModelState);
            }


            var duplicateExists =
                await _context.VehicleKinds
                    .AnyAsync(
                        x =>
                            x.Id != id &&
                            x.KindName == kindName,
                        cancellationToken);


            if (duplicateExists)
            {
                return Conflict(new
                {
                    message =
                        "Bu araç türü adı zaten kullanılıyor."
                });
            }


            item.KindName =
                kindName;


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
                        "Bu araç türü adı zaten kullanılıyor."
                });
            }


            var response =
                await GetDetailAsync(
                    id,
                    cancellationToken);


            return Ok(response);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            [FromBody] DeleteVehicleKindRequest request,
            CancellationToken cancellationToken)
        {
            var item =
                await _context.VehicleKinds
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);


            if (item is null)
            {
                return NotFound(new
                {
                    message =
                        "Silinecek araç türü bulunamadı."
                });
            }


            var deleteReason =
                request.DeleteReason?.Trim()
                ?? string.Empty;


            if (string.IsNullOrWhiteSpace(deleteReason))
            {
                ModelState.AddModelError(
                    nameof(request.DeleteReason),
                    "Silme açıklaması zorunludur.");

                return ValidationProblem(ModelState);
            }


            var isUsed =
                await _context.Vehicles
                    .AnyAsync(
                        x => x.VehicleKindId == id,
                        cancellationToken);


            if (isUsed)
            {
                return Conflict(new
                {
                    message =
                        "Bu araç türü araç kartlarında kullanıldığı için silinemez."
                });
            }


            item.IsDeleted = true;

            item.DeleteReason =
                deleteReason;


            await _context.SaveChangesAsync(
                cancellationToken);


            return NoContent();
        }


        private async Task<VehicleKindDetailDto?> GetDetailAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _context.VehicleKinds
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new VehicleKindDetailDto
                {
                    Id = x.Id,

                    KindName = x.KindName,

                    CreatedDate = x.CreatedDate,

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
                .FirstOrDefaultAsync(
                    cancellationToken);
        }
    }
}