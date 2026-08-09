using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.VehicleModels
{
    public class VehicleListDto
    {
        public int Id { get; set; }


        // Araç
        public string Plate { get; set; } = string.Empty;

        public int VehicleTypeId { get; set; }

        public string VehicleTypeName { get; set; } = string.Empty;

        public int VehicleKindId { get; set; }

        public string VehicleKindName { get; set; } = string.Empty;

        public string? TrailerPlate { get; set; }

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? Country { get; set; }


        // Şoför
        public int DriverAccountCardId { get; set; }

        public string DriverName { get; set; } = string.Empty;

        public string? DriverIdentityNumber { get; set; }

        public string? DriverPhoneNumber { get; set; }


        // Referans
        public int? ReferenceAccountCardId { get; set; }

        public string? ReferenceName { get; set; }

        public string? ReferencePhoneNumber { get; set; }


        // Ruhsat carisi
        public int LicenseAccountCardId { get; set; }

        public string LicenseAccountCardName { get; set; } = string.Empty;


        // Fatura hesabı
        public int InvoiceAccountCardId { get; set; }

        public string InvoiceAccountCardName { get; set; } = string.Empty;


        // Ruhsat sahibi snapshot
        public string LicenseOwnerName { get; set; } = string.Empty;

        public string? LicenseOwnerIdentityNumber { get; set; }

        public string? LicenseOwnerTaxNumber { get; set; }

        public int LicenseOwnerCityId { get; set; }

        public string LicenseOwnerCityName { get; set; } = string.Empty;


        // Yetkili
        public string? AuthorizedName { get; set; }

        public string? AuthorizedPhone { get; set; }


        // Belge
        public DateTime? InsuranceExpiryDate { get; set; }

        public DateTime? InspectionExpiryDate { get; set; }


        // Durum
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


        // Araç
        public string Plate { get; set; } = string.Empty;

        public int VehicleTypeId { get; set; }

        public string VehicleTypeName { get; set; } = string.Empty;

        public int VehicleKindId { get; set; }

        public string VehicleKindName { get; set; } = string.Empty;

        public string? TrailerPlate { get; set; }

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? Country { get; set; }


        // Şoför
        public int DriverAccountCardId { get; set; }

        public string DriverName { get; set; } = string.Empty;

        public string? DriverIdentityNumber { get; set; }

        public string? DriverPhoneNumber { get; set; }

        public bool DriverIsLicenseOwner { get; set; }


        // Referans
        public int? ReferenceAccountCardId { get; set; }

        public string? ReferenceName { get; set; }

        public string? ReferencePhoneNumber { get; set; }


        // Ruhsat carisi
        public int LicenseAccountCardId { get; set; }

        public string LicenseAccountCardName { get; set; } = string.Empty;


        // Fatura
        public int InvoiceAccountCardId { get; set; }

        public string InvoiceAccountCardName { get; set; } = string.Empty;

        public bool ReferenceIsInvoiceAccount { get; set; }

        public bool LicenseOwnerIsInvoiceAccount { get; set; }


        // Ruhsat sahibi
        public string LicenseOwnerName { get; set; } = string.Empty;

        public string? LicenseOwnerTaxNumber { get; set; }

        public string? LicenseOwnerIdentityNumber { get; set; }

        public string? LicenseOwnerAddress { get; set; }

        public int LicenseOwnerCityId { get; set; }

        public string LicenseOwnerCityName { get; set; } = string.Empty;

        public int? LicenseOwnerTaxOfficeId { get; set; }

        public string? LicenseOwnerTaxOfficeName { get; set; }


        // Yetkili
        public string? AuthorizedName { get; set; }

        public string? AuthorizedPhone { get; set; }


        // Belgeler
        public DateTime? InsuranceExpiryDate { get; set; }

        public DateTime? InspectionExpiryDate { get; set; }


        // Durum
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


    // =========================================================
    // CREATE
    // =========================================================

    public class CreateVehicleRequest
    {
        [Required(ErrorMessage = "Plaka zorunludur.")]
        [StringLength(
            20,
            ErrorMessage = "Plaka en fazla 20 karakter olabilir.")]
        public string Plate { get; set; } = string.Empty;


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Araç tipi seçiniz.")]
        public int VehicleTypeId { get; set; }


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


        // Şoför
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Şoför seçiniz.")]
        public int DriverAccountCardId { get; set; }


        public bool DriverIsLicenseOwner { get; set; }


        // Referans
        public int? ReferenceAccountCardId { get; set; }


        // Ruhsat carisi
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Araç ruhsat carisi seçiniz.")]
        public int LicenseAccountCardId { get; set; }


        // Fatura
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Fatura hesabı seçiniz.")]
        public int InvoiceAccountCardId { get; set; }


        public bool ReferenceIsInvoiceAccount { get; set; }

        public bool LicenseOwnerIsInvoiceAccount { get; set; }


        // Ruhsat sahibi snapshot
        [StringLength(200)]
        public string LicenseOwnerName { get; set; } = string.Empty;


        [StringLength(10)]
        public string? LicenseOwnerTaxNumber { get; set; }


        [StringLength(11)]
        public string? LicenseOwnerIdentityNumber { get; set; }


        [StringLength(500)]
        public string? LicenseOwnerAddress { get; set; }


        public int LicenseOwnerCityId { get; set; }


        public int? LicenseOwnerTaxOfficeId { get; set; }


        // Yetkili
        [StringLength(100)]
        public string? AuthorizedName { get; set; }


        [StringLength(20)]
        public string? AuthorizedPhone { get; set; }


        // Belge
        public DateTime? InsuranceExpiryDate { get; set; }

        public DateTime? InspectionExpiryDate { get; set; }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    public class UpdateVehicleRequest
        : CreateVehicleRequest
    {
        [Required(
            ErrorMessage =
                "Kayıt sürüm bilgisi zorunludur.")]
        public byte[] RowVersion { get; set; } = [];
    }


    // =========================================================
    // STATUS
    // =========================================================

    public class ChangeVehicleStatusRequest
    {
        public bool IsActive { get; set; }


        [Required(
            ErrorMessage =
                "Kayıt sürüm bilgisi zorunludur.")]
        public byte[] RowVersion { get; set; } = [];
    }


    // =========================================================
    // DELETE
    // =========================================================

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
}