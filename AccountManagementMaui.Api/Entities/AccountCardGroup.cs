using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class AccountCardGroup : BaseEntity
    {
        public int Id { get; set; }

        public string GroupName { get; set; } = string.Empty;
    }
}
