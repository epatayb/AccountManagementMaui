using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.AccountCardTypeModels
{
    public class DeleteAccountCardTypeRequest
    {
        [Display(Name = "Silme Açıklaması")]
        [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
        [MaxLength(500, ErrorMessage = "En fazla {1} karakter girilebilir.")]
        public string DeleteReason { get; set; } = null!;
    }
}
