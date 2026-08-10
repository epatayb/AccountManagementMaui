using AccountManagementMaui.Api.Data.Context;
using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.VehicleModels;

using Microsoft.EntityFrameworkCore;

namespace AccountManagementMaui.Api.Services.VehicleServices
{
    public class VehicleAccountResolver
        : IVehicleAccountResolver
    {
        private const string DriverType =
            "Operasyon Cari";

        private const string DriverKind =
            "Navluncu";


        private const string ReferenceType =
            "Cari";

        private const string ReferenceKind =
            "Referans";


        private const string LicenseType =
            "Operasyon Cari";

        private const string LicenseKind =
            "Navluncu";


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
            // -------------------------------------------------
            // 1. EXISTING SELECTED ACCOUNT
            // -------------------------------------------------

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


                // Mevcut seçili kartın türüne müdahale etmiyoruz.
                return selected;
            }


            // -------------------------------------------------
            // 2. NOTHING ENTERED
            // -------------------------------------------------

            if (input is null ||
                !input.HasAnyValue())
            {
                return null;
            }


            var title =
                Normalize(input.Title);

            var identityNumber =
                Normalize(input.IdentityNumber);

            var taxNumber =
                Normalize(input.TaxNumber);

            var phoneNumber =
                Normalize(input.PhoneNumber);


            /*
             * Backend'de alan zorunlu değil.
             *
             * Ancak AccountCard entity'sinde Title zorunlu.
             * Kullanıcı isim yazmadıysa AccountCard üretmiyoruz.
             *
             * UI tarafı bu durumu zaten engelleyecek.
             */
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }


            // -------------------------------------------------
            // 3. FIND EXISTING
            // -------------------------------------------------

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


            // -------------------------------------------------
            // 4. CREATE CLASSIFICATION
            // -------------------------------------------------

            var classification =
                await GetClassificationAsync(
                    createTypeName,
                    createKindName,
                    cancellationToken);


            // -------------------------------------------------
            // 5. CREATE
            // -------------------------------------------------

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
                        NormalizeId(input.CityId),

                    DistrictId =
                        NormalizeId(input.DistrictId),

                    TaxOfficeId =
                        NormalizeId(input.TaxOfficeId),


                    TaxNumber =
                        taxNumber,

                    IdentityNumber =
                        identityNumber,

                    PhoneNumber =
                        phoneNumber,

                    Email =
                        Normalize(input.Email),

                    Address =
                        Normalize(input.Address),

                    ContactPerson =
                        null
                };


            _context.AccountCards.Add(item);


            return item;
        }


        // =====================================================
        // FIND EXISTING
        // =====================================================

        private async Task<AccountCard?> FindExistingAccountAsync(
            string title,
            string? identityNumber,
            string? taxNumber,
            string? phoneNumber,
            CancellationToken cancellationToken)
        {
            /*
             * 1. TC en güçlü eşleşme.
             */
            if (!string.IsNullOrWhiteSpace(
                identityNumber))
            {
                var byIdentity =
                    await _context.AccountCards
                        .FirstOrDefaultAsync(
                            x =>
                                x.IdentityNumber ==
                                identityNumber,
                            cancellationToken);


                if (byIdentity is not null)
                {
                    return byIdentity;
                }
            }


            /*
             * 2. Vergi No
             */
            if (!string.IsNullOrWhiteSpace(
                taxNumber))
            {
                var byTaxNumber =
                    await _context.AccountCards
                        .FirstOrDefaultAsync(
                            x =>
                                x.TaxNumber ==
                                taxNumber,
                            cancellationToken);


                if (byTaxNumber is not null)
                {
                    return byTaxNumber;
                }
            }


            /*
             * 3. Ad + Telefon
             *
             * Sadece ada göre eşleştirmiyoruz.
             * Aynı isimde iki kişi olabilir.
             */
            if (!string.IsNullOrWhiteSpace(
                phoneNumber))
            {
                var byTitleAndPhone =
                    await _context.AccountCards
                        .FirstOrDefaultAsync(
                            x =>
                                x.Title == title &&
                                x.PhoneNumber ==
                                phoneNumber,
                            cancellationToken);


                if (byTitleAndPhone is not null)
                {
                    return byTitleAndPhone;
                }
            }


            return null;
        }


        // =====================================================
        // TYPE / KIND
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
        // NORMALIZE
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
        // PRIVATE MODEL
        // =====================================================

        private sealed class AccountClassification
        {
            public int TypeId { get; set; }

            public int KindId { get; set; }
        }
    }
}