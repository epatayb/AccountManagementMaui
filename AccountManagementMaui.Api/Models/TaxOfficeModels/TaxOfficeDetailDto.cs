namespace AccountManagementMaui.Api.Models.TaxOfficeModels
{
    public class TaxOfficeDetailDto
    {
        public int Id { get; set; }

        public string TaxOfficeCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int CityId { get; set; }

        public string CityCode { get; set; } = string.Empty;

        public string CityName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int? CreatedByUserId { get; set; }

        public string? CreatedByUserFullName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public string? ModifiedByUserFullName { get; set; }
    }
}
