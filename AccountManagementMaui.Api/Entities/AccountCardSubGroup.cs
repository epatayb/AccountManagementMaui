using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class AccountCardSubGroup : BaseEntity
    {
        public int Id { get; set; }

        public string SubGroupName { get; set; } = string.Empty;

        public int AccountCardGroupId { get; set; }

        public AccountCardGroup AccountCardGroup { get; set; } = null!;
    }
}
