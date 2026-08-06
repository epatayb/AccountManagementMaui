using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Shared.Models.AccountCardKindModels;

public class AccountCardKindFormModel
{
    [Display(Name = "Kart Tür Kodu")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(7, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string KindCode { get; set; } = string.Empty;

    [Display(Name = "Kart Tür Adı")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(50, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    public string KindName { get; set; } = string.Empty;

    [Display(Name = "Kart Tipi")]
    [Range(1, int.MaxValue, ErrorMessage = "Bir kart tipi seçiniz.")]
    public int AccountCardTypeId { get; set; }
}