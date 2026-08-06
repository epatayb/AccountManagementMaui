using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Entities.Common
{
    public abstract class BaseEntity
    {
        public bool IsDeleted { get; set; } = false;

        [MaxLength(500, ErrorMessage = "En fazla {0} karakter girilebilir.")]
        public string? DeleteReason { get; set; }

        public int? CreatedByUserId { get; set; }

        public AppUser? CreatedByUser { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int? ModifiedByUserId { get; set; }

        public AppUser? ModifiedByUser { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
