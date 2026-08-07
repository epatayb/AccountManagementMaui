using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountManagementMaui.Api.Entities
{
    public class AppUser :IdentityUser<int>
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        public bool IsDeleted { get; set; }

        public string? DeleteReason { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        public int? CreatedByUserId { get; set; }

        public AppUser? CreatedByUser { get; set; }

        public int? ModifiedByUserId { get; set; }

        public AppUser? ModifiedByUser { get; set; }

        public ICollection<AppRefreshToken> RefreshTokens { get; set; } = [];
    }
}
