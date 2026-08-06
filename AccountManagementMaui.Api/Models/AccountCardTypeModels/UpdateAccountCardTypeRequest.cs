using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.AccountCardTypeModels;

public class UpdateAccountCardTypeRequest
{
    [Display(Name = "Kart Tip Kodu")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(7, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string TypeCode { get; set; } = null!;

    [Display(Name = "Kart Tip Adı")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(50, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string TypeName { get; set; } = null!;
}