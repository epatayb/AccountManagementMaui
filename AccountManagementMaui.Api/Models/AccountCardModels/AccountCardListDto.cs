namespace AccountManagementMaui.Api.Models.AccountCardModels;

public class AccountCardListDto
{
    public int Id { get; set; }

    public string AccountCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;


    // Tip
    public int AccountCardTypeId { get; set; }

    public string AccountCardTypeCode { get; set; } = string.Empty;

    public string AccountCardTypeName { get; set; } = string.Empty;


    // Tür
    public int AccountCardKindId { get; set; }

    public string AccountCardKindCode { get; set; } = string.Empty;

    public string AccountCardKindName { get; set; } = string.Empty;


    // Grup
    public int? AccountCardGroupId { get; set; }

    public string? AccountCardGroupName { get; set; }


    // Alt Grup
    public int? AccountCardSubGroupId { get; set; }

    public string? AccountCardSubGroupName { get; set; }


    // Konum
    public int? CityId { get; set; }

    public string? CityCode { get; set; }

    public string? CityName { get; set; }

    public int? DistrictId { get; set; }

    public string? DistrictName { get; set; }


    // Vergi
    public int? TaxOfficeId { get; set; }

    public string? TaxOfficeCode { get; set; }

    public string? TaxOfficeName { get; set; }

    public string? TaxNumber { get; set; }

    public string? IdentityNumber { get; set; }


    // İletişim
    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? ContactPerson { get; set; }


    // Audit
    public DateTime CreatedDate { get; set; }

    public int? CreatedByUserId { get; set; }

    public string? CreatedByUserFullName { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public string? ModifiedByUserFullName { get; set; }
}