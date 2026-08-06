namespace AccountManagementMaui.Shared.Models.AccountCardTypeModels;

public class AccountCardTypeListDto
{
    public int Id { get; set; }

    public string TypeCode { get; set; } = string.Empty;

    public string TypeName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public int? CreatedByUserId { get; set; }

    public string? CreatedByUserFullName { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public string? ModifiedByUserFullName { get; set; }
}