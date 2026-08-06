using Microsoft.AspNetCore.Mvc;
using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.AccountCardKindModels;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountCardKindsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountCardKindsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountCardKindListDto>>> GetAll([FromQuery] string? search, [FromQuery] int? accountCardTypeId, CancellationToken cancellationToken)
        {
            search = search?.Trim();

            var query = _context.AccountCardKinds
                .AsNoTracking()
                .AsQueryable();

            if (accountCardTypeId.HasValue && accountCardTypeId > 0)
            {
                query = query.Where(x =>
                    x.AccountCardTypeId == accountCardTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.KindCode.Contains(search) ||
                    x.KindName.Contains(search) ||
                    x.AccountCardType.TypeCode.Contains(search) ||
                    x.AccountCardType.TypeName.Contains(search));
            }

            var items = await query
                .OrderBy(x => x.AccountCardType.TypeCode)
                .ThenBy(x => x.KindCode)
                .Select(x => new AccountCardKindListDto
                {
                    Id = x.Id,
                    KindCode = x.KindCode,
                    KindName = x.KindName,

                    AccountCardTypeId = x.AccountCardTypeId,
                    AccountCardTypeCode = x.AccountCardType.TypeCode,
                    AccountCardTypeName = x.AccountCardType.TypeName,

                    CreatedDate = x.CreatedDate,
                    CreatedByUserId = x.CreatedByUserId,
                    CreatedByUserFullName =
                        x.CreatedByUser.FullName == null
                            ? null
                            : x.CreatedByUser.FullName,

                    ModifiedDate = x.ModifiedDate,
                    ModifiedByUserId = x.ModifiedByUserId,
                    ModifiedByUserFullName =
                        x.ModifiedByUser.FullName == null
                            ? null
                            : x.ModifiedByUser.FullName
                })
                .ToListAsync(cancellationToken);

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AccountCardKindDetailDto>> GetById(
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
                    message = "Hesap kart türü bulunamadı."
                });
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<AccountCardKindDetailDto>> Create(
        [FromBody] CreateAccountCardKindRequest request,
        CancellationToken cancellationToken)
        {
            var kindCode = request.KindCode?
                .Trim()
                .ToUpperInvariant() ?? string.Empty;

            var kindName =
                request.KindName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(kindCode))
            {
                ModelState.AddModelError(
                    nameof(request.KindCode),
                    "Kart tür kodu boş geçilemez.");
            }

            if (string.IsNullOrWhiteSpace(kindName))
            {
                ModelState.AddModelError(
                    nameof(request.KindName),
                    "Kart tür adı boş geçilemez.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var typeExists = await _context.AccountCardTypes
                .AnyAsync(
                    x => x.Id == request.AccountCardTypeId,
                    cancellationToken);

            if (!typeExists)
            {
                return BadRequest(new
                {
                    message = "Seçilen ana hesap tipi bulunamadı."
                });
            }

            var duplicateExists =
                await _context.AccountCardKinds.AnyAsync(
                    x =>
                        x.KindCode == kindCode ||
                        (
                            x.AccountCardTypeId ==
                            request.AccountCardTypeId &&
                            x.KindName == kindName
                        ),
                    cancellationToken);

            if (duplicateExists)
            {
                return Conflict(new
                {
                    message = "Bu hesap kart tür kodu veya seçilen ana hesap tipinde bu tür adı kullanılıyor."
                });
            }

            var item = new AccountCardKind
            {
                KindCode = kindCode,
                KindName = kindName,
                AccountCardTypeId =
                    request.AccountCardTypeId
            };

            _context.AccountCardKinds.Add(item);

            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Bu hesap kart tür kodu veya seçilen ana hesap tipinde bu tür adı kullanılıyor."
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
                            "Kart türü oluşturuldu ancak bilgileri getirilemedi."
                    });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = item.Id },
                response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<AccountCardKindDetailDto>> Update(
        int id,
        [FromBody] UpdateAccountCardKindRequest request,
        CancellationToken cancellationToken)
        {
            var item = await _context.AccountCardKinds
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (item is null)
            {
                return NotFound(new
                {
                    message =
                        "Güncellenecek hesap kart türü bulunamadı."
                });
            }

            var kindCode = request.KindCode?
                .Trim()
                .ToUpperInvariant() ?? string.Empty;

            var kindName =
                request.KindName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(kindCode))
            {
                ModelState.AddModelError(
                    nameof(request.KindCode),
                    "Kart tür kodu boş geçilemez.");
            }

            if (string.IsNullOrWhiteSpace(kindName))
            {
                ModelState.AddModelError(
                    nameof(request.KindName),
                    "Kart tür adı boş geçilemez.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var typeExists = await _context.AccountCardTypes
                .AnyAsync(
                    x => x.Id == request.AccountCardTypeId,
                    cancellationToken);

            if (!typeExists)
            {
                return BadRequest(new
                {
                    message = "Seçilen kart tipi bulunamadı."
                });
            }

            var duplicateExists =
                await _context.AccountCardKinds.AnyAsync(
                    x =>
                        x.Id != id &&
                        (
                            x.KindCode == kindCode ||
                            (
                                x.AccountCardTypeId ==
                                request.AccountCardTypeId &&
                                x.KindName == kindName
                            )
                        ),
                    cancellationToken);

            if (duplicateExists)
            {
                return Conflict(new
                {
                    message = "Bu hesap kart tür kodu veya seçilen ana hesap tipinde bu tür adı kullanılıyor."
                });
            }

            item.KindCode = kindCode;
            item.KindName = kindName;
            item.AccountCardTypeId =
                request.AccountCardTypeId;

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
                        "Bu kart tür kodu veya kart tür adı başka bir kayıtta kullanılıyor."
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
                            "Kart türü güncellendi ancak bilgileri getirilemedi."
                    });
            }

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
        int id,
        [FromBody] DeleteAccountCardKindRequest request,
        CancellationToken cancellationToken)
        {
            var item = await _context.AccountCardKinds
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (item is null)
            {
                return NotFound(new
                {
                    message =
                        "Silinecek hesap kart türü bulunamadı."
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

        private async Task<AccountCardKindDetailDto?> GetDetailAsync(
        int id,
        CancellationToken cancellationToken)
        {
            return await _context.AccountCardKinds
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new AccountCardKindDetailDto
                {
                    Id = x.Id,
                    KindCode = x.KindCode,
                    KindName = x.KindName,

                    AccountCardTypeId =
                        x.AccountCardTypeId,

                    AccountCardTypeCode =
                        x.AccountCardType.TypeCode,

                    AccountCardTypeName =
                        x.AccountCardType.TypeName,

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
}
