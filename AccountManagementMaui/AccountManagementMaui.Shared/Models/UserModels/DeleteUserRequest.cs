using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Models.UserModels
{
    public class DeleteUserRequest
    {
        [Display(Name = "Silme Nedeni")]
        [Required(ErrorMessage = "{0} boş geçilemez.")]
        [MaxLength(500, ErrorMessage = "En fazla {1} karakter girilebilir.")]
        public string DeleteReason { get; set; } = null!;
    }
}
