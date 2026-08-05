using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Models.CityModels
{
    public class CreateCityRequest
    {
        [Display(Name = "Şehir Adı")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "{0} 2 ile 20 karakter arasında olmalıdır.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Şehir Kodu")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "{0} iki rakamdan oluşmalıdır.")]
        public string CityCode { get; set; } = string.Empty;
    }
}
