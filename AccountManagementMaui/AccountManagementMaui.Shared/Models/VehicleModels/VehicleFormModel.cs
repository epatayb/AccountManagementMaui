using System.ComponentModel.DataAnnotations;

using AccountManagementMaui.Shared.Models.VehicleLookupModels;

namespace AccountManagementMaui.Shared.Models.VehicleModels
{
    public class VehicleFormModel
        : IValidatableObject
    {
        // =====================================================
        // VEHICLE
        // =====================================================

        [Required(
            ErrorMessage = "Plaka zorunludur.")]
        [StringLength(
            20,
            ErrorMessage =
                "Plaka en fazla 20 karakter olabilir.")]
        public string Plate { get; set; } =
            string.Empty;


        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Araç tipi seçiniz.")]
        public int VehicleTypeId { get; set; }


        // Araç Türü ZORUNLU
        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Araç türü seçiniz.")]
        public int VehicleKindId { get; set; }


        [StringLength(20)]
        public string? TrailerPlate { get; set; }


        [StringLength(100)]
        public string? Brand { get; set; }


        [StringLength(100)]
        public string? Model { get; set; }


        [StringLength(100)]
        public string? Country { get; set; }


        // =====================================================
        // DRIVER
        // =====================================================

        public int? DriverAccountCardId { get; set; }


        /*
         * Checkbox yalnız UI davranışını kontrol eder.
         *
         * Backend yine TC / Vergi No / Ad+Telefon
         * üzerinden mevcut kayıt kontrolü yapar.
         */
        public bool IsNewDriver { get; set; }


        public VehicleAccountInputDto DriverAccount { get; set; } =
            new();


        public bool DriverIsLicenseOwner { get; set; }


        // =====================================================
        // REFERENCE
        // =====================================================

        public int? ReferenceAccountCardId { get; set; }


        public VehicleAccountInputDto ReferenceAccount { get; set; } =
            new();


        // =====================================================
        // LICENSE
        // =====================================================

        public int? LicenseAccountCardId { get; set; }


        public VehicleAccountInputDto LicenseAccount { get; set; } =
            new();


        // =====================================================
        // INVOICE
        // =====================================================

        public int? InvoiceAccountCardId { get; set; }


        public VehicleAccountInputDto InvoiceAccount { get; set; } =
            new();


        public bool ReferenceIsInvoiceAccount { get; set; }


        public bool LicenseOwnerIsInvoiceAccount { get; set; }


        // =====================================================
        // LICENSE SNAPSHOT
        // =====================================================

        [StringLength(200)]
        public string? LicenseOwnerName { get; set; }


        [StringLength(10)]
        public string? LicenseOwnerTaxNumber { get; set; }


        [StringLength(11)]
        public string? LicenseOwnerIdentityNumber { get; set; }


        [StringLength(500)]
        public string? LicenseOwnerAddress { get; set; }


        public int? LicenseOwnerCityId { get; set; }


        public int? LicenseOwnerTaxOfficeId { get; set; }


        // =====================================================
        // AUTHORIZED
        // =====================================================

        [StringLength(100)]
        public string? AuthorizedName { get; set; }


        [StringLength(20)]
        public string? AuthorizedPhone { get; set; }


        // =====================================================
        // DOCUMENTS
        // =====================================================

        public DateTime? InsuranceExpiryDate { get; set; }


        public DateTime? InspectionExpiryDate { get; set; }


        // =====================================================
        // CONCURRENCY
        // =====================================================

        public byte[] RowVersion { get; set; } =
            [];


        // =====================================================
        // VALIDATION
        // =====================================================

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            // =================================================
            // DRIVER
            //
            // UI'DA ŞOFÖR ZORUNLU.
            //
            // Mevcut kart yoksa:
            // Ad + Telefon zorunlu.
            // =================================================

            if (!DriverAccountCardId.HasValue)
            {
                if (string.IsNullOrWhiteSpace(
                    DriverAccount.Title))
                {
                    yield return new ValidationResult(
                        "Şoför adı veya ünvanı zorunludur.",
                        new[]
                        {
                            nameof(DriverAccount)
                        });
                }


                if (string.IsNullOrWhiteSpace(
                    DriverAccount.PhoneNumber))
                {
                    yield return new ValidationResult(
                        "Şoför telefonu zorunludur.",
                        new[]
                        {
                            nameof(DriverAccount)
                        });
                }
            }


            var driverTcError =
                ValidateIdentityNumber(
                    DriverAccount.IdentityNumber,
                    "Şoför");


            if (driverTcError is not null)
            {
                yield return driverTcError;
            }


            // =================================================
            // REFERENCE
            //
            // Tamamen opsiyonel.
            //
            // Ancak herhangi bir manuel alan girildiyse
            // Ad / Ünvan zorunlu.
            // =================================================

            if (!ReferenceAccountCardId.HasValue &&
                ReferenceAccount.HasAnyValue() &&
                string.IsNullOrWhiteSpace(
                    ReferenceAccount.Title))
            {
                yield return new ValidationResult(
                    "Yeni referans için ad veya ünvan giriniz.",
                    new[]
                    {
                        nameof(ReferenceAccount)
                    });
            }


            var referenceTcError =
                ValidateIdentityNumber(
                    ReferenceAccount.IdentityNumber,
                    "Referans");


            if (referenceTcError is not null)
            {
                yield return referenceTcError;
            }


            // =================================================
            // LICENSE ACCOUNT
            // =================================================

            if (!DriverIsLicenseOwner &&
                !LicenseAccountCardId.HasValue &&
                LicenseAccount.HasAnyValue() &&
                string.IsNullOrWhiteSpace(
                    LicenseAccount.Title))
            {
                yield return new ValidationResult(
                    "Yeni ruhsat carisi için ad veya ünvan giriniz.",
                    new[]
                    {
                        nameof(LicenseAccount)
                    });
            }


            var licenseTcError =
                ValidateIdentityNumber(
                    LicenseAccount.IdentityNumber,
                    "Ruhsat carisi");


            if (licenseTcError is not null)
            {
                yield return licenseTcError;
            }


            // =================================================
            // INVOICE ACCOUNT
            // =================================================

            if (!ReferenceIsInvoiceAccount &&
                !LicenseOwnerIsInvoiceAccount &&
                !InvoiceAccountCardId.HasValue &&
                InvoiceAccount.HasAnyValue() &&
                string.IsNullOrWhiteSpace(
                    InvoiceAccount.Title))
            {
                yield return new ValidationResult(
                    "Yeni fatura carisi için ad veya ünvan giriniz.",
                    new[]
                    {
                        nameof(InvoiceAccount)
                    });
            }


            var invoiceTcError =
                ValidateIdentityNumber(
                    InvoiceAccount.IdentityNumber,
                    "Fatura carisi");


            if (invoiceTcError is not null)
            {
                yield return invoiceTcError;
            }


            // =================================================
            // INVOICE FLAGS
            // =================================================

            if (ReferenceIsInvoiceAccount &&
                LicenseOwnerIsInvoiceAccount)
            {
                yield return new ValidationResult(
                    "Referans ve ruhsat carisi aynı anda fatura hesabı olarak seçilemez.",
                    new[]
                    {
                        nameof(ReferenceIsInvoiceAccount),
                        nameof(LicenseOwnerIsInvoiceAccount)
                    });
            }


            if (ReferenceIsInvoiceAccount &&
                !ReferenceAccountCardId.HasValue &&
                !ReferenceAccount.HasAnyValue())
            {
                yield return new ValidationResult(
                    "Referans fatura carisi olarak kullanılacaksa mevcut referans seçin veya referans bilgisi girin.",
                    new[]
                    {
                        nameof(ReferenceAccount)
                    });
            }


            if (LicenseOwnerIsInvoiceAccount &&
                !DriverIsLicenseOwner &&
                !LicenseAccountCardId.HasValue &&
                !LicenseAccount.HasAnyValue())
            {
                yield return new ValidationResult(
                    "Ruhsat carisi fatura carisi olacaksa mevcut cari seçin veya ruhsat cari bilgisi girin.",
                    new[]
                    {
                        nameof(LicenseAccount)
                    });
            }


            // =================================================
            // LEGACY LICENSE SNAPSHOT
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                    LicenseOwnerTaxNumber) &&
                (
                    LicenseOwnerTaxNumber.Length != 10 ||
                    !LicenseOwnerTaxNumber.All(
                        char.IsDigit)
                ))
            {
                yield return new ValidationResult(
                    "Vergi No 10 haneli ve yalnızca rakamlardan oluşmalıdır.",
                    new[]
                    {
                        nameof(LicenseOwnerTaxNumber)
                    });
            }


            if (!string.IsNullOrWhiteSpace(
                    LicenseOwnerIdentityNumber) &&
                (
                    LicenseOwnerIdentityNumber.Length != 11 ||
                    !LicenseOwnerIdentityNumber.All(
                        char.IsDigit)
                ))
            {
                yield return new ValidationResult(
                    "TC No 11 haneli ve yalnızca rakamlardan oluşmalıdır.",
                    new[]
                    {
                        nameof(LicenseOwnerIdentityNumber)
                    });
            }
        }


        // =====================================================
        // IDENTITY VALIDATION
        // =====================================================

        private static ValidationResult?
            ValidateIdentityNumber(
                string? identityNumber,
                string title)
        {
            if (string.IsNullOrWhiteSpace(
                identityNumber))
            {
                return null;
            }


            if (identityNumber.Length == 11 &&
                identityNumber.All(
                    char.IsDigit))
            {
                return null;
            }


            return new ValidationResult(
                $"{title} TC No 11 haneli ve yalnızca rakamlardan oluşmalıdır.");
        }
    }


    // =========================================================
    // FORM STATE
    // =========================================================

    public class VehicleFormState
    {
        public VehicleFormModel Model { get; set; } =
            new();


        public VehicleAccountLookupDto? Driver { get; set; }


        public VehicleAccountLookupDto? Reference { get; set; }


        public VehicleAccountLookupDto? LicenseAccount { get; set; }


        public VehicleAccountLookupDto? InvoiceAccount { get; set; }
    }
}