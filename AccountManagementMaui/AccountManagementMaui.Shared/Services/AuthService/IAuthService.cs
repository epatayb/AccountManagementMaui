using AccountManagementMaui.Shared.Models.AuthModels;

namespace AccountManagementMaui.Shared.Services.AuthServices;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);


    Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);


    Task<AuthResponse?> RefreshAsync(
        CancellationToken cancellationToken = default);


    Task LogoutAsync(
        CancellationToken cancellationToken = default);


    Task<AuthResponse?> GetSessionAsync();


    Task<bool> IsAuthenticatedAsync();
}