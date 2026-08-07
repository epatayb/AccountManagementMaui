namespace AccountManagementMaui.Api.Models.AccountCardSubGroupModels;

public class AccountCardSubGroupDetailDto
{
    public int Id { get; set; }

    public string SubGroupName { get; set; } = string.Empty;

    public int AccountCardGroupId { get; set; }

    public string AccountCardGroupName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public int? CreatedByUserId { get; set; }

    public string? CreatedByUserFullName { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public string? ModifiedByUserFullName { get; set; }
}