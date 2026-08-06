using Microsoft.AspNetCore.Identity;

namespace AccountManagementMaui.Api.Entities
{
    public class AppUser :IdentityUser<int>
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public string DeleteReason { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        public int? CreatedByUserId { get; set; }

        public AppUser? CreatedByUser { get; set; }

        public int? ModifiedByUserId { get; set; }

        public AppUser? ModifiedByUser { get; set; }
    }
}
