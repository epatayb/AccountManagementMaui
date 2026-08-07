namespace AccountManagementMaui.Shared.Models.AccountCardGroupModels;

public class AccountCardGroupDetailDto
{
    public int Id { get; set; }

    public string GroupName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public int? CreatedByUserId { get; set; }

    public string? CreatedByUserFullName { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public string? ModifiedByUserFullName { get; set; }
}