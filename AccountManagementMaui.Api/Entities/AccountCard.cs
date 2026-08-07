using AccountManagementMaui.Api.Entities.Common;

namespace AccountManagementMaui.Api.Entities;

public class AccountCard : BaseEntity
{
    public int Id { get; set; }

    public string AccountCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;


    public int AccountCardTypeId { get; set; }

    public AccountCardType AccountCardType { get; set; } = null!;


    public int AccountCardKindId { get; set; }

    public AccountCardKind AccountCardKind { get; set; } = null!;


    public int? AccountCardGroupId { get; set; }

    public AccountCardGroup? AccountCardGroup { get; set; }


    public int? AccountCardSubGroupId { get; set; }

    public AccountCardSubGroup? AccountCardSubGroup { get; set; }


    public int? CityId { get; set; }

    public City? City { get; set; }

    public int? DistrictId { get; set; }

    public District? District { get; set; }


    public int? TaxOfficeId { get; set; }

    public TaxOffice? TaxOffice { get; set; }

    public string? TaxNumber { get; set; }

    public string? IdentityNumber { get; set; }


    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? ContactPerson { get; set; }


    public string? Address { get; set; }
}