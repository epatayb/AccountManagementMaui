using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Shared.Models.UserModels;

public class UserFormModel : IValidatableObject
{
    [Display(Name = "Ad")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(50, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Soyad")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(50, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Kullanıcı Adı")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(50, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "E-Posta")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$", ErrorMessage = "E-posta adresini kullanici@alanadi.com biçiminde giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Telefon Numarası")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [RegularExpression(
        @"^\(\d{3}\)\d{3}-\d{4}$",
        ErrorMessage = "Telefon numarasını (XXX)XXX-XXXX biçiminde eksiksiz giriniz.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Parola")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "{0} en az {1} karakter olmalıdır.")]
    public string? Password { get; set; }

    [Display(Name = "Parola Tekrar")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }

    public bool IsCreateMode { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        var passwordWasEntered =
            !string.IsNullOrWhiteSpace(Password) ||
            !string.IsNullOrWhiteSpace(ConfirmPassword);

        if (IsCreateMode || passwordWasEntered)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult(
                    "Parola boş geçilemez.",
                    [nameof(Password)]);
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                yield return new ValidationResult(
                    "Parola Tekrar boş geçilemez.",
                    [nameof(ConfirmPassword)]);
            }

            if (!string.IsNullOrWhiteSpace(Password) &&
                !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                Password != ConfirmPassword)
            {
                yield return new ValidationResult(
                    "Şifreler uyuşmuyor.",
                    [nameof(ConfirmPassword)]);
            }
        }
    }
}