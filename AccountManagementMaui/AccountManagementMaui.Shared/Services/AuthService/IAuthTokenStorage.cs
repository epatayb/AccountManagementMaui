using AccountManagementMaui.Shared.Models.AuthModels;

namespace AccountManagementMaui.Shared.Services.AuthServices;

public interface IAuthTokenStorage
{
    Task SaveAsync(
        AuthResponse authResponse);

    Task<AuthResponse?> GetAsync();

    Task ClearAsync();
}