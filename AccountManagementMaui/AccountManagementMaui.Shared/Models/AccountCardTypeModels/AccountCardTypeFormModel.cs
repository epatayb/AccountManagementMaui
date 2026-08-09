using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Shared.Models.AccountCardTypeModels;

public class AccountCardTypeFormModel
{
    [Display(Name = "Ana Hesap Tip Adı")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(50, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string TypeName { get; set; } = string.Empty;
}