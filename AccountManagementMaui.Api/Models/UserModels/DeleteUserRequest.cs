using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.UserModels
{
    public class DeleteUserRequest
    {
        [Display(Name = "Silme Nedeni")]
        [Required(ErrorMessage = "{0} boş geçilemez.")]
        [MaxLength(500, ErrorMessage = "En fazla {1} karakter girilebilir.")]
        public string DeleteReason { get; set; } = null!;
    }
}
