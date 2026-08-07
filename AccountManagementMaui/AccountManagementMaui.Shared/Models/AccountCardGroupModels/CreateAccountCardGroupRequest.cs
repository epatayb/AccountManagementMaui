using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Shared.Models.AccountCardGroupModels
{
    public class CreateAccountCardGroupRequest
    {
        [Display(Name = "Hesap Kart Grup Adı")]
        [Required(ErrorMessage = "{0} bilgisi boş geçilemez.")]
        [MaxLength(20, ErrorMessage = "En fazla {1} karakter uzunluğunda olmalıdır.")]
        public string GroupName { get; set; } = null!;
    }
}
