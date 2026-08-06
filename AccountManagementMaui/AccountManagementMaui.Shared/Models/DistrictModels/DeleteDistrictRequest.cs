using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Models.DistrictModels
{
    public class DeleteDistrictRequest
    {
        [Display(Name = "Silme Nedeni")]
        [Required(ErrorMessage = "Silme nedeni boş geçilemez.")]
        [MaxLength(500, ErrorMessage = "En fazla {1} karakter girilebilir.")]
        public string DeleteReason { get; set; } = null!;
    }
}
