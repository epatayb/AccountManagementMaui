using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Shared.Models.AccountCardModels;

public class UpdateAccountCardRequest : IValidatableObject
{
    [Display(Name = "Hesap Ünvanı")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(200, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string Title { get; set; } = null!;


    [Display(Name = "Hesap Kart Tipi")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Bir hesap kart tipi seçiniz.")]
    public int AccountCardTypeId { get; set; }


    [Display(Name = "Hesap Kart Türü")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Bir hesap kart türü seçiniz.")]
    public int AccountCardKindId { get; set; }


    public int? AccountCardGroupId { get; set; }

    public int? AccountCardSubGroupId { get; set; }

    public int? CityId { get; set; }

    public int? DistrictId { get; set; }

    public int? TaxOfficeId { get; set; }


    [Display(Name = "Vergi Numarası")]
    [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "Vergi numarası 10 rakamdan oluşmalıdır.")]
    public string? TaxNumber { get; set; }


    [Display(Name = "T.C. Kimlik Numarası")]
    [RegularExpression(
        @"^\d{11}$",
        ErrorMessage = "T.C. kimlik numarası 11 rakamdan oluşmalıdır.")]
    public string? IdentityNumber { get; set; }


    [Display(Name = "Telefon Numarası")]
    [RegularExpression(
        @"^\(\d{3}\)\d{3}-\d{4}$",
        ErrorMessage =
            "Telefon numarasını (XXX)XXX-XXXX biçiminde eksiksiz giriniz.")]
    public string? PhoneNumber { get; set; }


    [Display(Name = "E-Posta")]
    [EmailAddress(
        ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
        ErrorMessage =
            "E-posta adresini kullanici@alanadi.com biçiminde giriniz.")]
    [MaxLength(150, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string? Email { get; set; }


    [Display(Name = "Yetkili Kişi")]
    [MaxLength(100, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string? ContactPerson { get; set; }


    [Display(Name = "Açık Adres")]
    [MaxLength(500, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string? Address { get; set; }

    public IEnumerable<ValidationResult> Validate(
    ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(TaxNumber) &&
            string.IsNullOrWhiteSpace(IdentityNumber))
        {
            yield return new ValidationResult(
                "Vergi numarası veya T.C. kimlik numarasından en az biri girilmelidir.",
                new[]
                {
                nameof(TaxNumber)
                });
        }
    }
}