using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class VehicleType : BaseEntity
    {
        public int Id { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public ICollection<Vehicle> Vehicles { get; set; } = [];
    }
}
