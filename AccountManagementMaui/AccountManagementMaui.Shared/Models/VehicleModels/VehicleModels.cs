using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Shared.Models.VehicleModels
{
    public class VehicleListDto
    {
        public int Id { get; set; }

        public string Plate { get; set; } = string.Empty;


        public int VehicleTypeId { get; set; }

        public string VehicleTypeName { get; set; } =
            string.Empty;


        public int VehicleKindId { get; set; }

        public string VehicleKindName { get; set; } =
            string.Empty;


        public string? TrailerPlate { get; set; }

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? Country { get; set; }


        // Driver
        public int? DriverAccountCardId { get; set; }

        public string? DriverName { get; set; } = string.Empty;

        public string? DriverIdentityNumber { get; set; }

        public string? DriverPhoneNumber { get; set; }


        // Reference
        public int? ReferenceAccountCardId { get; set; }

        public string? ReferenceName { get; set; }

        public string? ReferencePhoneNumber { get; set; }


        // License
        public int? LicenseAccountCardId { get; set; }

        public string? LicenseAccountCardName { get; set; } =
            string.Empty;


        // Invoice
        public int? InvoiceAccountCardId { get; set; }

        public string? InvoiceAccountCardName { get; set; } =
            string.Empty;


        // License owner snapshot
        public string? LicenseOwnerName { get; set; } =
            string.Empty;

        public string? LicenseOwnerIdentityNumber { get; set; }

        public string? LicenseOwnerTaxNumber { get; set; }

        public int? LicenseOwnerCityId { get; set; }

        public string? LicenseOwnerCityName { get; set; } =
            string.Empty;


        // Authorized
        public string? AuthorizedName { get; set; }

        public string? AuthorizedPhone { get; set; }


        // Documents
        public DateTime? InsuranceExpiryDate { get; set; }

        public DateTime? InspectionExpiryDate { get; set; }


        public bool IsActive { get; set; }


        // Audit
        public DateTime CreatedDate { get; set; }

        public string? CreatedByUserFullName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedByUserFullName { get; set; }
    }


    public class VehicleListResponse
    {
        public List<VehicleListDto> Items { get; set; } = [];

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }


    public class VehicleDetailDto
    {
        public int Id { get; set; }


        public string Plate { get; set; } =
            string.Empty;


        public int VehicleTypeId { get; set; }

        public string VehicleTypeName { get; set; } =
            string.Empty;


        public int VehicleKindId { get; set; }

        public string VehicleKindName { get; set; } =
            string.Empty;


        public string? TrailerPlate { get; set; }

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? Country { get; set; }


        // Driver
        public int? DriverAccountCardId { get; set; }

        public string? DriverName { get; set; } =
            string.Empty;

        public string? DriverIdentityNumber { get; set; }

        public string? DriverPhoneNumber { get; set; }

        public bool DriverIsLicenseOwner { get; set; }


        // Reference
        public int? ReferenceAccountCardId { get; set; }

        public string? ReferenceName { get; set; }

        public string? ReferencePhoneNumber { get; set; }


        // License account
        public int? LicenseAccountCardId { get; set; }

        public string? LicenseAccountCardName { get; set; }


        // Invoice
        public int? InvoiceAccountCardId { get; set; }

        public string? InvoiceAccountCardName { get; set; } =
            string.Empty;

        public bool ReferenceIsInvoiceAccount { get; set; }

        public bool LicenseOwnerIsInvoiceAccount { get; set; }


        // License owner
        public string? LicenseOwnerName { get; set; } =
            string.Empty;

        public string? LicenseOwnerTaxNumber { get; set; }

        public string? LicenseOwnerIdentityNumber { get; set; }

        public string? LicenseOwnerAddress { get; set; }

        public int? LicenseOwnerCityId { get; set; }

        public string? LicenseOwnerCityName { get; set; } =
            string.Empty;

        public int? LicenseOwnerTaxOfficeId { get; set; }

        public string? LicenseOwnerTaxOfficeName { get; set; }


        // Authorized
        public string? AuthorizedName { get; set; }

        public string? AuthorizedPhone { get; set; }


        // Documents
        public DateTime? InsuranceExpiryDate { get; set; }

        public DateTime? InspectionExpiryDate { get; set; }


        public bool IsActive { get; set; }


        // Concurrency
        public byte[] RowVersion { get; set; } = [];


        // Audit
        public DateTime CreatedDate { get; set; }

        public int? CreatedByUserId { get; set; }

        public string? CreatedByUserFullName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public string? ModifiedByUserFullName { get; set; }
    }


    public class CreateVehicleRequest
    {
        // =====================================================
        // VEHICLE
        // =====================================================

        [Required(
            ErrorMessage = "Plaka zorunludur.")]
        [StringLength(
            20,
            ErrorMessage = "Plaka en fazla 20 karakter olabilir.")]
        public string Plate { get; set; } =
            string.Empty;


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Araç tipi seçiniz.")]
        public int VehicleTypeId { get; set; }


        // Araç Türü ZORUNLU
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Araç türü seçiniz.")]
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

        public VehicleAccountInputDto?
            DriverAccount
        { get; set; }

        public bool DriverIsLicenseOwner { get; set; }


        // =====================================================
        // REFERENCE
        // =====================================================

        public int? ReferenceAccountCardId { get; set; }

        public VehicleAccountInputDto?
            ReferenceAccount
        { get; set; }


        // =====================================================
        // LICENSE
        // =====================================================

        public int? LicenseAccountCardId { get; set; }

        public VehicleAccountInputDto?
            LicenseAccount
        { get; set; }


        // =====================================================
        // INVOICE
        // =====================================================

        public int? InvoiceAccountCardId { get; set; }

        public VehicleAccountInputDto?
            InvoiceAccount
        { get; set; }


        public bool ReferenceIsInvoiceAccount { get; set; }

        public bool LicenseOwnerIsInvoiceAccount { get; set; }


        // =====================================================
        // LICENSE OWNER SNAPSHOT
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
    }


    public class UpdateVehicleRequest
        : CreateVehicleRequest
    {
        [Required(
            ErrorMessage =
                "Kayıt sürüm bilgisi zorunludur.")]
        public byte[] RowVersion { get; set; } = [];
    }


    public class ChangeVehicleStatusRequest
    {
        public bool IsActive { get; set; }


        [Required(
            ErrorMessage =
                "Kayıt sürüm bilgisi zorunludur.")]
        public byte[] RowVersion { get; set; } = [];
    }


    public class DeleteVehicleRequest
    {
        [Required(
            ErrorMessage =
                "Silme açıklaması zorunludur.")]
        [StringLength(
            500,
            ErrorMessage =
                "Silme açıklaması en fazla 500 karakter olabilir.")]
        public string DeleteReason { get; set; } =
            string.Empty;

        [Required(
        ErrorMessage = "Kayıt sürüm bilgisi zorunludur.")]
        public byte[] RowVersion { get; set; } = [];
    }

    public class VehicleAccountInputDto
    {
        public string? Title { get; set; }


        public string? IdentityNumber { get; set; }


        public string? TaxNumber { get; set; }


        public string? PhoneNumber { get; set; }


        public string? Email { get; set; }


        public string? Address { get; set; }


        public int? CityId { get; set; }


        public int? DistrictId { get; set; }


        public int? TaxOfficeId { get; set; }


        public bool HasAnyValue()
        {
            return
                !string.IsNullOrWhiteSpace(
                    Title) ||

                !string.IsNullOrWhiteSpace(
                    IdentityNumber) ||

                !string.IsNullOrWhiteSpace(
                    TaxNumber) ||

                !string.IsNullOrWhiteSpace(
                    PhoneNumber) ||

                !string.IsNullOrWhiteSpace(
                    Email) ||

                !string.IsNullOrWhiteSpace(
                    Address) ||

                CityId.HasValue ||

                DistrictId.HasValue ||

                TaxOfficeId.HasValue;
        }
    }
}