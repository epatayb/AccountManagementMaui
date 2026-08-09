using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Models.VehicleLookupModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/vehicle-lookups")]
    public class VehicleLookupsController : ControllerBase
    {
        private const string DriverTypeName =
            "Operasyon Cari";

        private const string DriverKindName =
            "Navluncu";

        private const string CustomerKindName =
            "Müşteri";

        private const string CarrierKindName =
            "Navluncu";

        private readonly AppDbContext _context;


        public VehicleLookupsController(
            AppDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // DRIVER LOOKUP
        // =====================================================
        // GET:
        // api/vehicle-lookups/drivers?search=ahmet
        // =====================================================

        [HttpGet("drivers")]
        public async Task<ActionResult<List<VehicleAccountLookupDto>>>
            GetDrivers(
                [FromQuery] string? search,
                [FromQuery] int take = 30,
                CancellationToken cancellationToken = default)
        {
            search =
                search?.Trim();


            take =
                NormalizeTake(take);


            var query =
                _context.AccountCards
                    .AsNoTracking()
                    .Where(x =>
                        x.AccountCardType.TypeName ==
                        DriverTypeName &&

                        x.AccountCardKind.KindName ==
                        DriverKindName);


            if (!string.IsNullOrWhiteSpace(search))
            {
                query =
                    query.Where(x =>
                        x.Title.Contains(search) ||

                        x.AccountCode.Contains(search) ||

                        (
                            x.PhoneNumber != null &&
                            x.PhoneNumber.Contains(search)
                        ) ||

                        (
                            x.IdentityNumber != null &&
                            x.IdentityNumber.Contains(search)
                        ));
            }


            var items =
                await query
                    .OrderBy(x => x.Title)
                    .ThenBy(x => x.AccountCode)
                    .Take(take)
                    .Select(x =>
                        new VehicleAccountLookupDto
                        {
                            Id =
                                x.Id,

                            AccountCode =
                                x.AccountCode,

                            Title =
                                x.Title,

                            AccountCardTypeName =
                                x.AccountCardType.TypeName,

                            AccountCardKindName =
                                x.AccountCardKind.KindName,

                            PhoneNumber =
                                x.PhoneNumber,

                            CityName =
                                x.City == null
                                    ? null
                                    : x.City.Name
                        })
                    .ToListAsync(
                        cancellationToken);


            return Ok(items);
        }


        // =====================================================
        // GENERAL ACCOUNT LOOKUP
        // =====================================================
        // Referans
        // Ruhsat Carisi
        // Fatura Hesabı
        //
        // GET:
        // api/vehicle-lookups/accounts?search=lojistik
        // =====================================================

        [HttpGet("accounts")]
        public async Task<ActionResult<List<VehicleAccountLookupDto>>>
            GetAccounts(
                [FromQuery] string? search,
                [FromQuery] int take = 30,
                CancellationToken cancellationToken = default)
        {
            search =
                search?.Trim();


            take =
                NormalizeTake(take);


            var query =
                _context.AccountCards
                    .AsNoTracking()
                    .AsQueryable();


            if (!string.IsNullOrWhiteSpace(search))
            {
                query =
                    query.Where(x =>
                        x.Title.Contains(search) ||

                        x.AccountCode.Contains(search) ||

                        (
                            x.PhoneNumber != null &&
                            x.PhoneNumber.Contains(search)
                        ) ||

                        (
                            x.TaxNumber != null &&
                            x.TaxNumber.Contains(search)
                        ) ||

                        (
                            x.IdentityNumber != null &&
                            x.IdentityNumber.Contains(search)
                        ));
            }


            var items =
                await query
                    .OrderBy(x => x.Title)
                    .ThenBy(x => x.AccountCode)
                    .Take(take)
                    .Select(x =>
                        new VehicleAccountLookupDto
                        {
                            Id =
                                x.Id,

                            AccountCode =
                                x.AccountCode,

                            Title =
                                x.Title,

                            AccountCardTypeName =
                                x.AccountCardType.TypeName,

                            AccountCardKindName =
                                x.AccountCardKind.KindName,

                            PhoneNumber =
                                x.PhoneNumber,

                            CityName =
                                x.City == null
                                    ? null
                                    : x.City.Name
                        })
                    .ToListAsync(
                        cancellationToken);


            return Ok(items);
        }


        // =====================================================
        // ACCOUNT DETAIL
        // =====================================================
        // Seçim sonrası kullanılır.
        //
        // GET:
        // api/vehicle-lookups/accounts/15
        // =====================================================

        [HttpGet("accounts/{id:int}")]
        public async Task<ActionResult<VehicleAccountLookupDetailDto>>
            GetAccountById(
                int id,
                CancellationToken cancellationToken)
        {
            var item =
                await _context.AccountCards
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == id)
                    .Select(x =>
                        new VehicleAccountLookupDetailDto
                        {
                            Id =
                                x.Id,

                            AccountCode =
                                x.AccountCode,

                            Title =
                                x.Title,


                            // Classification
                            AccountCardTypeId =
                                x.AccountCardTypeId,

                            AccountCardTypeName =
                                x.AccountCardType.TypeName,

                            AccountCardKindId =
                                x.AccountCardKindId,

                            AccountCardKindName =
                                x.AccountCardKind.KindName,


                            // Tax / identity
                            TaxNumber =
                                x.TaxNumber,

                            IdentityNumber =
                                x.IdentityNumber,


                            // Contact
                            PhoneNumber =
                                x.PhoneNumber,

                            Email =
                                x.Email,

                            ContactPerson =
                                x.ContactPerson,


                            // Address
                            Address =
                                x.Address,


                            // City
                            CityId =
                                x.CityId,

                            CityName =
                                x.City == null
                                    ? null
                                    : x.City.Name,


                            // District
                            DistrictId =
                                x.DistrictId,

                            DistrictName =
                                x.District == null
                                    ? null
                                    : x.District.Name,


                            // Tax Office
                            TaxOfficeId =
                                x.TaxOfficeId,

                            TaxOfficeName =
                                x.TaxOffice == null
                                    ? null
                                    : x.TaxOffice.Name
                        })
                    .FirstOrDefaultAsync(
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

        // GET:
        // api/vehicle-lookups/invoice-accounts?search=abc
        // =====================================================

        [HttpGet("invoice-accounts")]
        public async Task<ActionResult<List<VehicleAccountLookupDto>>>
            GetInvoiceAccounts(
                [FromQuery] string? search,
                [FromQuery] int take = 30,
                CancellationToken cancellationToken = default)
        {
            search =
                search?.Trim();


            take =
                NormalizeTake(take);


            var query =
                _context.AccountCards
                    .AsNoTracking()
                    .Where(x =>
                        x.AccountCardKind.KindName ==
                            CustomerKindName ||

                        x.AccountCardKind.KindName ==
                            CarrierKindName);


            if (!string.IsNullOrWhiteSpace(
                search))
            {
                query =
                    query.Where(x =>
                        x.Title.Contains(search) ||

                        x.AccountCode.Contains(search) ||

                        (
                            x.PhoneNumber != null &&
                            x.PhoneNumber.Contains(search)
                        ) ||

                        (
                            x.TaxNumber != null &&
                            x.TaxNumber.Contains(search)
                        ) ||

                        (
                            x.IdentityNumber != null &&
                            x.IdentityNumber.Contains(search)
                        ));
            }


            var items =
                await query
                    .OrderBy(x =>
                        x.Title)
                    .ThenBy(x =>
                        x.AccountCode)
                    .Take(take)
                    .Select(x =>
                        new VehicleAccountLookupDto
                        {
                            Id =
                                x.Id,

                            AccountCode =
                                x.AccountCode,

                            Title =
                                x.Title,

                            AccountCardTypeName =
                                x.AccountCardType.TypeName,

                            AccountCardKindName =
                                x.AccountCardKind.KindName,

                            PhoneNumber =
                                x.PhoneNumber,

                            CityName =
                                x.City == null
                                    ? null
                                    : x.City.Name
                        })
                    .ToListAsync(
                        cancellationToken);


            return Ok(items);
        }


        [HttpGet("cities")]
        public async Task<ActionResult<List<VehicleCityLookupDto>>> GetCities(CancellationToken cancellationToken)
        {
            var items =
                await _context.Cities
                    .AsNoTracking()
                    .OrderBy(x => x.CityCode)
                    .Select(x =>
                        new VehicleCityLookupDto
                        {
                            Id = x.Id,

                            CityCode = x.CityCode,

                            Name = x.Name
                        })
                    .ToListAsync(
                        cancellationToken);


            return Ok(items);
        }


        [HttpGet("tax-offices")]
        public async Task<ActionResult<List<VehicleTaxOfficeLookupDto>>>
            GetTaxOffices(
                [FromQuery] int? cityId,
                CancellationToken cancellationToken)
        {
            var query =
                _context.TaxOffices
                    .AsNoTracking()
                    .AsQueryable();


            if (cityId.HasValue &&
                cityId.Value > 0)
            {
                query =
                    query.Where(x =>
                        x.CityId == cityId.Value);
            }


            var items =
                await query
                    .OrderBy(x => x.Name)
                    .Select(x =>
                        new VehicleTaxOfficeLookupDto
                        {
                            Id = x.Id,

                            TaxOfficeCode =
                                x.TaxOfficeCode,

                            Name =
                                x.Name,

                            CityId =
                                x.CityId
                        })
                    .ToListAsync(
                        cancellationToken);


            return Ok(items);
        }

        // =====================================================
        // HELPERS
        // =====================================================

        private static int NormalizeTake(
            int take)
        {
            if (take < 1)
            {
                return 30;
            }


            if (take > 50)
            {
                return 50;
            }


            return take;
        }
    }
}