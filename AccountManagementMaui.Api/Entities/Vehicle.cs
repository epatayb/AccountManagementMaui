using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class Vehicle : BaseEntity
    {
        public int Id { get; set; }

        // ARAÇ BİLGİLERİ
        public string Plate { get; set; } = string.Empty;

        public string NormalizedPlate { get; set; } = string.Empty;


        public int VehicleTypeId { get; set; }

        public VehicleType VehicleType { get; set; } = null!;


        public int VehicleKindId { get; set; }

        public VehicleKind VehicleKind { get; set; } = null!;


        public string? TrailerPlate { get; set; }

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? Country { get; set; }


        // ŞOFÖR
        public int DriverAccountCardId { get; set; }

        public AccountCard DriverAccountCard { get; set; } = null!;

        public bool DriverIsLicenseOwner { get; set; }


        // REFERANS
        public int? ReferenceAccountCardId { get; set; }

        public AccountCard? ReferenceAccountCard { get; set; }


        // RUHSAT CARİSİ
        public int LicenseAccountCardId { get; set; }

        public AccountCard LicenseAccountCard { get; set; } = null!;


        // FATURA HESABI
        public int InvoiceAccountCardId { get; set; }

        public AccountCard InvoiceAccountCard { get; set; } = null!;


        public bool ReferenceIsInvoiceAccount { get; set; }

        public bool LicenseOwnerIsInvoiceAccount { get; set; }


        // RUHSAT SAHİBİ SNAPSHOT
        public string LicenseOwnerName { get; set; } = string.Empty;

        public string? LicenseOwnerTaxNumber { get; set; }

        public string? LicenseOwnerIdentityNumber { get; set; }

        public string? LicenseOwnerAddress { get; set; }


        public int LicenseOwnerCityId { get; set; }

        public City LicenseOwnerCity { get; set; } = null!;


        public int? LicenseOwnerTaxOfficeId { get; set; }

        public TaxOffice? LicenseOwnerTaxOffice { get; set; }


        // YETKİLİ
        public string? AuthorizedName { get; set; }

        public string? AuthorizedPhone { get; set; }


        // BELGELER
        public DateTime? InsuranceExpiryDate { get; set; }

        public DateTime? InspectionExpiryDate { get; set; }


        // DURUM
        public bool IsActive { get; set; } = true;


        // CONCURRENCY
        public byte[] RowVersion { get; set; } = [];
    }
}
