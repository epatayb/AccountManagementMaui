using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class VehicleKind : BaseEntity
    {
        public int Id { get; set; }

        public string KindName { get; set; } = string.Empty;

        public ICollection<Vehicle> Vehicles { get; set; } = [];
    }
}
