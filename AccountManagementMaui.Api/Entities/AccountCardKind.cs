using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities
{
    public class AccountCardKind : BaseEntity
    {
        public int Id { get; set; }

        public string? KindCode { get; set; } = string.Empty;

        public string KindName { get; set; } = string.Empty;
        
        public int AccountCardTypeId { get; set; }

        public AccountCardType AccountCardType { get; set; } = null!;
    }
}
