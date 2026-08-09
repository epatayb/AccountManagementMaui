namespace AccountManagementMaui.Api.Models.VehicleLookupModels
{
    // =========================================================
    // ACCOUNT SEARCH RESULT
    // =========================================================

    public class VehicleAccountLookupDto
    {
        public int Id { get; set; }

        public string AccountCode { get; set; } =
            string.Empty;

        public string Title { get; set; } =
            string.Empty;

        public string AccountCardTypeName { get; set; } =
            string.Empty;

        public string AccountCardKindName { get; set; } =
            string.Empty;

        public string? PhoneNumber { get; set; }

        public string? CityName { get; set; }
    }


    // =========================================================
    // SELECTED ACCOUNT DETAIL
    // =========================================================

    public class VehicleAccountLookupDetailDto
    {
        public int Id { get; set; }

        public string AccountCode { get; set; } =
            string.Empty;

        public string Title { get; set; } =
            string.Empty;


        // Classification
        public int AccountCardTypeId { get; set; }

        public string AccountCardTypeName { get; set; } =
            string.Empty;

        public int AccountCardKindId { get; set; }

        public string AccountCardKindName { get; set; } =
            string.Empty;


        // Identity / tax
        public string? TaxNumber { get; set; }

        public string? IdentityNumber { get; set; }


        // Contact
        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? ContactPerson { get; set; }


        // Address
        public string? Address { get; set; }


        // City
        public int? CityId { get; set; }

        public string? CityName { get; set; }


        // District
        public int? DistrictId { get; set; }

        public string? DistrictName { get; set; }


        // Tax office
        public int? TaxOfficeId { get; set; }

        public string? TaxOfficeName { get; set; }
    }

    public class VehicleCityLookupDto
    {
        public int Id { get; set; }

        public string CityCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }


    public class VehicleTaxOfficeLookupDto
    {
        public int Id { get; set; }

        public string TaxOfficeCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int CityId { get; set; }
    }
}