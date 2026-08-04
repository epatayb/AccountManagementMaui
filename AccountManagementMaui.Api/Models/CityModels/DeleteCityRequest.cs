using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.CityModels
{
    public class DeleteCityRequest
    {
        [Display(Name = "Silme Nedeni")]
        [Required(ErrorMessage = "Silme nedeni boş geçilemez.")]
        [MaxLength(500, ErrorMessage = "En fazla {1} karakter girilebilir.")]
        public string DeleteReason { get; set; } = null!;
    }
}
