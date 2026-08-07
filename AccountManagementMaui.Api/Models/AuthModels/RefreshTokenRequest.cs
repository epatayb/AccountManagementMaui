using System.ComponentModel.DataAnnotations;

namespace AccountManagementMaui.Api.Models.AuthModels
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token bilgisi zorunludur.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
