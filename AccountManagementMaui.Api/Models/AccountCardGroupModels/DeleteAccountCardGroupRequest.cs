using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.AccountCardGroupModels
{
    public class DeleteAccountCardGroupRequest
    {
        [Display(Name = "Silme Açıklaması")]
        [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
        [MinLength(3, ErrorMessage = "{0} en az {1} karakter olmalıdır.")]
        [MaxLength(500, ErrorMessage = "En fazla {1} karakter girilebilir.")]
        public string DeleteReason { get; set; } = null!;
    }
}
