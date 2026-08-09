namespace AccountManagementMaui.Shared.Models.AccountCardKindModels;

public class AccountCardKindListDto
{
    public int Id { get; set; }

    public string KindName { get; set; } = string.Empty;

    public int AccountCardTypeId { get; set; }

    public string AccountCardTypeName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public int? CreatedByUserId { get; set; }

    public string? CreatedByUserFullName { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public string? ModifiedByUserFullName { get; set; }
}