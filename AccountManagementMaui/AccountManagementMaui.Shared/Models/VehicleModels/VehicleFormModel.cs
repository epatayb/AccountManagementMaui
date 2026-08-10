using System.ComponentModel.DataAnnotations;
using AccountManagementMaui.Shared.Models.VehicleLookupModels;

namespace AccountManagementMaui.Shared.Models.VehicleModels
{
    public class VehicleFormModel : IValidatableObject
    {
        // =====================================================
        // VEHICLE
        // =====================================================

        [Required(ErrorMessage = "Plaka zorunludur.")]
        [StringLength(20, ErrorMessage = "Plaka en fazla 20 karakter olabilir.")]
        public string Plate { get; set; } = string.Empty;


        [Range(1, int.MaxValue, ErrorMessage = "Araç tipi seçiniz.")]
        public int VehicleTypeId { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Araç türü seçiniz.")]
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

        public bool DriverIsLicenseOwner { get; set; }


        // =====================================================
        // REFERENCE
        // =====================================================

        public int? ReferenceAccountCardId { get; set; }


        // =====================================================
        // LICENSE ACCOUNT
        // =====================================================

        public int? LicenseAccountCardId { get; set; }


        // =====================================================
        // INVOICE
        // =====================================================

        public int? InvoiceAccountCardId { get; set; }


        public bool ReferenceIsInvoiceAccount { get; set; }

        public bool LicenseOwnerIsInvoiceAccount { get; set; }


        // =====================================================
        // LICENSE OWNER SNAPSHOT
        // =====================================================

        public string? LicenseOwnerName { get; set; } = string.Empty;


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

        public byte[] RowVersion { get; set; } = [];


        // =====================================================
        // CUSTOM VALIDATION
        // =====================================================

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            
            if (!string.IsNullOrWhiteSpace(
                    LicenseOwnerTaxNumber) &&
                (
                    LicenseOwnerTaxNumber.Length != 10 ||
                    !LicenseOwnerTaxNumber.All(char.IsDigit)
                ))
            {
                yield return new ValidationResult("Vergi No 10 haneli ve yalnızca rakamlardan oluşmalıdır.",
                    new[]
                    {
                        nameof(LicenseOwnerTaxNumber)
                    });
            }


            if (!string.IsNullOrWhiteSpace(LicenseOwnerIdentityNumber) &&
                (
                    LicenseOwnerIdentityNumber.Length != 11 ||
                    !LicenseOwnerIdentityNumber.All(char.IsDigit)
                ))
            {
                yield return new ValidationResult("TC No 11 haneli ve yalnızca rakamlardan oluşmalıdır.",
                    new[]
                    {
                        nameof(LicenseOwnerIdentityNumber)
                    });
            }


            if (ReferenceIsInvoiceAccount &&
                LicenseOwnerIsInvoiceAccount)
            {
                yield return new ValidationResult("Referans ve ruhsat carisi aynı anda fatura hesabı olarak seçilemez.",
                    new[]
                    {
                        nameof(ReferenceIsInvoiceAccount),
                        nameof(LicenseOwnerIsInvoiceAccount)
                    });
            }


            if (ReferenceIsInvoiceAccount && !ReferenceAccountCardId.HasValue)
            {
                yield return new ValidationResult("Referans fatura carisi seçeneği için önce referans seçilmelidir.",
                    new[]
                    {
                        nameof(ReferenceAccountCardId)
                    });
            }
        }
    }


    // =========================================================
    // FORM STATE
    // =========================================================

    public class VehicleFormState
    {
        public VehicleFormModel Model { get; set; } = new();


        public VehicleAccountLookupDto? Driver { get; set; }

        public VehicleAccountLookupDto? Reference { get; set; }

        public VehicleAccountLookupDto? LicenseAccount { get; set; }

        public VehicleAccountLookupDto? InvoiceAccount { get; set; }
    }
}