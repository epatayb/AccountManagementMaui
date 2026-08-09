using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.VehicleTypeModels
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
        [StringLength(
            50,
            ErrorMessage = "Araç tipi adı en fazla 50 karakter olabilir.")]
        public string TypeName { get; set; } = string.Empty;
    }


    public class UpdateVehicleTypeRequest
    {
        [Required(ErrorMessage = "Araç tipi adı zorunludur.")]
        [StringLength(
            50,
            ErrorMessage = "Araç tipi adı en fazla 50 karakter olabilir.")]
        public string TypeName { get; set; } = string.Empty;
    }


    public class DeleteVehicleTypeRequest
    {
        [Required(ErrorMessage = "Silme açıklaması zorunludur.")]
        [StringLength(
            500,
            ErrorMessage = "Silme açıklaması en fazla 500 karakter olabilir.")]
        public string DeleteReason { get; set; } = string.Empty;
    }
}
