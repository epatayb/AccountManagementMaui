using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.UserModels;

public class CreateUserRequest
{
    [Display(Name = "Ad")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(50, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Soyad")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(50, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Kullanıcı Adı")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(50, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string UserName { get; set; } = null!;

    [Display(Name = "E-Posta")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$", ErrorMessage = "E-posta adresini kullanici@alanadi.com biçiminde giriniz.")]
    public string Email { get; set; } = null!;

    [Display(Name = "Telefon Numarası")]
    [Required(ErrorMessage = "{0} boş geçilemez.")]
    [RegularExpression(@"^\(\d{3}\)\d{3}-\d{4}$", ErrorMessage = "Telefon numarasını (XXX)XXX-XXXX biçiminde eksiksiz giriniz.")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "Parola")]
    [Required(ErrorMessage = "{0} boş geçilemez.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "{0} en az {1} karakter olmalıdır.")]
    public string Password { get; set; } = null!;

    [Display(Name = "Parola Tekrar")]
    [Required(ErrorMessage = "{0} boş geçilemez.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifreler uyuşmuyor.")]
    public string ConfirmPassword { get; set; } = null!;
}