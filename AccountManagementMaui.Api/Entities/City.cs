using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class City : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CityCode { get; set; } = string.Empty;

        public ICollection<District> Districts { get; set; }
            = new List<District>();
    }
}
