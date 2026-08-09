using System.Globalization;
using System.Text.RegularExpressions;

using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.VehicleModels;

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
        private const string DriverTypeName =
            "Operasyon Cari";

        private const string DriverKindName =
            "Navluncu";

        private const string InvoiceCustomerKindName =
            "Müşteri";

        private static readonly CultureInfo
            TurkishCulture =
                CultureInfo.GetCultureInfo(
                    "tr-TR");


        private readonly AppDbContext _context;


        public VehiclesController(
            AppDbContext context)
        {
            _context = context;
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

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Plate.Contains(search) ||

                    (
                        x.TrailerPlate != null &&
                        x.TrailerPlate.Contains(search)
                    ) ||

                    x.DriverAccountCard.Title.Contains(search) ||

                    (
                        x.DriverAccountCard.IdentityNumber != null &&
                        x.DriverAccountCard.IdentityNumber.Contains(search)
                    ) ||

                    (
                        x.DriverAccountCard.PhoneNumber != null &&
                        x.DriverAccountCard.PhoneNumber.Contains(search)
                    ) ||

                    x.LicenseAccountCard.Title.Contains(search) ||

                    x.InvoiceAccountCard.Title.Contains(search) ||

                    (
                        x.ReferenceAccountCard != null &&
                        x.ReferenceAccountCard.Title.Contains(search)
                    ) ||

                    x.LicenseOwnerName.Contains(search) ||

                    (
                        x.AuthorizedName != null &&
                        x.AuthorizedName.Contains(search)
                    ));
            }


            // =================================================
            // FILTERS
            // =================================================

            if (vehicleTypeId.HasValue &&
                vehicleTypeId.Value > 0)
            {
                query = query.Where(x =>
                    x.VehicleTypeId ==
                    vehicleTypeId.Value);
            }


            if (vehicleKindId.HasValue &&
                vehicleKindId.Value > 0)
            {
                query = query.Where(x =>
                    x.VehicleKindId ==
                    vehicleKindId.Value);
            }


            if (cityId.HasValue &&
                cityId.Value > 0)
            {
                query = query.Where(x =>
                    x.LicenseOwnerCityId ==
                    cityId.Value);
            }


            if (isActive.HasValue)
            {
                query = query.Where(x =>
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
                                x => x.VehicleType.TypeName)
                            : query.OrderBy(
                                x => x.VehicleType.TypeName),

                    "kind" =>
                        descending
                            ? query.OrderByDescending(
                                x => x.VehicleKind.KindName)
                            : query.OrderBy(
                                x => x.VehicleKind.KindName),

                    "driver" =>
                        descending
                            ? query.OrderByDescending(
                                x => x.DriverAccountCard.Title)
                            : query.OrderBy(
                                x => x.DriverAccountCard.Title),

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
                            Id = x.Id,

                            Plate = x.Plate,

                            VehicleTypeId =
                                x.VehicleTypeId,

                            VehicleTypeName =
                                x.VehicleType.TypeName,

                            VehicleKindId =
                                x.VehicleKindId,

                            VehicleKindName =
                                x.VehicleKind.KindName,

                            TrailerPlate =
                                x.TrailerPlate,

                            Brand =
                                x.Brand,

                            Model =
                                x.Model,

                            Country =
                                x.Country,


                            // Driver
                            DriverAccountCardId =
                                x.DriverAccountCardId,

                            DriverName =
                                x.DriverAccountCard.Title,

                            DriverIdentityNumber =
                                x.DriverAccountCard.IdentityNumber,

                            DriverPhoneNumber =
                                x.DriverAccountCard.PhoneNumber,


                            // Reference
                            ReferenceAccountCardId =
                                x.ReferenceAccountCardId,

                            ReferenceName =
                                x.ReferenceAccountCard == null
                                    ? null
                                    : x.ReferenceAccountCard.Title,

                            ReferencePhoneNumber =
                                x.ReferenceAccountCard == null
                                    ? null
                                    : x.ReferenceAccountCard.PhoneNumber,


                            // License
                            LicenseAccountCardId =
                                x.LicenseAccountCardId,

                            LicenseAccountCardName =
                                x.LicenseAccountCard.Title,


                            // Invoice
                            InvoiceAccountCardId =
                                x.InvoiceAccountCardId,

                            InvoiceAccountCardName =
                                x.InvoiceAccountCard.Title,


                            // Snapshot
                            LicenseOwnerName =
                                x.LicenseOwnerName,

                            LicenseOwnerIdentityNumber =
                                x.LicenseOwnerIdentityNumber,

                            LicenseOwnerTaxNumber =
                                x.LicenseOwnerTaxNumber,

                            LicenseOwnerCityId =
                                x.LicenseOwnerCityId,

                            LicenseOwnerCityName =
                                x.LicenseOwnerCity.Name,


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
                    Items = items,

                    Page = page,

                    PageSize = pageSize,

                    TotalCount = totalCount,

                    TotalPages = totalPages
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


            var relationResult =
                await ValidateAndResolveRelationsAsync(
                    request,
                    cancellationToken);


            if (!ModelState.IsValid ||
                relationResult is null)
            {
                return ValidationProblem(
                    ModelState);
            }


            var duplicateExists =
                await _context.Vehicles
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


                    // Driver
                    DriverAccountCardId =
                        request.DriverAccountCardId,

                    DriverIsLicenseOwner =
                        request.DriverIsLicenseOwner,


                    // Reference
                    ReferenceAccountCardId =
                        request.ReferenceAccountCardId,


                    // License
                    LicenseAccountCardId =
                        relationResult
                            .LicenseAccountCardId,


                    // Invoice
                    InvoiceAccountCardId =
                        relationResult
                            .InvoiceAccountCardId,

                    ReferenceIsInvoiceAccount =
                        request.ReferenceIsInvoiceAccount,

                    LicenseOwnerIsInvoiceAccount =
                        request.LicenseOwnerIsInvoiceAccount,


                    // Snapshot
                    LicenseOwnerName =
                        relationResult
                            .LicenseOwnerName,

                    LicenseOwnerTaxNumber =
                        relationResult
                            .LicenseOwnerTaxNumber,

                    LicenseOwnerIdentityNumber =
                        relationResult
                            .LicenseOwnerIdentityNumber,

                    LicenseOwnerAddress =
                        relationResult
                            .LicenseOwnerAddress,

                    LicenseOwnerCityId =
                        relationResult
                            .LicenseOwnerCityId,

                    LicenseOwnerTaxOfficeId =
                        relationResult
                            .LicenseOwnerTaxOfficeId,


                    // Authorized
                    AuthorizedName =
                        NormalizeOptional(
                            request.AuthorizedName),

                    AuthorizedPhone =
                        NormalizeOptional(
                            request.AuthorizedPhone),


                    // Documents
                    InsuranceExpiryDate =
                        request.InsuranceExpiryDate,

                    InspectionExpiryDate =
                        request.InspectionExpiryDate,


                    IsActive = true
                };


            _context.Vehicles.Add(item);


            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    code =
                        "VEHICLE_PLATE_EXISTS",

                    message =
                        "Bu plaka ile aktif bir araç kaydı zaten mevcut."
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
                        x => x.Id == id,
                        cancellationToken);


            if (item is null)
            {
                return NotFound(new
                {
                    message =
                        "Güncellenecek araç kaydı bulunamadı."
                });
            }


            if (request.RowVersion.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(request.RowVersion),
                    "Kayıt sürüm bilgisi zorunludur.");
            }


            var plate =
                NormalizePlateForDisplay(
                    request.Plate);


            var normalizedPlate =
                NormalizePlateForComparison(
                    plate);


            var relationResult =
                await ValidateAndResolveRelationsAsync(
                    request,
                    cancellationToken);


            if (!ModelState.IsValid ||
                relationResult is null)
            {
                return ValidationProblem(
                    ModelState);
            }


            if (item.IsActive)
            {
                var duplicateExists =
                    await _context.Vehicles
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


            item.DriverAccountCardId =
                request.DriverAccountCardId;

            item.DriverIsLicenseOwner =
                request.DriverIsLicenseOwner;


            item.ReferenceAccountCardId =
                request.ReferenceAccountCardId;


            item.LicenseAccountCardId =
                relationResult
                    .LicenseAccountCardId;


            item.InvoiceAccountCardId =
                relationResult
                    .InvoiceAccountCardId;


            item.ReferenceIsInvoiceAccount =
                request.ReferenceIsInvoiceAccount;

            item.LicenseOwnerIsInvoiceAccount =
                request.LicenseOwnerIsInvoiceAccount;


            item.LicenseOwnerName =
                relationResult
                    .LicenseOwnerName;

            item.LicenseOwnerTaxNumber =
                relationResult
                    .LicenseOwnerTaxNumber;

            item.LicenseOwnerIdentityNumber =
                relationResult
                    .LicenseOwnerIdentityNumber;

            item.LicenseOwnerAddress =
                relationResult
                    .LicenseOwnerAddress;

            item.LicenseOwnerCityId =
                relationResult
                    .LicenseOwnerCityId;

            item.LicenseOwnerTaxOfficeId =
                relationResult
                    .LicenseOwnerTaxOfficeId;


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


            _context.Entry(item)
                .Property(x => x.RowVersion)
                .OriginalValue =
                    request.RowVersion;


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
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    code =
                        "VEHICLE_PLATE_EXISTS",

                    message =
                        "Bu plaka ile aktif bir araç kaydı zaten mevcut."
                });
            }


            var response =
                await GetDetailAsync(
                    id,
                    cancellationToken);


            return Ok(response);
        }

        // =====================================================
        // RELATION VALIDATION
        // =====================================================

        private async Task<VehicleRelationResult?>
            ValidateAndResolveRelationsAsync(
                CreateVehicleRequest request,
                CancellationToken cancellationToken)
        {
            // =================================================
            // TYPE
            // =================================================

            var vehicleTypeExists =
                await _context.VehicleTypes
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


            // =================================================
            // KIND
            // =================================================

            var vehicleKindExists =
                await _context.VehicleKinds
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


            // =================================================
            // DRIVER
            // =================================================

            var driver =
                await _context.AccountCards
                    .AsNoTracking()
                    .Where(x =>
                        x.Id ==
                        request.DriverAccountCardId)
                    .Select(x =>
                        new AccountCardSnapshot
                        {
                            Id = x.Id,

                            Title =
                                x.Title,

                            TaxNumber =
                                x.TaxNumber,

                            IdentityNumber =
                                x.IdentityNumber,

                            Address =
                                x.Address,

                            CityId =
                                x.CityId,

                            TaxOfficeId =
                                x.TaxOfficeId,

                            TypeName =
                                x.AccountCardType.TypeName,

                            KindName =
                                x.AccountCardKind.KindName
                        })
                    .FirstOrDefaultAsync(
                        cancellationToken);


            if (driver is null)
            {
                ModelState.AddModelError(
                    nameof(request.DriverAccountCardId),
                    "Seçilen şoför hesabı bulunamadı.");
            }
            else if (
                !string.Equals(
                    driver.TypeName,
                    DriverTypeName,
                    StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(
                    driver.KindName,
                    DriverKindName,
                    StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    nameof(request.DriverAccountCardId),
                    "Şoför yalnızca Operasyon Cari / Navluncu hesap kartlarından seçilebilir.");
            }


            // =================================================
            // REFERENCE
            // =================================================

            if (request.ReferenceAccountCardId.HasValue)
            {
                var referenceExists =
                    await _context.AccountCards
                        .AnyAsync(
                            x =>
                                x.Id ==
                                request.ReferenceAccountCardId.Value,
                            cancellationToken);


                if (!referenceExists)
                {
                    ModelState.AddModelError(
                        nameof(request.ReferenceAccountCardId),
                        "Seçilen referans hesabı bulunamadı.");
                }
            }


            // =================================================
            // INVOICE CHECKBOX CONFLICT
            // =================================================

            if (request.ReferenceIsInvoiceAccount &&
                request.LicenseOwnerIsInvoiceAccount)
            {
                ModelState.AddModelError(
                    nameof(request.ReferenceIsInvoiceAccount),
                    "Referans ve ruhsat carisi aynı anda fatura hesabı olarak seçilemez.");
            }


            if (request.ReferenceIsInvoiceAccount &&
                !request.ReferenceAccountCardId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(request.ReferenceAccountCardId),
                    "Referans fatura carisi olarak seçildiyse referans hesabı seçilmelidir.");
            }


            // =================================================
            // LICENSE ACCOUNT
            // =================================================

            var resolvedLicenseAccountCardId = request.LicenseAccountCardId;


            var licenseAccount =
                await _context.AccountCards
                    .AsNoTracking()
                    .Where(x =>
                        x.Id ==
                        resolvedLicenseAccountCardId)
                    .Select(x =>
                        new AccountCardSnapshot
                        {
                            Id = x.Id,

                            Title =
                                x.Title,

                            TaxNumber =
                                x.TaxNumber,

                            IdentityNumber =
                                x.IdentityNumber,

                            Address =
                                x.Address,

                            CityId =
                                x.CityId,

                            TaxOfficeId =
                                x.TaxOfficeId
                        })
                    .FirstOrDefaultAsync(
                        cancellationToken);


            if (licenseAccount is null)
            {
                ModelState.AddModelError(
                    nameof(request.LicenseAccountCardId),
                    "Seçilen ruhsat carisi bulunamadı.");
            }


            // =================================================
            // INVOICE ACCOUNT
            // =================================================

            var resolvedInvoiceAccountCardId =
                request.ReferenceIsInvoiceAccount &&
                request.ReferenceAccountCardId.HasValue
                    ? request.ReferenceAccountCardId.Value

                    : request.LicenseOwnerIsInvoiceAccount
                        ? resolvedLicenseAccountCardId

                        : request.InvoiceAccountCardId;


            var invoiceAccountValid = await _context.AccountCards
                .AsNoTracking()
                .AnyAsync(x => x.Id == resolvedInvoiceAccountCardId &&
                (
                    x.AccountCardKind.KindName ==
                        InvoiceCustomerKindName ||

                    x.AccountCardKind.KindName ==
                        DriverKindName
                ),
                cancellationToken);


            if (!invoiceAccountValid)
            {
                ModelState.AddModelError(
                    nameof(request.InvoiceAccountCardId),
                    "Fatura hesabı yalnızca Müşteri veya Navluncu türündeki hesap kartlarından seçilebilir.");
            }


            // =================================================
            // SNAPSHOT
            // =================================================

            string licenseOwnerName;

            string? licenseOwnerTaxNumber;

            string? licenseOwnerIdentityNumber;

            string? licenseOwnerAddress;

            int licenseOwnerCityId;

            int? licenseOwnerTaxOfficeId;


            if (request.DriverIsLicenseOwner &&
                driver is not null)
            {
                licenseOwnerName =
                    driver.Title;

                licenseOwnerTaxNumber =
                    NormalizeOptional(
                        driver.TaxNumber);

                licenseOwnerIdentityNumber =
                    NormalizeOptional(
                        driver.IdentityNumber);

                licenseOwnerAddress =
                    NormalizeOptional(
                        driver.Address);

                licenseOwnerCityId =
                    driver.CityId ?? 0;

                licenseOwnerTaxOfficeId =
                    driver.TaxOfficeId;
            }
            else
            {
                licenseOwnerName =
                    request.LicenseOwnerName
                        ?.Trim()
                    ?? string.Empty;

                licenseOwnerTaxNumber =
                    NormalizeOptional(
                        request.LicenseOwnerTaxNumber);

                licenseOwnerIdentityNumber =
                    NormalizeOptional(
                        request.LicenseOwnerIdentityNumber);

                licenseOwnerAddress =
                    NormalizeOptional(
                        request.LicenseOwnerAddress);

                licenseOwnerCityId =
                    request.LicenseOwnerCityId;

                licenseOwnerTaxOfficeId =
                    request.LicenseOwnerTaxOfficeId;
            }


            if (string.IsNullOrWhiteSpace(
                licenseOwnerName))
            {
                ModelState.AddModelError(
                    nameof(request.LicenseOwnerName),
                    "Ruhsat sahibi adı veya ünvanı zorunludur.");
            }


            if (string.IsNullOrWhiteSpace(
                    licenseOwnerTaxNumber) &&
                string.IsNullOrWhiteSpace(
                    licenseOwnerIdentityNumber))
            {
                ModelState.AddModelError(
                    nameof(request.LicenseOwnerTaxNumber),
                    "Ruhsat sahibi için Vergi No veya TC No alanlarından en az biri girilmelidir.");
            }


            if (!string.IsNullOrWhiteSpace(
                    licenseOwnerTaxNumber) &&
                (
                    licenseOwnerTaxNumber.Length != 10 ||
                    !licenseOwnerTaxNumber.All(
                        char.IsDigit)
                ))
            {
                ModelState.AddModelError(
                    nameof(request.LicenseOwnerTaxNumber),
                    "Vergi No 10 haneli ve yalnızca rakamlardan oluşmalıdır.");
            }


            if (!string.IsNullOrWhiteSpace(
                    licenseOwnerIdentityNumber) &&
                (
                    licenseOwnerIdentityNumber.Length != 11 ||
                    !licenseOwnerIdentityNumber.All(
                        char.IsDigit)
                ))
            {
                ModelState.AddModelError(
                    nameof(request.LicenseOwnerIdentityNumber),
                    "TC No 11 haneli ve yalnızca rakamlardan oluşmalıdır.");
            }


            // =================================================
            // CITY
            // =================================================

            if (licenseOwnerCityId <= 0)
            {
                ModelState.AddModelError(
                    nameof(request.LicenseOwnerCityId),
                    "Ruhsat sahibi şehri zorunludur.");
            }
            else
            {
                var cityExists =
                    await _context.Cities
                        .AnyAsync(
                            x =>
                                x.Id ==
                                licenseOwnerCityId,
                            cancellationToken);


                if (!cityExists)
                {
                    ModelState.AddModelError(
                        nameof(request.LicenseOwnerCityId),
                        "Seçilen şehir bulunamadı.");
                }
            }


            // =================================================
            // TAX OFFICE
            // =================================================

            if (licenseOwnerTaxOfficeId.HasValue)
            {
                var taxOfficeValid =
                    await _context.TaxOffices
                        .AnyAsync(
                            x =>
                                x.Id ==
                                licenseOwnerTaxOfficeId.Value &&
                                x.CityId ==
                                licenseOwnerCityId,
                            cancellationToken);


                if (!taxOfficeValid)
                {
                    ModelState.AddModelError(
                        nameof(request.LicenseOwnerTaxOfficeId),
                        "Seçilen vergi dairesi ruhsat sahibinin şehri ile uyumlu değil.");
                }
            }


            if (!ModelState.IsValid ||
                licenseAccount is null ||
                driver is null)
            {
                return null;
            }


            return new VehicleRelationResult
            {
                LicenseAccountCardId =
                    resolvedLicenseAccountCardId,

                InvoiceAccountCardId =
                    resolvedInvoiceAccountCardId,

                LicenseOwnerName =
                    licenseOwnerName,

                LicenseOwnerTaxNumber =
                    licenseOwnerTaxNumber,

                LicenseOwnerIdentityNumber =
                    licenseOwnerIdentityNumber,

                LicenseOwnerAddress =
                    licenseOwnerAddress,

                LicenseOwnerCityId =
                    licenseOwnerCityId,

                LicenseOwnerTaxOfficeId =
                    licenseOwnerTaxOfficeId
            };
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
                        Id = x.Id,

                        Plate = x.Plate,

                        VehicleTypeId =
                            x.VehicleTypeId,

                        VehicleTypeName =
                            x.VehicleType.TypeName,

                        VehicleKindId =
                            x.VehicleKindId,

                        VehicleKindName =
                            x.VehicleKind.KindName,

                        TrailerPlate =
                            x.TrailerPlate,

                        Brand =
                            x.Brand,

                        Model =
                            x.Model,

                        Country =
                            x.Country,


                        DriverAccountCardId =
                            x.DriverAccountCardId,

                        DriverName =
                            x.DriverAccountCard.Title,

                        DriverIdentityNumber =
                            x.DriverAccountCard.IdentityNumber,

                        DriverPhoneNumber =
                            x.DriverAccountCard.PhoneNumber,

                        DriverIsLicenseOwner =
                            x.DriverIsLicenseOwner,


                        ReferenceAccountCardId =
                            x.ReferenceAccountCardId,

                        ReferenceName =
                            x.ReferenceAccountCard == null
                                ? null
                                : x.ReferenceAccountCard.Title,

                        ReferencePhoneNumber =
                            x.ReferenceAccountCard == null
                                ? null
                                : x.ReferenceAccountCard.PhoneNumber,


                        LicenseAccountCardId =
                            x.LicenseAccountCardId,

                        LicenseAccountCardName =
                            x.LicenseAccountCard.Title,


                        InvoiceAccountCardId =
                            x.InvoiceAccountCardId,

                        InvoiceAccountCardName =
                            x.InvoiceAccountCard.Title,

                        ReferenceIsInvoiceAccount =
                            x.ReferenceIsInvoiceAccount,

                        LicenseOwnerIsInvoiceAccount =
                            x.LicenseOwnerIsInvoiceAccount,


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
                            x.LicenseOwnerCity.Name,

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


        // =========================================================
        // CHANGE STATUS
        // PATCH: api/vehicles/1/status
        // =========================================================

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<VehicleDetailDto>> ChangeStatus(
            int id,
            [FromBody] ChangeVehicleStatusRequest request,
            CancellationToken cancellationToken)
        {
            var vehicle =
                await _context.Vehicles
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);


            if (vehicle is null)
            {
                return NotFound(new
                {
                    code = "VEHICLE_NOT_FOUND",

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


            // -----------------------------------------------------
            // AKTİF HALE GETİRİLİRKEN PLAKA KONTROLÜ
            // -----------------------------------------------------

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


            // -----------------------------------------------------
            // OPTIMISTIC CONCURRENCY
            // -----------------------------------------------------

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
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message =
                        "Araç durumu değiştirilirken veritabanı kısıtlaması nedeniyle işlem tamamlanamadı."
                });
            }


            var response =
                await GetDetailAsync(
                    vehicle.Id,
                    cancellationToken);


            if (response is null)
            {
                return NotFound(new
                {
                    code =
                        "VEHICLE_NOT_FOUND",

                    message =
                        "Araç durumu değiştirildi ancak güncel araç bilgileri getirilemedi."
                });
            }
            return Ok(response);
        }


        // =========================================================
        // DELETE
        // DELETE: api/vehicles/1
        // =========================================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            [FromBody] DeleteVehicleRequest request,
            CancellationToken cancellationToken)
        {
            var vehicle =
                await _context.Vehicles
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
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

                return ValidationProblem(
                    ModelState);
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

            _context.Entry(vehicle)
                .Property(x => x.RowVersion)
                .OriginalValue =
                    request.RowVersion;


            // Fiziksel silme YOK.
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
            if (string.IsNullOrWhiteSpace(
                value))
            {
                return null;
            }


            return NormalizePlateForDisplay(
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


        // =====================================================
        // PRIVATE TYPES
        // =====================================================

        private sealed class AccountCardSnapshot
        {
            public int Id { get; set; }

            public string Title { get; set; } =
                string.Empty;

            public string? TaxNumber { get; set; }

            public string? IdentityNumber { get; set; }

            public string? Address { get; set; }

            public int? CityId { get; set; }

            public int? TaxOfficeId { get; set; }

            public string? TypeName { get; set; }

            public string? KindName { get; set; }
        }


        private sealed class VehicleRelationResult
        {
            public int LicenseAccountCardId { get; set; }

            public int InvoiceAccountCardId { get; set; }

            public string LicenseOwnerName { get; set; } =
                string.Empty;

            public string? LicenseOwnerTaxNumber { get; set; }

            public string? LicenseOwnerIdentityNumber { get; set; }

            public string? LicenseOwnerAddress { get; set; }

            public int LicenseOwnerCityId { get; set; }

            public int? LicenseOwnerTaxOfficeId { get; set; }
        }
    }
}