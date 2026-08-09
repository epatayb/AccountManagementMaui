using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.AccountCardModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AccountManagementMaui.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountCardsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccountCardsController(AppDbContext context)
    {
        _context = context;
    }


    // GET:
    // api/accountcards
    //
    // api/accountcards?search=abc
    //
    // api/accountcards?
    // accountCardTypeId=1&
    // accountCardKindId=2&
    // accountCardGroupId=1&
    // accountCardSubGroupId=3&
    // cityId=34
    [HttpGet]
    public async Task<ActionResult<List<AccountCardListDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? accountCardTypeId,
        [FromQuery] int? accountCardKindId,
        [FromQuery] int? accountCardGroupId,
        [FromQuery] int? accountCardSubGroupId,
        [FromQuery] int? cityId,
        CancellationToken cancellationToken)
    {
        search = search?.Trim();

        var query = _context.AccountCards
            .AsNoTracking()
            .AsQueryable();


        // ARAMA
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.AccountCode.Contains(search) ||
                x.Title.Contains(search) ||

                (x.TaxNumber != null &&
                 x.TaxNumber.Contains(search)) ||

                (x.IdentityNumber != null &&
                 x.IdentityNumber.Contains(search)) ||

                (x.PhoneNumber != null &&
                 x.PhoneNumber.Contains(search)) ||

                (x.Email != null &&
                 x.Email.Contains(search)) ||

                (x.ContactPerson != null &&
                 x.ContactPerson.Contains(search)) ||

                (x.AccountCardGroup != null &&
                 x.AccountCardGroup.GroupName.Contains(search)) ||

                (x.AccountCardSubGroup != null &&
                 x.AccountCardSubGroup.SubGroupName.Contains(search)) ||

                (x.City != null &&
                 x.City.Name.Contains(search)) ||

                (x.District != null &&
                 x.District.Name.Contains(search)) ||

                (x.TaxOffice != null &&
                 x.TaxOffice.Name.Contains(search)));
        }


        // TİP FİLTRESİ
        if (accountCardTypeId.HasValue &&
            accountCardTypeId.Value > 0)
        {
            query = query.Where(x =>
                x.AccountCardTypeId ==
                accountCardTypeId.Value);
        }


        // TÜR FİLTRESİ
        if (accountCardKindId.HasValue &&
            accountCardKindId.Value > 0)
        {
            query = query.Where(x =>
                x.AccountCardKindId ==
                accountCardKindId.Value);
        }


        // GRUP FİLTRESİ
        if (accountCardGroupId.HasValue &&
            accountCardGroupId.Value > 0)
        {
            query = query.Where(x =>
                x.AccountCardGroupId ==
                accountCardGroupId.Value);
        }


        // ALT GRUP FİLTRESİ
        if (accountCardSubGroupId.HasValue &&
            accountCardSubGroupId.Value > 0)
        {
            query = query.Where(x =>
                x.AccountCardSubGroupId ==
                accountCardSubGroupId.Value);
        }


        // İL FİLTRESİ
        if (cityId.HasValue &&
            cityId.Value > 0)
        {
            query = query.Where(x =>
                x.CityId == cityId.Value);
        }


        var items = await query
            .OrderBy(x => x.Title)
            .ThenBy(x => x.AccountCode)
            .Select(x => new AccountCardListDto
            {
                Id = x.Id,

                AccountCode =
                    x.AccountCode,

                Title =
                    x.Title,


                // Tip
                AccountCardTypeId =
                    x.AccountCardTypeId,

                AccountCardTypeName =
                    x.AccountCardType.TypeName,


                // Tür
                AccountCardKindId =
                    x.AccountCardKindId,

                AccountCardKindName =
                    x.AccountCardKind.KindName,


                // Grup
                AccountCardGroupId =
                    x.AccountCardGroupId,

                AccountCardGroupName =
                    x.AccountCardGroup == null
                        ? null
                        : x.AccountCardGroup.GroupName,


                // Alt Grup
                AccountCardSubGroupId =
                    x.AccountCardSubGroupId,

                AccountCardSubGroupName =
                    x.AccountCardSubGroup == null
                        ? null
                        : x.AccountCardSubGroup.SubGroupName,


                // İl
                CityId =
                    x.CityId,

                CityCode =
                    x.City == null
                        ? null
                        : x.City.CityCode,

                CityName =
                    x.City == null
                        ? null
                        : x.City.Name,


                // İlçe
                DistrictId =
                    x.DistrictId,

                DistrictName =
                    x.District == null
                        ? null
                        : x.District.Name,


                // Vergi Dairesi
                TaxOfficeId =
                    x.TaxOfficeId,

                TaxOfficeCode =
                    x.TaxOffice == null
                        ? null
                        : x.TaxOffice.TaxOfficeCode,

                TaxOfficeName =
                    x.TaxOffice == null
                        ? null
                        : x.TaxOffice.Name,


                TaxNumber =
                    x.TaxNumber,

                IdentityNumber =
                    x.IdentityNumber,


                PhoneNumber =
                    x.PhoneNumber,

                Email =
                    x.Email,

                ContactPerson =
                    x.ContactPerson,


                // Audit
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

        return Ok(items);
    }


    // GET: api/accountcards/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccountCardDetailDto>> GetById(
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
                message =
                    "Hesap kartı bulunamadı."
            });
        }

        return Ok(item);
    }


    // POST: api/accountcards
    [HttpPost]
    public async Task<ActionResult<AccountCardDetailDto>> Create(
        [FromBody] CreateAccountCardRequest request,
        CancellationToken cancellationToken)
    {
        var title =
            request.Title?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            ModelState.AddModelError(
                nameof(request.Title),
                "Hesap ünvanı boş geçilemez.");
        }


        await ValidateRelationsAsync(
            request.AccountCardTypeId,
            request.AccountCardKindId,
            request.AccountCardGroupId,
            request.AccountCardSubGroupId,
            request.CityId,
            request.DistrictId,
            request.TaxOfficeId,
            cancellationToken);


        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }


        var item = new AccountCard
        {
            Title =
                title,

            AccountCardTypeId =
                request.AccountCardTypeId,

            AccountCardKindId =
                request.AccountCardKindId,

            AccountCardGroupId =
                request.AccountCardGroupId,

            AccountCardSubGroupId =
                request.AccountCardSubGroupId,

            CityId =
                request.CityId,

            DistrictId =
                request.DistrictId,

            TaxOfficeId =
                request.TaxOfficeId,

            TaxNumber =
                NormalizeOptional(request.TaxNumber),

            IdentityNumber =
                NormalizeOptional(request.IdentityNumber),

            PhoneNumber =
                NormalizeOptional(request.PhoneNumber),

            Email =
                NormalizeOptional(request.Email),

            ContactPerson =
                NormalizeOptional(request.ContactPerson),

            Address =
                NormalizeOptional(request.Address)
        };


        _context.AccountCards.Add(item);


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
                    "Hesap kartı kaydedilirken veritabanı kısıtlaması nedeniyle işlem tamamlanamadı."
            });
        }


        var response =
            await GetDetailAsync(
                item.Id,
                cancellationToken);


        if (response is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "Hesap kartı oluşturuldu ancak bilgileri getirilemedi."
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


    // PUT: api/accountcards/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<AccountCardDetailDto>> Update(
        int id,
        [FromBody] UpdateAccountCardRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _context.AccountCards
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (item is null)
        {
            return NotFound(new
            {
                message =
                    "Güncellenecek hesap kartı bulunamadı."
            });
        }


        var title =
            request.Title?.Trim() ?? string.Empty;


        if (string.IsNullOrWhiteSpace(title))
        {
            ModelState.AddModelError(
                nameof(request.Title),
                "Hesap ünvanı boş geçilemez.");
        }


        await ValidateRelationsAsync(
            request.AccountCardTypeId,
            request.AccountCardKindId,
            request.AccountCardGroupId,
            request.AccountCardSubGroupId,
            request.CityId,
            request.DistrictId,
            request.TaxOfficeId,
            cancellationToken);


        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }


        item.Title =
            title;

        item.AccountCardTypeId =
            request.AccountCardTypeId;

        item.AccountCardKindId =
            request.AccountCardKindId;

        item.AccountCardGroupId =
            request.AccountCardGroupId;

        item.AccountCardSubGroupId =
            request.AccountCardSubGroupId;

        item.CityId =
            request.CityId;

        item.DistrictId =
            request.DistrictId;

        item.TaxOfficeId =
            request.TaxOfficeId;

        item.TaxNumber =
            NormalizeOptional(request.TaxNumber);

        item.IdentityNumber =
            NormalizeOptional(request.IdentityNumber);

        item.PhoneNumber =
            NormalizeOptional(request.PhoneNumber);

        item.Email =
            NormalizeOptional(request.Email);

        item.ContactPerson =
            NormalizeOptional(request.ContactPerson);

        item.Address =
            NormalizeOptional(request.Address);


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
                    "Hesap kartı güncellenirken veritabanı kısıtlaması nedeniyle işlem tamamlanamadı."
            });
        }


        var response =
            await GetDetailAsync(
                item.Id,
                cancellationToken);


        if (response is null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "Hesap kartı güncellendi ancak bilgileri getirilemedi."
                });
        }


        return Ok(response);
    }


    // DELETE: api/accountcards/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        [FromBody] DeleteAccountCardRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _context.AccountCards
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);


        if (item is null)
        {
            return NotFound(new
            {
                message =
                    "Silinecek hesap kartı bulunamadı."
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


        item.IsDeleted = true;

        item.DeleteReason =
            deleteReason;


        await _context.SaveChangesAsync(
            cancellationToken);


        return NoContent();
    }


    // --------------------------------------------------
    // RELATION VALIDATION
    // --------------------------------------------------

    private async Task ValidateRelationsAsync(
        int accountCardTypeId,
        int accountCardKindId,
        int? accountCardGroupId,
        int? accountCardSubGroupId,
        int? cityId,
        int? districtId,
        int? taxOfficeId,
        CancellationToken cancellationToken)
    {
        // ----------------------------------------------
        // TIP
        // ----------------------------------------------

        var typeExists =
            await _context.AccountCardTypes.AnyAsync(
                x => x.Id == accountCardTypeId,
                cancellationToken);


        if (!typeExists)
        {
            ModelState.AddModelError(
                nameof(CreateAccountCardRequest.AccountCardTypeId),
                "Seçilen hesap kart tipi bulunamadı.");
        }


        // ----------------------------------------------
        // TÜR + TİP UYUMU
        // ----------------------------------------------

        var kindIsValid =
            await _context.AccountCardKinds.AnyAsync(
                x =>
                    x.Id == accountCardKindId &&
                    x.AccountCardTypeId ==
                    accountCardTypeId,
                cancellationToken);


        if (!kindIsValid)
        {
            ModelState.AddModelError(
                nameof(CreateAccountCardRequest.AccountCardKindId),
                "Seçilen hesap kart türü, hesap kart tipi ile uyumlu değildir.");
        }


        // ----------------------------------------------
        // GRUP
        // ----------------------------------------------

        if (accountCardGroupId.HasValue)
        {
            var groupExists =
                await _context.AccountCardGroups.AnyAsync(
                    x =>
                        x.Id ==
                        accountCardGroupId.Value,
                    cancellationToken);


            if (!groupExists)
            {
                ModelState.AddModelError(
                    nameof(CreateAccountCardRequest.AccountCardGroupId),
                    "Seçilen hesap kart grubu bulunamadı.");
            }
        }


        // ----------------------------------------------
        // ALT GRUP
        // ----------------------------------------------

        if (accountCardSubGroupId.HasValue)
        {
            if (!accountCardGroupId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(CreateAccountCardRequest.AccountCardSubGroupId),
                    "Alt grup seçebilmek için önce hesap kart grubu seçilmelidir.");
            }
            else
            {
                var subGroupIsValid =
                    await _context.AccountCardSubGroups.AnyAsync(
                        x =>
                            x.Id ==
                            accountCardSubGroupId.Value &&

                            x.AccountCardGroupId ==
                            accountCardGroupId.Value,
                        cancellationToken);


                if (!subGroupIsValid)
                {
                    ModelState.AddModelError(
                        nameof(CreateAccountCardRequest.AccountCardSubGroupId),
                        "Seçilen alt grup, hesap kart grubu ile uyumlu değildir.");
                }
            }
        }


        // ----------------------------------------------
        // İL
        // ----------------------------------------------

        if (cityId.HasValue)
        {
            var cityExists =
                await _context.Cities.AnyAsync(
                    x =>
                        x.Id ==
                        cityId.Value,
                    cancellationToken);


            if (!cityExists)
            {
                ModelState.AddModelError(
                    nameof(CreateAccountCardRequest.CityId),
                    "Seçilen il bulunamadı.");
            }
        }


        // ----------------------------------------------
        // İLÇE
        // ----------------------------------------------

        if (districtId.HasValue)
        {
            if (!cityId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(CreateAccountCardRequest.DistrictId),
                    "İlçe seçebilmek için önce il seçilmelidir.");
            }
            else
            {
                var districtIsValid =
                    await _context.Districts.AnyAsync(
                        x =>
                            x.Id ==
                            districtId.Value &&

                            x.CityId ==
                            cityId.Value,
                        cancellationToken);


                if (!districtIsValid)
                {
                    ModelState.AddModelError(
                        nameof(CreateAccountCardRequest.DistrictId),
                        "Seçilen ilçe, seçilen il ile uyumlu değildir.");
                }
            }
        }


        // ----------------------------------------------
        // VERGİ DAİRESİ
        // ----------------------------------------------

        if (taxOfficeId.HasValue)
        {
            if (!cityId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(CreateAccountCardRequest.TaxOfficeId),
                    "Vergi dairesi seçebilmek için önce il seçilmelidir.");
            }
            else
            {
                var taxOfficeIsValid =
                    await _context.TaxOffices.AnyAsync(
                        x =>
                            x.Id ==
                            taxOfficeId.Value &&

                            x.CityId ==
                            cityId.Value,
                        cancellationToken);


                if (!taxOfficeIsValid)
                {
                    ModelState.AddModelError(
                        nameof(CreateAccountCardRequest.TaxOfficeId),
                        "Seçilen vergi dairesi, seçilen il ile uyumlu değildir.");
                }
            }
        }
    }


    // --------------------------------------------------
    // DETAIL
    // --------------------------------------------------

    private async Task<AccountCardDetailDto?> GetDetailAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.AccountCards
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AccountCardDetailDto
            {
                Id =
                    x.Id,

                AccountCode =
                    x.AccountCode,

                Title =
                    x.Title,


                // Tip
                AccountCardTypeId =
                    x.AccountCardTypeId,

                AccountCardTypeName =
                    x.AccountCardType.TypeName,


                // Tür
                AccountCardKindId =
                    x.AccountCardKindId,

                AccountCardKindName =
                    x.AccountCardKind.KindName,


                // Grup
                AccountCardGroupId =
                    x.AccountCardGroupId,

                AccountCardGroupName =
                    x.AccountCardGroup == null
                        ? null
                        : x.AccountCardGroup.GroupName,


                // Alt Grup
                AccountCardSubGroupId =
                    x.AccountCardSubGroupId,

                AccountCardSubGroupName =
                    x.AccountCardSubGroup == null
                        ? null
                        : x.AccountCardSubGroup.SubGroupName,


                // İl
                CityId =
                    x.CityId,

                CityCode =
                    x.City == null
                        ? null
                        : x.City.CityCode,

                CityName =
                    x.City == null
                        ? null
                        : x.City.Name,


                // İlçe
                DistrictId =
                    x.DistrictId,

                DistrictName =
                    x.District == null
                        ? null
                        : x.District.Name,


                // Vergi Dairesi
                TaxOfficeId =
                    x.TaxOfficeId,

                TaxOfficeCode =
                    x.TaxOffice == null
                        ? null
                        : x.TaxOffice.TaxOfficeCode,

                TaxOfficeName =
                    x.TaxOffice == null
                        ? null
                        : x.TaxOffice.Name,


                TaxNumber =
                    x.TaxNumber,

                IdentityNumber =
                    x.IdentityNumber,


                PhoneNumber =
                    x.PhoneNumber,

                Email =
                    x.Email,

                ContactPerson =
                    x.ContactPerson,


                Address =
                    x.Address,


                // Audit
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


    // --------------------------------------------------
    // NORMALIZE
    // --------------------------------------------------

    private static string? NormalizeOptional(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}