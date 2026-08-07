using System.ComponentModel.DataAnnotations.Schema;

namespace AccountManagementMaui.Api.Entities;

public class AppRefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public AppUser User { get; set; } = null!;


    // Refresh token'ın kendisini DB'de açık tutmayacağız.
    // SHA-256 hash değeri saklanacak.
    public string TokenHash { get; set; } = string.Empty;


    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }


    // Token yenilendiyse yeni token'ın hash'i.
    public string? ReplacedByTokenHash { get; set; }


    [NotMapped]
    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAtUtc;


    [NotMapped]
    public bool IsRevoked =>
        RevokedAtUtc.HasValue;


    [NotMapped]
    public bool IsActive =>
        !IsExpired && !IsRevoked;
}