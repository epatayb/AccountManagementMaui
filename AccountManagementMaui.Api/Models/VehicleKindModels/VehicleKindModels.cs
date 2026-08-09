using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.VehicleKindModels
{
    public class VehicleKindListDto
    {
        public int Id { get; set; }

        public string KindName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int? CreatedByUserId { get; set; }

        public string? CreatedByUserFullName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public string? ModifiedByUserFullName { get; set; }
    }


    public class VehicleKindDetailDto
    {
        public int Id { get; set; }

        public string KindName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int? CreatedByUserId { get; set; }

        public string? CreatedByUserFullName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public string? ModifiedByUserFullName { get; set; }
    }


    public class CreateVehicleKindRequest
    {
        [Required(ErrorMessage = "Araç türü adı zorunludur.")]
        [StringLength(
            50,
            ErrorMessage = "Araç türü adı en fazla 50 karakter olabilir.")]
        public string KindName { get; set; } = string.Empty;
    }


    public class UpdateVehicleKindRequest
    {
        [Required(ErrorMessage = "Araç türü adı zorunludur.")]
        [StringLength(
            50,
            ErrorMessage = "Araç türü adı en fazla 50 karakter olabilir.")]
        public string KindName { get; set; } = string.Empty;
    }


    public class DeleteVehicleKindRequest
    {
        [Required(ErrorMessage = "Silme açıklaması zorunludur.")]
        [StringLength(
            500,
            ErrorMessage = "Silme açıklaması en fazla 500 karakter olabilir.")]
        public string DeleteReason { get; set; } = string.Empty;
    }
}
