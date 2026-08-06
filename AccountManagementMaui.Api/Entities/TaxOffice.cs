using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class TaxOffice : BaseEntity
    {
        public int Id { get; set; }

        public string TaxOfficeCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int CityId { get; set; }

        public City City { get; set; } = null!;
    }
}
