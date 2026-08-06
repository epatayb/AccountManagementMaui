using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class AccountCardType : BaseEntity
    {
        public int Id { get; set; }

        public string TypeCode { get; set; } = string.Empty;

        public string TypeName { get; set; } = string.Empty;

        public ICollection<AccountCardKind> AccountCardKinds { get; set; } = new List<AccountCardKind>();
    }
}
