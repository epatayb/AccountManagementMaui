using System.Globalization;
using System.Text.RegularExpressions;

using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.VehicleModels;
using AccountManagementMaui.Api.Services.VehicleServices;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private static readonly CultureInfo
            TurkishCulture =
                CultureInfo.GetCultureInfo(
                    "tr-TR");


        private readonly AppDbContext _context;

        private readonly IVehicleAccountResolver
            _vehicleAccountResolver;


        public VehiclesController(
            AppDbContext context,
            IVehicleAccountResolver vehicleAccountResolver)
        {
            _context =
                context;

            _vehicleAccountResolver =
                vehicleAccountResolver;
        }


        // =====================================================
        // LIST
        // =====================================================

        [HttpGet]
        public async Task<ActionResult<VehicleListResponse>> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? vehicleTypeId,
            [FromQuery] int? vehicleKindId,
            [FromQuery] int? cityId,
            [FromQuery] bool? isActive,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            CancellationToken cancellationToken = default)
        {
            search =
                search?.Trim();


            if (page < 1)
            {
                page = 1;
            }


            pageSize =
                pageSize switch
                {
                    25 => 25,
                    50 => 50,
                    100 => 100,
                    _ => 25
                };


            var query =
                _context.Vehicles
                    .AsNoTracking()
                    .AsQueryable();


            // =================================================
            // SEARCH
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                search))
            {
                query =
                    query.Where(x =>
                        x.Plate.Contains(search) ||

                        (
                            x.TrailerPlate != null &&
                            x.TrailerPlate.Contains(search)
                        ) ||

                        (
                            x.DriverAccountCard != null &&
                            (
                                x.DriverAccountCard.Title
                                    .Contains(search) ||

                                (
                                    x.DriverAccountCard
                                        .IdentityNumber != null &&

                                    x.DriverAccountCard
                                        .IdentityNumber
                                        .Contains(search)
                                ) ||

                                (
                                    x.DriverAccountCard
                                        .PhoneNumber != null &&

                                    x.DriverAccountCard
                                        .PhoneNumber
                                        .Contains(search)
                                )
                            )
                        ) ||

                        (
                            x.ReferenceAccountCard != null &&
                            x.ReferenceAccountCard.Title
                                .Contains(search)
                        ) ||

                        (
                            x.LicenseAccountCard != null &&
                            x.LicenseAccountCard.Title
                                .Contains(search)
                        ) ||

                        (
                            x.InvoiceAccountCard != null &&
                            x.InvoiceAccountCard.Title
                                .Contains(search)
                        ) ||

                        (
                            x.LicenseOwnerName != null &&
                            x.LicenseOwnerName
                                .Contains(search)
                        ) ||

                        (
                            x.AuthorizedName != null &&
                            x.AuthorizedName
                                .Contains(search)
                        ));
            }


            // =================================================
            // FILTERS
            // =================================================

            if (vehicleTypeId.HasValue &&
                vehicleTypeId.Value > 0)
            {
                query =
                    query.Where(x =>
                        x.VehicleTypeId ==
                        vehicleTypeId.Value);
            }


            if (vehicleKindId.HasValue &&
                vehicleKindId.Value > 0)
            {
                query =
                    query.Where(x =>
                        x.VehicleKindId ==
                        vehicleKindId.Value);
            }


            if (cityId.HasValue &&
                cityId.Value > 0)
            {
                query =
                    query.Where(x =>
                        x.LicenseOwnerCityId ==
                        cityId.Value);
            }


            if (isActive.HasValue)
            {
                query =
                    query.Where(x =>
                        x.IsActive ==
                        isActive.Value);
            }


            // =================================================
            // COUNT
            // =================================================

            var totalCount =
                await query.CountAsync(
                    cancellationToken);


            // =================================================
            // SORT
            // =================================================

            var descending =
                string.Equals(
                    sortDirection,
                    "desc",
                    StringComparison.OrdinalIgnoreCase);


            query =
                sortBy?.Trim().ToLowerInvariant()
                switch
                {
                    "type" =>
                        descending
                            ? query.OrderByDescending(
                                x =>
                                    x.VehicleType.TypeName)
                            : query.OrderBy(
                                x =>
                                    x.VehicleType.TypeName),

                    "kind" =>
                        descending
                            ? query.OrderByDescending(
                                x =>
                                    x.VehicleKind == null
                                        ? string.Empty
                                        : x.VehicleKind.KindName)
                            : query.OrderBy(
                                x =>
                                    x.VehicleKind == null
                                        ? string.Empty
                                        : x.VehicleKind.KindName),

                    "driver" =>
                        descending
                            ? query.OrderByDescending(
                                x =>
                                    x.DriverAccountCard == null
                                        ? string.Empty
                                        : x.DriverAccountCard.Title)
                            : query.OrderBy(
                                x =>
                                    x.DriverAccountCard == null
                                        ? string.Empty
                                        : x.DriverAccountCard.Title),

                    "createddate" =>
                        descending
                            ? query.OrderByDescending(
                                x => x.CreatedDate)
                            : query.OrderBy(
                                x => x.CreatedDate),

                    _ =>
                        descending
                            ? query.OrderByDescending(
                                x => x.Plate)
                            : query.OrderBy(
                                x => x.Plate)
                };


            // =================================================
            // PAGE
            // =================================================

            var items =
                await query
                    .Skip(
                        (page - 1) *
                        pageSize)
                    .Take(pageSize)
                    .Select(x =>
                        new VehicleListDto
                        {
                            Id =
                                x.Id,

                            Plate =
                                x.Plate,


                            VehicleTypeId =
                                x.VehicleTypeId,

                            VehicleTypeName =
                                x.VehicleType.TypeName,


                            VehicleKindId =
                                x.VehicleKindId,

                            VehicleKindName =
                                x.VehicleKind == null
                                    ? null
                                    : x.VehicleKind.KindName,


                            TrailerPlate =
                                x.TrailerPlate,

                            Brand =
                                x.Brand,

                            Model =
                                x.Model,

                            Country =
                                x.Country,


                            // DRIVER

                            DriverAccountCardId =
                                x.DriverAccountCardId,

                            DriverName =
                                x.DriverAccountCard == null
                                    ? null
                                    : x.DriverAccountCard.Title,

                            DriverIdentityNumber =
                                x.DriverAccountCard == null
                                    ? null
                                    : x.DriverAccountCard
                                        .IdentityNumber,

                            DriverPhoneNumber =
                                x.DriverAccountCard == null
                                    ? null
                                    : x.DriverAccountCard
                                        .PhoneNumber,


                            // REFERENCE

                            ReferenceAccountCardId =
                                x.ReferenceAccountCardId,

                            ReferenceName =
                                x.ReferenceAccountCard == null
                                    ? null
                                    : x.ReferenceAccountCard.Title,

                            ReferencePhoneNumber =
                                x.ReferenceAccountCard == null
                                    ? null
                                    : x.ReferenceAccountCard
                                        .PhoneNumber,


                            // LICENSE

                            LicenseAccountCardId =
                                x.LicenseAccountCardId,

                            LicenseAccountCardName =
                                x.LicenseAccountCard == null
                                    ? null
                                    : x.LicenseAccountCard.Title,


                            // INVOICE

                            InvoiceAccountCardId =
                                x.InvoiceAccountCardId,

                            InvoiceAccountCardName =
                                x.InvoiceAccountCard == null
                                    ? null
                                    : x.InvoiceAccountCard.Title,


                            // SNAPSHOT

                            LicenseOwnerName =
                                x.LicenseOwnerName,

                            LicenseOwnerIdentityNumber =
                                x.LicenseOwnerIdentityNumber,

                            LicenseOwnerTaxNumber =
                                x.LicenseOwnerTaxNumber,

                            LicenseOwnerCityId =
                                x.LicenseOwnerCityId,

                            LicenseOwnerCityName =
                                x.LicenseOwnerCity == null
                                    ? null
                                    : x.LicenseOwnerCity.Name,


                            AuthorizedName =
                                x.AuthorizedName,

                            AuthorizedPhone =
                                x.AuthorizedPhone,


                            InsuranceExpiryDate =
                                x.InsuranceExpiryDate,

                            InspectionExpiryDate =
                                x.InspectionExpiryDate,


                            IsActive =
                                x.IsActive,


                            CreatedDate =
                                x.CreatedDate,

                            CreatedByUserFullName =
                                x.CreatedByUser == null
                                    ? null
                                    : x.CreatedByUser.FirstName +
                                      " " +
                                      x.CreatedByUser.LastName,

                            ModifiedDate =
                                x.ModifiedDate,

                            ModifiedByUserFullName =
                                x.ModifiedByUser == null
                                    ? null
                                    : x.ModifiedByUser.FirstName +
                                      " " +
                                      x.ModifiedByUser.LastName
                        })
                    .ToListAsync(
                        cancellationToken);


            var totalPages =
                totalCount == 0
                    ? 0
                    : (int)Math.Ceiling(
                        totalCount /
                        (double)pageSize);


            return Ok(
                new VehicleListResponse
                {
                    Items =
                        items,

                    Page =
                        page,

                    PageSize =
                        pageSize,

                    TotalCount =
                        totalCount,

                    TotalPages =
                        totalPages
                });
        }


        // =====================================================
        // DETAIL
        // =====================================================

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehicleDetailDto>> GetById(
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
                    code =
                        "VEHICLE_NOT_FOUND",

                    message =
                        "Araç kaydı bulunamadı."
                });
            }


            return Ok(item);
        }


        // =====================================================
        // CREATE
        // =====================================================

        [HttpPost]
        public async Task<ActionResult<VehicleDetailDto>> Create(
    [FromBody] CreateVehicleRequest request,
    CancellationToken cancellationToken)
        {
            // =====================================================
            // PLATE
            // =====================================================

            var plate =
                NormalizePlateForDisplay(
                    request.Plate);


            var normalizedPlate =
                NormalizePlateForComparison(
                    plate);


            if (string.IsNullOrWhiteSpace(
                normalizedPlate))
            {
                ModelState.AddModelError(
                    nameof(request.Plate),
                    "Plaka zorunludur.");
            }


            // =====================================================
            // VEHICLE DEFINITIONS
            // =====================================================

            await ValidateVehicleDefinitionsAsync(
                request,
                cancellationToken);


            if (!ModelState.IsValid)
            {
                return ValidationProblem(
                    ModelState);
            }


            // =====================================================
            // DUPLICATE PLATE
            // =====================================================

            var duplicateExists =
                await _context.Vehicles
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.IsActive &&
                            x.NormalizedPlate ==
                            normalizedPlate,
                        cancellationToken);


            if (duplicateExists)
            {
                return Conflict(new
                {
                    code =
                        "VEHICLE_PLATE_EXISTS",

                    message =
                        "Bu plaka ile aktif bir araç kaydı zaten mevcut."
                });
            }


            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync(
                        cancellationToken);


            try
            {
                // =================================================
                // DRIVER
                // =================================================

                var driver =
                    await _vehicleAccountResolver
                        .ResolveDriverAsync(
                            request.DriverAccountCardId,
                            request.DriverAccount,
                            cancellationToken);


                // =================================================
                // REFERENCE
                // =================================================

                var reference =
                    await _vehicleAccountResolver
                        .ResolveReferenceAsync(
                            request.ReferenceAccountCardId,
                            request.ReferenceAccount,
                            cancellationToken);


                // =================================================
                // LICENSE
                // =================================================

                var licenseInput =
                    request.LicenseAccount ??
                    BuildLegacyLicenseInput(
                        request);


                AccountCard? licenseAccount;


                if (request.DriverIsLicenseOwner &&
                    driver is not null)
                {
                    /*
                     * Aynı AccountCard kullanılır.
                     * İkinci kayıt açılmaz.
                     */
                    licenseAccount =
                        driver;
                }
                else
                {
                    licenseAccount =
                        await _vehicleAccountResolver
                            .ResolveLicenseAsync(
                                request.LicenseAccountCardId,
                                licenseInput,
                                cancellationToken);
                }


                // =================================================
                // INVOICE
                // =================================================

                AccountCard? invoiceAccount;


                if (request.ReferenceIsInvoiceAccount)
                {
                    invoiceAccount =
                        reference;
                }
                else if (
                    request.LicenseOwnerIsInvoiceAccount)
                {
                    invoiceAccount =
                        licenseAccount;
                }
                else
                {
                    /*
                     * Mevcut herhangi bir AccountCard kabul edilir.
                     *
                     * Resolver yalnız yeni kayıt oluştururken
                     * Müşteri / Müşteri kullanır.
                     */
                    invoiceAccount =
                        await _vehicleAccountResolver
                            .ResolveInvoiceAsync(
                                request.InvoiceAccountCardId,
                                request.InvoiceAccount,
                                cancellationToken);
                }


                // =================================================
                // VEHICLE
                // =================================================

                var item =
                    new Vehicle
                    {
                        Plate =
                            plate,

                        NormalizedPlate =
                            normalizedPlate,


                        VehicleTypeId =
                            request.VehicleTypeId,

                        VehicleKindId =
                            request.VehicleKindId,


                        TrailerPlate =
                            NormalizePlateOptional(
                                request.TrailerPlate),

                        Brand =
                            NormalizeOptional(
                                request.Brand),

                        Model =
                            NormalizeOptional(
                                request.Model),

                        Country =
                            NormalizeOptional(
                                request.Country),


                        // =========================================
                        // RELATIONS
                        // =========================================

                        DriverAccountCard =
                            driver,

                        ReferenceAccountCard =
                            reference,

                        LicenseAccountCard =
                            licenseAccount,

                        InvoiceAccountCard =
                            invoiceAccount,


                        DriverIsLicenseOwner =
                            request.DriverIsLicenseOwner,

                        ReferenceIsInvoiceAccount =
                            request.ReferenceIsInvoiceAccount,

                        LicenseOwnerIsInvoiceAccount =
                            request.LicenseOwnerIsInvoiceAccount,


                        // =========================================
                        // LICENSE SNAPSHOT
                        // =========================================

                        LicenseOwnerName =
                            licenseAccount?.Title
                            ?? NormalizeOptional(
                                licenseInput?.Title)
                            ?? NormalizeOptional(
                                request.LicenseOwnerName),


                        LicenseOwnerTaxNumber =
                            licenseAccount?.TaxNumber
                            ?? NormalizeOptional(
                                licenseInput?.TaxNumber)
                            ?? NormalizeOptional(
                                request.LicenseOwnerTaxNumber),


                        LicenseOwnerIdentityNumber =
                            licenseAccount?.IdentityNumber
                            ?? NormalizeOptional(
                                licenseInput?.IdentityNumber)
                            ?? NormalizeOptional(
                                request.LicenseOwnerIdentityNumber),


                        LicenseOwnerAddress =
                            licenseAccount?.Address
                            ?? NormalizeOptional(
                                licenseInput?.Address)
                            ?? NormalizeOptional(
                                request.LicenseOwnerAddress),


                        LicenseOwnerCityId =
                            licenseAccount?.CityId
                            ?? NormalizeId(
                                licenseInput?.CityId)
                            ?? NormalizeId(
                                request.LicenseOwnerCityId),


                        LicenseOwnerTaxOfficeId =
                            licenseAccount?.TaxOfficeId
                            ?? NormalizeId(
                                licenseInput?.TaxOfficeId)
                            ?? NormalizeId(
                                request.LicenseOwnerTaxOfficeId),


                        // =========================================
                        // AUTHORIZED
                        // =========================================

                        AuthorizedName =
                            NormalizeOptional(
                                request.AuthorizedName),

                        AuthorizedPhone =
                            NormalizeOptional(
                                request.AuthorizedPhone),


                        // =========================================
                        // DOCUMENT
                        // =========================================

                        InsuranceExpiryDate =
                            request.InsuranceExpiryDate,

                        InspectionExpiryDate =
                            request.InspectionExpiryDate,


                        IsActive =
                            true
                    };


                _context.Vehicles.Add(
                    item);


                /*
                 * Yeni AccountCard kayıtları ve Vehicle
                 * aynı transaction içinde kaydedilir.
                 */
                await _context.SaveChangesAsync(
                    cancellationToken);


                await transaction.CommitAsync(
                    cancellationToken);


                var response =
                    await GetDetailAsync(
                        item.Id,
                        cancellationToken);


                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id =
                            item.Id
                    },
                    response);
            }
            catch (VehicleAccountResolverException exception)
            {
                await transaction.RollbackAsync(
                    cancellationToken);


                return BadRequest(new
                {
                    code =
                        "VEHICLE_ACCOUNT_INVALID",

                    message =
                        exception.Message
                });
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync(
                    cancellationToken);


                return Conflict(new
                {
                    code =
                        "VEHICLE_ACCOUNT_CONFLICT",

                    message =
                        "Araç veya hesap kartı kaydedilemedi. Plaka, TC veya diğer benzersiz bilgileri kontrol edin."
                });
            }
        }


        // =====================================================
        // UPDATE
        // =====================================================

        [HttpPut("{id:int}")]
        public async Task<ActionResult<VehicleDetailDto>> Update(
    int id,
    [FromBody] UpdateVehicleRequest request,
    CancellationToken cancellationToken)
        {
            var item =
                await _context.Vehicles
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == id,
                        cancellationToken);


            if (item is null)
            {
                return NotFound(new
                {
                    code =
                        "VEHICLE_NOT_FOUND",

                    message =
                        "Güncellenecek araç kaydı bulunamadı."
                });
            }


            // =====================================================
            // CONCURRENCY INPUT
            // =====================================================

            if (request.RowVersion is null ||
                request.RowVersion.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(request.RowVersion),
                    "Kayıt sürüm bilgisi zorunludur.");
            }


            // =====================================================
            // PLATE
            // =====================================================

            var plate =
                NormalizePlateForDisplay(
                    request.Plate);


            var normalizedPlate =
                NormalizePlateForComparison(
                    plate);


            if (string.IsNullOrWhiteSpace(
                normalizedPlate))
            {
                ModelState.AddModelError(
                    nameof(request.Plate),
                    "Plaka zorunludur.");
            }


            // =====================================================
            // DEFINITIONS
            // =====================================================

            await ValidateVehicleDefinitionsAsync(
                request,
                cancellationToken);


            if (!ModelState.IsValid)
            {
                return ValidationProblem(
                    ModelState);
            }


            // =====================================================
            // DUPLICATE PLATE
            // =====================================================

            if (item.IsActive)
            {
                var duplicateExists =
                    await _context.Vehicles
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.Id != id &&
                                x.IsActive &&
                                x.NormalizedPlate ==
                                normalizedPlate,
                            cancellationToken);


                if (duplicateExists)
                {
                    return Conflict(new
                    {
                        code =
                            "VEHICLE_PLATE_EXISTS",

                        message =
                            "Bu plaka ile aktif bir araç kaydı zaten mevcut."
                    });
                }
            }


            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync(
                        cancellationToken);


            try
            {
                // =================================================
                // DRIVER
                // =================================================

                var driver =
                    await _vehicleAccountResolver
                        .ResolveDriverAsync(
                            request.DriverAccountCardId,
                            request.DriverAccount,
                            cancellationToken);


                // =================================================
                // REFERENCE
                // =================================================

                var reference =
                    await _vehicleAccountResolver
                        .ResolveReferenceAsync(
                            request.ReferenceAccountCardId,
                            request.ReferenceAccount,
                            cancellationToken);


                // =================================================
                // LICENSE
                // =================================================

                var licenseInput =
                    request.LicenseAccount ??
                    BuildLegacyLicenseInput(
                        request);


                AccountCard? licenseAccount;


                if (request.DriverIsLicenseOwner &&
                    driver is not null)
                {
                    licenseAccount =
                        driver;
                }
                else
                {
                    licenseAccount =
                        await _vehicleAccountResolver
                            .ResolveLicenseAsync(
                                request.LicenseAccountCardId,
                                licenseInput,
                                cancellationToken);
                }


                // =================================================
                // INVOICE
                // =================================================

                AccountCard? invoiceAccount;


                if (request.ReferenceIsInvoiceAccount)
                {
                    invoiceAccount =
                        reference;
                }
                else if (
                    request.LicenseOwnerIsInvoiceAccount)
                {
                    invoiceAccount =
                        licenseAccount;
                }
                else
                {
                    invoiceAccount =
                        await _vehicleAccountResolver
                            .ResolveInvoiceAsync(
                                request.InvoiceAccountCardId,
                                request.InvoiceAccount,
                                cancellationToken);
                }


                // =================================================
                // VEHICLE
                // =================================================

                item.Plate =
                    plate;


                item.NormalizedPlate =
                    normalizedPlate;


                item.VehicleTypeId =
                    request.VehicleTypeId;


                item.VehicleKindId =
                    request.VehicleKindId;


                item.TrailerPlate =
                    NormalizePlateOptional(
                        request.TrailerPlate);


                item.Brand =
                    NormalizeOptional(
                        request.Brand);


                item.Model =
                    NormalizeOptional(
                        request.Model);


                item.Country =
                    NormalizeOptional(
                        request.Country);


                // =================================================
                // RELATIONS
                // =================================================

                item.DriverAccountCard =
                    driver;


                item.ReferenceAccountCard =
                    reference;


                item.LicenseAccountCard =
                    licenseAccount;


                item.InvoiceAccountCard =
                    invoiceAccount;


                item.DriverIsLicenseOwner =
                    request.DriverIsLicenseOwner;


                item.ReferenceIsInvoiceAccount =
                    request.ReferenceIsInvoiceAccount;


                item.LicenseOwnerIsInvoiceAccount =
                    request.LicenseOwnerIsInvoiceAccount;


                // =================================================
                // SNAPSHOT
                // =================================================

                item.LicenseOwnerName =
                    licenseAccount?.Title
                    ?? NormalizeOptional(
                        licenseInput?.Title)
                    ?? NormalizeOptional(
                        request.LicenseOwnerName);


                item.LicenseOwnerTaxNumber =
                    licenseAccount?.TaxNumber
                    ?? NormalizeOptional(
                        licenseInput?.TaxNumber)
                    ?? NormalizeOptional(
                        request.LicenseOwnerTaxNumber);


                item.LicenseOwnerIdentityNumber =
                    licenseAccount?.IdentityNumber
                    ?? NormalizeOptional(
                        licenseInput?.IdentityNumber)
                    ?? NormalizeOptional(
                        request.LicenseOwnerIdentityNumber);


                item.LicenseOwnerAddress =
                    licenseAccount?.Address
                    ?? NormalizeOptional(
                        licenseInput?.Address)
                    ?? NormalizeOptional(
                        request.LicenseOwnerAddress);


                item.LicenseOwnerCityId =
                    licenseAccount?.CityId
                    ?? NormalizeId(
                        licenseInput?.CityId)
                    ?? NormalizeId(
                        request.LicenseOwnerCityId);


                item.LicenseOwnerTaxOfficeId =
                    licenseAccount?.TaxOfficeId
                    ?? NormalizeId(
                        licenseInput?.TaxOfficeId)
                    ?? NormalizeId(
                        request.LicenseOwnerTaxOfficeId);


                // =================================================
                // OTHER
                // =================================================

                item.AuthorizedName =
                    NormalizeOptional(
                        request.AuthorizedName);


                item.AuthorizedPhone =
                    NormalizeOptional(
                        request.AuthorizedPhone);


                item.InsuranceExpiryDate =
                    request.InsuranceExpiryDate;


                item.InspectionExpiryDate =
                    request.InspectionExpiryDate;


                // =================================================
                // CONCURRENCY
                // =================================================

                _context.Entry(item)
                    .Property(x => x.RowVersion)
                    .OriginalValue =
                        request.RowVersion;


                await _context.SaveChangesAsync(
                    cancellationToken);


                await transaction.CommitAsync(
                    cancellationToken);


                var response =
                    await GetDetailAsync(
                        id,
                        cancellationToken);


                return Ok(response);
            }
            catch (VehicleAccountResolverException exception)
            {
                await transaction.RollbackAsync(
                    cancellationToken);


                return BadRequest(new
                {
                    code =
                        "VEHICLE_ACCOUNT_INVALID",

                    message =
                        exception.Message
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(
                    cancellationToken);


                return Conflict(new
                {
                    code =
                        "CONCURRENCY_CONFLICT",

                    message =
                        "Kayıt başka bir kullanıcı tarafından güncellendi. Verileri yenileyip tekrar deneyin."
                });
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync(
                    cancellationToken);


                return Conflict(new
                {
                    code =
                        "VEHICLE_ACCOUNT_CONFLICT",

                    message =
                        "Araç veya hesap kartı güncellenemedi. Plaka, TC veya diğer benzersiz bilgileri kontrol edin."
                });
            }
        }


        // =====================================================
        // CHANGE STATUS
        // =====================================================

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<VehicleDetailDto>>
            ChangeStatus(
                int id,
                [FromBody] ChangeVehicleStatusRequest request,
                CancellationToken cancellationToken)
        {
            var vehicle =
                await _context.Vehicles
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == id,
                        cancellationToken);


            if (vehicle is null)
            {
                return NotFound(new
                {
                    code =
                        "VEHICLE_NOT_FOUND",

                    message =
                        "Araç kaydı bulunamadı."
                });
            }


            if (request.RowVersion is null ||
                request.RowVersion.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(request.RowVersion),
                    "Kayıt sürüm bilgisi zorunludur.");


                return ValidationProblem(
                    ModelState);
            }


            if (request.IsActive &&
                !vehicle.IsActive)
            {
                var duplicateExists =
                    await _context.Vehicles
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.Id != vehicle.Id &&
                                x.IsActive &&
                                x.NormalizedPlate ==
                                vehicle.NormalizedPlate,
                            cancellationToken);


                if (duplicateExists)
                {
                    return Conflict(new
                    {
                        code =
                            "VEHICLE_PLATE_EXISTS",

                        message =
                            "Bu plaka ile aktif bir araç kaydı zaten mevcut."
                    });
                }
            }


            _context.Entry(vehicle)
                .Property(x => x.RowVersion)
                .OriginalValue =
                    request.RowVersion;


            vehicle.IsActive =
                request.IsActive;


            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new
                {
                    code =
                        "CONCURRENCY_CONFLICT",

                    message =
                        "Kayıt başka bir kullanıcı tarafından güncellendi. Verileri yenileyip tekrar deneyin."
                });
            }


            var response =
                await GetDetailAsync(
                    vehicle.Id,
                    cancellationToken);


            return Ok(response);
        }


        // =====================================================
        // DELETE
        // =====================================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            [FromBody] DeleteVehicleRequest request,
            CancellationToken cancellationToken)
        {
            var vehicle =
                await _context.Vehicles
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == id,
                        cancellationToken);


            if (vehicle is null)
            {
                return NotFound(new
                {
                    code =
                        "VEHICLE_NOT_FOUND",

                    message =
                        "Silinecek araç kaydı bulunamadı."
                });
            }


            var deleteReason =
                request.DeleteReason?.Trim()
                ?? string.Empty;


            if (string.IsNullOrWhiteSpace(
                deleteReason))
            {
                ModelState.AddModelError(
                    nameof(request.DeleteReason),
                    "Silme açıklaması zorunludur.");
            }


            if (request.RowVersion is null ||
                request.RowVersion.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(request.RowVersion),
                    "Kayıt sürüm bilgisi zorunludur.");
            }


            if (!ModelState.IsValid)
            {
                return ValidationProblem(
                    ModelState);
            }


            _context.Entry(vehicle)
                .Property(x => x.RowVersion)
                .OriginalValue =
                    request.RowVersion;


            vehicle.IsDeleted =
                true;

            vehicle.DeleteReason =
                deleteReason;


            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new
                {
                    code =
                        "CONCURRENCY_CONFLICT",

                    message =
                        "Kayıt başka bir kullanıcı tarafından güncellendi. Verileri yenileyip tekrar deneyin."
                });
            }


            return NoContent();
        }


        // =====================================================
        // VEHICLE DEFINITIONS VALIDATION
        // =====================================================

        private async Task ValidateVehicleDefinitionsAsync(
    CreateVehicleRequest request,
    CancellationToken cancellationToken)
        {
            // =====================================================
            // VEHICLE TYPE
            // =====================================================

            var vehicleTypeExists =
                await _context.VehicleTypes
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id ==
                            request.VehicleTypeId,
                        cancellationToken);


            if (!vehicleTypeExists)
            {
                ModelState.AddModelError(
                    nameof(request.VehicleTypeId),
                    "Seçilen araç tipi bulunamadı.");
            }


            // =====================================================
            // VEHICLE KIND
            // ZORUNLU
            // =====================================================

            var vehicleKindExists =
                await _context.VehicleKinds
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id ==
                            request.VehicleKindId,
                        cancellationToken);


            if (!vehicleKindExists)
            {
                ModelState.AddModelError(
                    nameof(request.VehicleKindId),
                    "Seçilen araç türü bulunamadı.");
            }


            // =====================================================
            // INVOICE FLAG CONFLICT
            // =====================================================

            if (request.ReferenceIsInvoiceAccount &&
                request.LicenseOwnerIsInvoiceAccount)
            {
                ModelState.AddModelError(
                    nameof(
                        request.ReferenceIsInvoiceAccount),
                    "Referans ve ruhsat carisi aynı anda fatura hesabı olarak seçilemez.");
            }
        }


        // =====================================================
        // DETAIL QUERY
        // =====================================================

        private async Task<VehicleDetailDto?>
            GetDetailAsync(
                int id,
                CancellationToken cancellationToken)
        {
            return await _context.Vehicles
                .AsNoTracking()
                .Where(x =>
                    x.Id == id)
                .Select(x =>
                    new VehicleDetailDto
                    {
                        Id =
                            x.Id,

                        Plate =
                            x.Plate,


                        VehicleTypeId =
                            x.VehicleTypeId,

                        VehicleTypeName =
                            x.VehicleType.TypeName,


                        VehicleKindId =
                            x.VehicleKindId,

                        VehicleKindName =
                            x.VehicleKind == null
                                ? null
                                : x.VehicleKind.KindName,


                        TrailerPlate =
                            x.TrailerPlate,

                        Brand =
                            x.Brand,

                        Model =
                            x.Model,

                        Country =
                            x.Country,


                        // DRIVER

                        DriverAccountCardId =
                            x.DriverAccountCardId,

                        DriverName =
                            x.DriverAccountCard == null
                                ? null
                                : x.DriverAccountCard.Title,

                        DriverIdentityNumber =
                            x.DriverAccountCard == null
                                ? null
                                : x.DriverAccountCard
                                    .IdentityNumber,

                        DriverPhoneNumber =
                            x.DriverAccountCard == null
                                ? null
                                : x.DriverAccountCard
                                    .PhoneNumber,

                        DriverIsLicenseOwner =
                            x.DriverIsLicenseOwner,


                        // REFERENCE

                        ReferenceAccountCardId =
                            x.ReferenceAccountCardId,

                        ReferenceName =
                            x.ReferenceAccountCard == null
                                ? null
                                : x.ReferenceAccountCard.Title,

                        ReferencePhoneNumber =
                            x.ReferenceAccountCard == null
                                ? null
                                : x.ReferenceAccountCard
                                    .PhoneNumber,


                        // LICENSE

                        LicenseAccountCardId =
                            x.LicenseAccountCardId,

                        LicenseAccountCardName =
                            x.LicenseAccountCard == null
                                ? null
                                : x.LicenseAccountCard.Title,


                        // INVOICE

                        InvoiceAccountCardId =
                            x.InvoiceAccountCardId,

                        InvoiceAccountCardName =
                            x.InvoiceAccountCard == null
                                ? null
                                : x.InvoiceAccountCard.Title,

                        ReferenceIsInvoiceAccount =
                            x.ReferenceIsInvoiceAccount,

                        LicenseOwnerIsInvoiceAccount =
                            x.LicenseOwnerIsInvoiceAccount,


                        // SNAPSHOT

                        LicenseOwnerName =
                            x.LicenseOwnerName,

                        LicenseOwnerTaxNumber =
                            x.LicenseOwnerTaxNumber,

                        LicenseOwnerIdentityNumber =
                            x.LicenseOwnerIdentityNumber,

                        LicenseOwnerAddress =
                            x.LicenseOwnerAddress,

                        LicenseOwnerCityId =
                            x.LicenseOwnerCityId,

                        LicenseOwnerCityName =
                            x.LicenseOwnerCity == null
                                ? null
                                : x.LicenseOwnerCity.Name,

                        LicenseOwnerTaxOfficeId =
                            x.LicenseOwnerTaxOfficeId,

                        LicenseOwnerTaxOfficeName =
                            x.LicenseOwnerTaxOffice == null
                                ? null
                                : x.LicenseOwnerTaxOffice.Name,


                        AuthorizedName =
                            x.AuthorizedName,

                        AuthorizedPhone =
                            x.AuthorizedPhone,


                        InsuranceExpiryDate =
                            x.InsuranceExpiryDate,

                        InspectionExpiryDate =
                            x.InspectionExpiryDate,


                        IsActive =
                            x.IsActive,

                        RowVersion =
                            x.RowVersion,


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
                .FirstOrDefaultAsync(
                    cancellationToken);
        }


        // =====================================================
        // LEGACY LICENSE INPUT
        // =====================================================

        private static VehicleAccountInputDto?
            BuildLegacyLicenseInput(
                CreateVehicleRequest request)
        {
            var model =
                new VehicleAccountInputDto
                {
                    Title =
                        request.LicenseOwnerName,

                    TaxNumber =
                        request.LicenseOwnerTaxNumber,

                    IdentityNumber =
                        request.LicenseOwnerIdentityNumber,

                    Address =
                        request.LicenseOwnerAddress,

                    CityId =
                        NormalizeId(
                            request.LicenseOwnerCityId),

                    TaxOfficeId =
                        NormalizeId(
                            request.LicenseOwnerTaxOfficeId)
                };


            return model.HasAnyValue()
                ? model
                : null;
        }


        // =====================================================
        // NORMALIZATION
        // =====================================================

        private static string NormalizePlateForDisplay(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(
                value))
            {
                return string.Empty;
            }


            var normalizedSpaces =
                Regex.Replace(
                    value.Trim(),
                    @"\s+",
                    " ");


            return normalizedSpaces
                .ToUpper(
                    TurkishCulture);
        }


        private static string NormalizePlateForComparison(
            string? value)
        {
            var displayValue =
                NormalizePlateForDisplay(
                    value);


            return Regex.Replace(
                displayValue,
                @"\s+",
                string.Empty);
        }


        private static string? NormalizePlateOptional(
            string? value)
        {
            return string.IsNullOrWhiteSpace(
                value)
                ? null
                : NormalizePlateForDisplay(
                    value);
        }


        private static string? NormalizeOptional(
            string? value)
        {
            return string.IsNullOrWhiteSpace(
                value)
                ? null
                : value.Trim();
        }


        private static int? NormalizeId(
            int? value)
        {
            return value.HasValue &&
                   value.Value > 0
                ? value.Value
                : null;
        }
    }
}