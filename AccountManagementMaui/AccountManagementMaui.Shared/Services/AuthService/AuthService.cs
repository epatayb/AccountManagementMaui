using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.AuthModels;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.AuthServices;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    private readonly IAuthTokenStorage _tokenStorage;

    private readonly SemaphoreSlim _refreshLock =
        new(1, 1);

    public AuthService(
        HttpClient httpClient,
        IAuthTokenStorage tokenStorage)
    {
        _httpClient = httpClient;

        _tokenStorage = tokenStorage;
    }


    // =========================================================
    // LOGIN
    // =========================================================

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsJsonAsync(
                "api/auth/login",
                request,
                cancellationToken);


        await EnsureSuccessAsync(
            response,
            cancellationToken);


        var authResponse =
            await response.Content
                .ReadFromJsonAsync<AuthResponse>(
                    cancellationToken:
                        cancellationToken);


        if (authResponse is null)
        {
            throw new ApiException(
                "Giriş bilgileri API tarafından alınamadı.",
                (int)response.StatusCode);
        }


        await _tokenStorage.SaveAsync(
            authResponse);


        return authResponse;
    }


    // =========================================================
    // REGISTER
    // =========================================================

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsJsonAsync(
                "api/auth/register",
                request,
                cancellationToken);


        await EnsureSuccessAsync(
            response,
            cancellationToken);


        var result =
            await response.Content
                .ReadFromJsonAsync<RegisterResponse>(
                    cancellationToken:
                        cancellationToken);


        return result ??
            new RegisterResponse
            {
                Message =
                    "Hesabınız başarıyla oluşturuldu."
            };
    }


    // =========================================================
    // REFRESH
    // =========================================================

    public async Task<AuthResponse?> RefreshAsync(
        CancellationToken cancellationToken = default)
    {
        await _refreshLock.WaitAsync(cancellationToken);

        try
        {
            var session =
                await _tokenStorage.GetAsync();

            if (session is null ||
                string.IsNullOrWhiteSpace(
                    session.RefreshToken))
            {
                return null;
            }

            // Başka bir işlem biz beklerken token'ı
            // yenilemiş olabilir.
            if (session.AccessTokenExpiresAtUtc >
                DateTime.UtcNow.AddSeconds(10))
            {
                return session;
            }

            if (session.RefreshTokenExpiresAtUtc <=
                DateTime.UtcNow)
            {
                await _tokenStorage.ClearAsync();

                return null;
            }

            var request =
                new RefreshTokenRequest
                {
                    RefreshToken =
                        session.RefreshToken
                };

            using var response =
                await _httpClient.PostAsJsonAsync(
                    "api/auth/refresh",
                    request,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _tokenStorage.ClearAsync();

                return null;
            }

            var authResponse =
                await response.Content
                    .ReadFromJsonAsync<AuthResponse>(
                        cancellationToken:
                            cancellationToken);

            if (authResponse is null)
            {
                await _tokenStorage.ClearAsync();

                return null;
            }

            await _tokenStorage.SaveAsync(
                authResponse);

            return authResponse;
        }
        finally
        {
            _refreshLock.Release();
        }
    }


    // =========================================================
    // LOGOUT
    // =========================================================

    public async Task LogoutAsync(
        CancellationToken cancellationToken = default)
    {
        var session =
            await _tokenStorage.GetAsync();


        try
        {
            if (session is not null &&
                !string.IsNullOrWhiteSpace(
                     session.RefreshToken) &&
                session.RefreshTokenExpiresAtUtc >
                    DateTime.UtcNow)
            {
                var request =
                    new LogoutRequest
                    {
                        RefreshToken =
                            session.RefreshToken
                    };


                using var response =
                    await _httpClient.PostAsJsonAsync(
                        "api/auth/logout",
                        request,
                        cancellationToken);


                // Logout sırasında server hatası olsa bile
                // local session aşağıdaki finally'de silinecek.
            }
        }
        finally
        {
            await _tokenStorage.ClearAsync();
        }
    }


    // =========================================================
    // SESSION
    // =========================================================

    public Task<AuthResponse?> GetSessionAsync()
    {
        return _tokenStorage.GetAsync();
    }

    public async Task<AuthResponse?> GetValidSessionAsync(
        CancellationToken cancellationToken = default)
    {
        var session =
            await _tokenStorage.GetAsync();

        if (session is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(
            session.AccessToken))
        {
            await _tokenStorage.ClearAsync();

            return null;
        }

        if (session.RefreshTokenExpiresAtUtc <=
            DateTime.UtcNow)
        {
            await _tokenStorage.ClearAsync();

            return null;
        }

        if (session.AccessTokenExpiresAtUtc >
            DateTime.UtcNow.AddSeconds(10))
        {
            return session;
        }

        return await RefreshAsync(
            cancellationToken);
    }

    public async Task<bool> IsAuthenticatedAsync(
       CancellationToken cancellationToken = default)
    {
        try
        {
            var session =
                await GetValidSessionAsync(
                    cancellationToken);

            return session is not null;
        }
        catch
        {
            return false;
        }
    }


    // =========================================================
    // ERROR
    // =========================================================

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }


        var error =
            await TryReadErrorAsync(
                response,
                cancellationToken);


        throw new ApiException(
            error?.GetErrorMessage() ??
            $"API isteği başarısız oldu. Durum kodu: {(int)response.StatusCode}",
            (int)response.StatusCode);
    }


    private static async Task<ApiErrorResponse?>
        TryReadErrorAsync(
            HttpResponseMessage response,
            CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content
                .ReadFromJsonAsync<ApiErrorResponse>(
                    cancellationToken:
                        cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}