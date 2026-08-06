using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.TaxOfficeModels
{
    public class CreateTaxOfficeRequest
    {
        [Display(Name = "Vergi Dairesi Kodu")]
        [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
        [MaxLength(20, ErrorMessage = "En fazla {1} karakter girilebilir.")]
        public string TaxOfficeCode { get; set; } = null!;

        [Display(Name = "Vergi Dairesi Adı")]
        [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
        [MaxLength(100, ErrorMessage = "En fazla {1} karakter girilebilir.")]
        public string Name { get; set; } = null!;

        [Display(Name = "İl")]
        [Range(1, int.MaxValue, ErrorMessage = "Bir il seçiniz.")]
        public int CityId { get; set; }
    }
}
