using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Shared.Models.AccountCardGroupModels;

public class AccountCardGroupFormModel
{
    [Display(Name = "Hesap Kart Grup Adı")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(100, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string GroupName { get; set; } = string.Empty;
}