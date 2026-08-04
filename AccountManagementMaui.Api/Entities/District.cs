using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class District : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string DistrictCode { get; set; } = string.Empty;

        public int CityId { get; set; }

        public City City { get; set; } = null!;
    }
}
