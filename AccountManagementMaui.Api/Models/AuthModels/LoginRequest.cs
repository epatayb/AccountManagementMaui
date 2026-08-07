using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.AuthModels;

public class LoginRequest
{
    [Display(Name = "Kullanıcı Adı veya E-Posta")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(150, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string Identifier { get; set; } = string.Empty;


    [Display(Name = "Parola")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    public string Password { get; set; } = string.Empty;
}