using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Shared.Models.AuthModels;

public class RegisterRequest
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
    [EmailAddress(
        ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
        ErrorMessage =
            "E-posta adresini kullanici@alanadi.com biçiminde giriniz.")]
    [MaxLength(150, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string Email { get; set; } = string.Empty;


    [Display(Name = "Telefon Numarası")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [RegularExpression(
        @"^\(\d{3}\)\d{3}-\d{4}$",
        ErrorMessage =
            "Telefon numarasını (XXX)XXX-XXXX biçiminde eksiksiz giriniz.")]
    public string PhoneNumber { get; set; } = string.Empty;


    [Display(Name = "Parola")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [DataType(DataType.Password)]
    [MinLength(
        6,
        ErrorMessage = "{0} en az {1} karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;


    [Display(Name = "Parola Tekrar")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Password),
        ErrorMessage = "Parolalar uyuşmuyor.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}