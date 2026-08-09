using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Models.VehicleTypeModels
{
    public class VehicleTypeListDto
    {
        public int Id { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int? CreatedByUserId { get; set; }

        public string? CreatedByUserFullName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public string? ModifiedByUserFullName { get; set; }
    }


    public class VehicleTypeDetailDto
    {
        public int Id { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int? CreatedByUserId { get; set; }

        public string? CreatedByUserFullName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public string? ModifiedByUserFullName { get; set; }
    }


    public class CreateVehicleTypeRequest
    {
        [Required(ErrorMessage = "Araç tipi adı zorunludur.")]
        [StringLength(50)]
        public string TypeName { get; set; } = string.Empty;
    }


    public class UpdateVehicleTypeRequest
    {
        [Required(ErrorMessage = "Araç tipi adı zorunludur.")]
        [StringLength(50)]
        public string TypeName { get; set; } = string.Empty;
    }


    public class DeleteVehicleTypeRequest
    {
        [Required(ErrorMessage = "Silme açıklaması zorunludur.")]
        [StringLength(500)]
        public string DeleteReason { get; set; } = string.Empty;
    }
}
