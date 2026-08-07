using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.AccountCardSubGroupModels;

public class UpdateAccountCardSubGroupRequest
{
    [Display(Name = "Hesap Kart Grubu")]
    [Range(1, int.MaxValue, ErrorMessage = "Bir hesap kart grubu seçiniz.")]
    public int AccountCardGroupId { get; set; }

    [Display(Name = "Alt Grup Adı")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(100, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string SubGroupName { get; set; } = null!;
}