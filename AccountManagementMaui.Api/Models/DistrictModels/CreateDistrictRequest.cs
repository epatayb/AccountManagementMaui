using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.DistrictModels;

public class CreateDistrictRequest
{
    [Display(Name = "İlçe Kodu")]
    [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
    [MaxLength(6, ErrorMessage = "En fazla {1} karakter girilebilir.")]
    [RegularExpression(@"^\d{1,6}$", ErrorMessage = "İlçe kodu yalnızca rakamlardan oluşmalıdır.")]
    public string DistrictCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "İlçe adı zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "İlçe adı 2 ile 50 karakter arasında olmalıdır.")]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir il seçilmelidir.")]
    public int CityId { get; set; }
}