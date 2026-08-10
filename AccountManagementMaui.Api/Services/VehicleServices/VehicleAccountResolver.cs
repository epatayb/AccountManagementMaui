using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.VehicleModels;

using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Services.VehicleServices
{
    public class VehicleAccountResolver
        : IVehicleAccountResolver
    {
        // =====================================================
        // DRIVER
        // =====================================================

        private const string DriverType =
            "Operasyon Cari";

        private const string DriverKind =
            "Navluncu";


        // =====================================================
        // REFERENCE
        // =====================================================

        private const string ReferenceType =
            "Cari";

        private const string ReferenceKind =
            "Referans";


        // =====================================================
        // LICENSE
        // =====================================================

        private const string LicenseType =
            "Operasyon Cari";

        private const string LicenseKind =
            "Navluncu";


        // =====================================================
        // INVOICE
        // =====================================================

        private const string InvoiceType =
            "Müşteri";

        private const string InvoiceKind =
            "Müşteri";


        private readonly AppDbContext _context;


        public VehicleAccountResolver(
            AppDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // DRIVER
        // =====================================================

        public Task<AccountCard?> ResolveDriverAsync(
            int? selectedAccountId,
            VehicleAccountInputDto? input,
            CancellationToken cancellationToken = default)
        {
            return ResolveAsync(
                selectedAccountId,
                input,
                DriverType,
                DriverKind,
                cancellationToken);
        }


        // =====================================================
        // REFERENCE
        // =====================================================

        public Task<AccountCard?> ResolveReferenceAsync(
            int? selectedAccountId,
            VehicleAccountInputDto? input,
            CancellationToken cancellationToken = default)
        {
            return ResolveAsync(
                selectedAccountId,
                input,
                ReferenceType,
                ReferenceKind,
                cancellationToken);
        }


        // =====================================================
        // LICENSE
        // =====================================================

        public Task<AccountCard?> ResolveLicenseAsync(
            int? selectedAccountId,
            VehicleAccountInputDto? input,
            CancellationToken cancellationToken = default)
        {
            return ResolveAsync(
                selectedAccountId,
                input,
                LicenseType,
                LicenseKind,
                cancellationToken);
        }


        // =====================================================
        // INVOICE
        // =====================================================

        public Task<AccountCard?> ResolveInvoiceAsync(
            int? selectedAccountId,
            VehicleAccountInputDto? input,
            CancellationToken cancellationToken = default)
        {
            return ResolveAsync(
                selectedAccountId,
                input,
                InvoiceType,
                InvoiceKind,
                cancellationToken);
        }


        // =====================================================
        // RESOLVE
        // =====================================================

        private async Task<AccountCard?> ResolveAsync(
            int? selectedAccountId,
            VehicleAccountInputDto? input,
            string createTypeName,
            string createKindName,
            CancellationToken cancellationToken)
        {
            // =================================================
            // EXISTING SELECTED ACCOUNT
            // =================================================

            if (selectedAccountId.HasValue &&
                selectedAccountId.Value > 0)
            {
                var selected =
                    await _context.AccountCards
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                selectedAccountId.Value,
                            cancellationToken);


                if (selected is null)
                {
                    throw new VehicleAccountResolverException(
                        "Seçilen hesap kartı bulunamadı.");
                }


                /*
                 * Mevcut cari seçilmişse Tip / Tür
                 * kontrolü veya değişikliği yapmıyoruz.
                 *
                 * Fatura hesabında bu özellikle önemli:
                 * API herhangi bir AccountCard kabul edebilir.
                 */
                return selected;
            }


            // =================================================
            // NO MANUAL INPUT
            // =================================================

            if (input is null ||
                !input.HasAnyValue())
            {
                return null;
            }


            var title =
                Normalize(
                    input.Title);


            var identityNumber =
                Normalize(
                    input.IdentityNumber);


            var taxNumber =
                Normalize(
                    input.TaxNumber);


            var phoneNumber =
                Normalize(
                    input.PhoneNumber);


            /*
             * AccountCard.Title DB seviyesinde zorunlu.
             *
             * Backend vehicle ilişkisini zorunlu tutmuyor.
             * Title yoksa yeni AccountCard açmıyoruz.
             *
             * UI gerekli yerlerde Title kontrol edecek.
             */
            if (string.IsNullOrWhiteSpace(
                title))
            {
                return null;
            }


            // =================================================
            // FIND EXISTING
            // =================================================

            var existing =
                await FindExistingAccountAsync(
                    title,
                    identityNumber,
                    taxNumber,
                    phoneNumber,
                    cancellationToken);


            if (existing is not null)
            {
                return existing;
            }


            // =================================================
            // LOCATION
            // =================================================

            var location =
                await ResolveLocationAsync(
                    input,
                    cancellationToken);


            // =================================================
            // CLASSIFICATION
            // =================================================

            var classification =
                await GetClassificationAsync(
                    createTypeName,
                    createKindName,
                    cancellationToken);


            // =================================================
            // CREATE
            // =================================================

            var item =
                new AccountCard
                {
                    Title =
                        title,

                    AccountCardTypeId =
                        classification.TypeId,

                    AccountCardKindId =
                        classification.KindId,


                    AccountCardGroupId =
                        null,

                    AccountCardSubGroupId =
                        null,


                    CityId =
                        location.CityId,

                    DistrictId =
                        location.DistrictId,

                    TaxOfficeId =
                        location.TaxOfficeId,


                    IdentityNumber =
                        identityNumber,

                    TaxNumber =
                        taxNumber,

                    PhoneNumber =
                        phoneNumber,

                    Email =
                        Normalize(
                            input.Email),

                    Address =
                        Normalize(
                            input.Address),

                    ContactPerson =
                        null
                };


            _context.AccountCards.Add(
                item);


            return item;
        }


        // =====================================================
        // FIND EXISTING ACCOUNT
        // =====================================================

        private async Task<AccountCard?>
            FindExistingAccountAsync(
                string title,
                string? identityNumber,
                string? taxNumber,
                string? phoneNumber,
                CancellationToken cancellationToken)
        {
            /*
             * Aynı HTTP request içerisinde daha önce
             * oluşturulmuş fakat henüz SaveChanges
             * yapılmamış AccountCard kayıtlarını da arıyoruz.
             *
             * Bu özellikle:
             *
             * Şoför = Ruhsat Carisi
             *
             * olduğunda ikinci AccountCard açılmasını
             * engeller.
             */

            var trackedAccounts =
                _context.ChangeTracker
                    .Entries<AccountCard>()
                    .Where(x =>
                        x.State !=
                        EntityState.Deleted)
                    .Select(x =>
                        x.Entity)
                    .ToList();


            // =================================================
            // 1. TC
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                identityNumber))
            {
                var tracked =
                    trackedAccounts
                        .FirstOrDefault(x =>
                            x.IdentityNumber ==
                            identityNumber);


                if (tracked is not null)
                {
                    return tracked;
                }


                var database =
                    await _context.AccountCards
                        .FirstOrDefaultAsync(
                            x =>
                                x.IdentityNumber ==
                                identityNumber,
                            cancellationToken);


                if (database is not null)
                {
                    return database;
                }
            }


            // =================================================
            // 2. TAX NUMBER
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                taxNumber))
            {
                var tracked =
                    trackedAccounts
                        .FirstOrDefault(x =>
                            x.TaxNumber ==
                            taxNumber);


                if (tracked is not null)
                {
                    return tracked;
                }


                var database =
                    await _context.AccountCards
                        .FirstOrDefaultAsync(
                            x =>
                                x.TaxNumber ==
                                taxNumber,
                            cancellationToken);


                if (database is not null)
                {
                    return database;
                }
            }


            // =================================================
            // 3. TITLE + PHONE
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                phoneNumber))
            {
                var tracked =
                    trackedAccounts
                        .FirstOrDefault(x =>
                            x.Title ==
                            title &&

                            x.PhoneNumber ==
                            phoneNumber);


                if (tracked is not null)
                {
                    return tracked;
                }


                var database =
                    await _context.AccountCards
                        .FirstOrDefaultAsync(
                            x =>
                                x.Title ==
                                title &&

                                x.PhoneNumber ==
                                phoneNumber,
                            cancellationToken);


                if (database is not null)
                {
                    return database;
                }
            }


            /*
             * Yalnız isme göre eşleştirmiyoruz.
             * Aynı isimde farklı kişiler olabilir.
             */
            return null;
        }


        // =====================================================
        // LOCATION
        // =====================================================

        private async Task<AccountLocation>
            ResolveLocationAsync(
                VehicleAccountInputDto input,
                CancellationToken cancellationToken)
        {
            var cityId =
                NormalizeId(
                    input.CityId);


            var districtId =
                NormalizeId(
                    input.DistrictId);


            var taxOfficeId =
                NormalizeId(
                    input.TaxOfficeId);


            // =================================================
            // DISTRICT
            // =================================================

            if (districtId.HasValue)
            {
                var district =
                    await _context.Districts
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                districtId.Value,
                            cancellationToken);


                if (district is null)
                {
                    throw new VehicleAccountResolverException(
                        "Seçilen ilçe bulunamadı.");
                }


                if (cityId.HasValue &&
                    cityId.Value !=
                    district.CityId)
                {
                    throw new VehicleAccountResolverException(
                        "Seçilen ilçe şehir ile uyumlu değil.");
                }


                cityId ??=
                    district.CityId;
            }


            // =================================================
            // TAX OFFICE
            // =================================================

            if (taxOfficeId.HasValue)
            {
                var taxOffice =
                    await _context.TaxOffices
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                taxOfficeId.Value,
                            cancellationToken);


                if (taxOffice is null)
                {
                    throw new VehicleAccountResolverException(
                        "Seçilen vergi dairesi bulunamadı.");
                }


                if (cityId.HasValue &&
                    cityId.Value !=
                    taxOffice.CityId)
                {
                    throw new VehicleAccountResolverException(
                        "Seçilen vergi dairesi şehir ile uyumlu değil.");
                }


                cityId ??=
                    taxOffice.CityId;
            }


            // =================================================
            // CITY
            // =================================================

            if (cityId.HasValue)
            {
                var cityExists =
                    await _context.Cities
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.Id ==
                                cityId.Value,
                            cancellationToken);


                if (!cityExists)
                {
                    throw new VehicleAccountResolverException(
                        "Seçilen şehir bulunamadı.");
                }
            }


            return new AccountLocation
            {
                CityId =
                    cityId,

                DistrictId =
                    districtId,

                TaxOfficeId =
                    taxOfficeId
            };
        }


        // =====================================================
        // CLASSIFICATION
        // =====================================================

        private async Task<AccountClassification>
            GetClassificationAsync(
                string typeName,
                string kindName,
                CancellationToken cancellationToken)
        {
            var type =
                await _context.AccountCardTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.TypeName ==
                            typeName,
                        cancellationToken);


            if (type is null)
            {
                throw new VehicleAccountResolverException(
                    $"'{typeName}' hesap tipi Tanımlar alanında bulunamadı.");
            }


            var kind =
                await _context.AccountCardKinds
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.AccountCardTypeId ==
                            type.Id &&

                            x.KindName ==
                            kindName,
                        cancellationToken);


            if (kind is null)
            {
                throw new VehicleAccountResolverException(
                    $"'{typeName} / {kindName}' hesap kart türü Tanımlar alanında bulunamadı.");
            }


            return new AccountClassification
            {
                TypeId =
                    type.Id,

                KindId =
                    kind.Id
            };
        }


        // =====================================================
        // HELPERS
        // =====================================================

        private static string? Normalize(
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


        // =====================================================
        // PRIVATE TYPES
        // =====================================================

        private sealed class AccountClassification
        {
            public int TypeId { get; set; }

            public int KindId { get; set; }
        }


        private sealed class AccountLocation
        {
            public int? CityId { get; set; }

            public int? DistrictId { get; set; }

            public int? TaxOfficeId { get; set; }
        }
    }
}