using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.CityModels
{
    public class CreateCityRequest
    {
        [Display(Name = "Şehir Adı")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "{0} 2 ile 20 karakter arasında olmalıdır.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Şehir Kodu")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "{0} iki rakamdan oluşmalıdır. Örnek: 06, 23, 34.")]
        public string CityCode { get; set; } = string.Empty;
    }
}