using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.VehicleTypeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehicleTypesController(
            AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<VehicleTypeListDto>>> GetAll(
            [FromQuery] string? search,
            CancellationToken cancellationToken)
        {
            search = search?.Trim();

            var query = _context.VehicleTypes
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.TypeName.Contains(search));
            }

            var items = await query
                .OrderBy(x => x.TypeName)
                .Select(x => new VehicleTypeListDto
                {
                    Id = x.Id,

                    TypeName = x.TypeName,

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
        public async Task<ActionResult<VehicleTypeDetailDto>> GetById(
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
                    message = "Araç tipi bulunamadı."
                });
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<VehicleTypeDetailDto>> Create(
            [FromBody] CreateVehicleTypeRequest request,
            CancellationToken cancellationToken)
        {
            var typeName = request.TypeName?.Trim()
                ?? string.Empty;


            if (string.IsNullOrWhiteSpace(typeName))
            {
                ModelState.AddModelError(nameof(request.TypeName),
                    "Araç tipi adı boş geçilemez.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var duplicateExists = await _context.VehicleTypes
                    .AnyAsync(
                        x => x.TypeName == typeName,
                        cancellationToken);


            if (duplicateExists)
            {
                return Conflict(new
                {
                    message = "Bu araç tipi adı zaten kullanılıyor."
                });
            }


            var item = new VehicleType
                {
                    TypeName = typeName
                };

            _context.VehicleTypes.Add(item);

            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Bu araç tipi adı zaten kullanılıyor."
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
        public async Task<ActionResult<VehicleTypeDetailDto>> Update(
            int id,
            [FromBody] UpdateVehicleTypeRequest request,
            CancellationToken cancellationToken)
        {
            var item =
                await _context.VehicleTypes
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);


            if (item is null)
            {
                return NotFound(new
                {
                    message =
                        "Güncellenecek araç tipi bulunamadı."
                });
            }


            var typeName =
                request.TypeName?.Trim()
                ?? string.Empty;


            if (string.IsNullOrWhiteSpace(typeName))
            {
                ModelState.AddModelError(
                    nameof(request.TypeName),
                    "Araç tipi adı boş geçilemez.");

                return ValidationProblem(ModelState);
            }


            var duplicateExists =
                await _context.VehicleTypes
                    .AnyAsync(
                        x =>
                            x.Id != id &&
                            x.TypeName == typeName,
                        cancellationToken);


            if (duplicateExists)
            {
                return Conflict(new
                {
                    message =
                        "Bu araç tipi adı zaten kullanılıyor."
                });
            }


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
                    message =
                        "Bu araç tipi adı zaten kullanılıyor."
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
            [FromBody] DeleteVehicleTypeRequest request,
            CancellationToken cancellationToken)
        {
            var item = await _context.VehicleTypes
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);


            if (item is null)
            {
                return NotFound(new
                {
                    message =
                        "Silinecek araç tipi bulunamadı."
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
                        x => x.VehicleTypeId == id,
                        cancellationToken);


            if (isUsed)
            {
                return Conflict(new
                {
                    message =
                        "Bu araç tipi araç kartlarında kullanıldığı için silinemez."
                });
            }


            item.IsDeleted = true;

            item.DeleteReason =
                deleteReason;


            await _context.SaveChangesAsync(
                cancellationToken);


            return NoContent();
        }

        private async Task<VehicleTypeDetailDto?> GetDetailAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _context.VehicleTypes
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new VehicleTypeDetailDto
                {
                    Id = x.Id,

                    TypeName = x.TypeName,

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
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}