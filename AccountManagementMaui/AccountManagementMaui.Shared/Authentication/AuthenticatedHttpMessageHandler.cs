using System.Net.Http.Headers;
using AccountManagementMaui.Shared.Models.AuthModels;
using AccountManagementMaui.Shared.Services.AuthServices;

namespace AccountManagementMaui.Shared.Authentication;

public class AuthenticatedHttpMessageHandler
    : DelegatingHandler
{
    private readonly IAuthTokenStorage _tokenStorage;
    private readonly IAuthService _authService;


    public AuthenticatedHttpMessageHandler(
        IAuthTokenStorage tokenStorage,
        IAuthService authService)
    {
        _tokenStorage = tokenStorage;
        _authService = authService;
    }


    protected override async Task<HttpResponseMessage>
        SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
    {
        var session =
            await _tokenStorage.GetAsync();


        if (session is not null)
        {
            if (session.AccessTokenExpiresAtUtc <=
                DateTime.UtcNow)
            {
                session =
                    await TryRefreshAsync(
                        cancellationToken);
            }


            if (session is not null &&
                !string.IsNullOrWhiteSpace(
                    session.AccessToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        session.AccessToken);
            }
        }


        return await base.SendAsync(
            request,
            cancellationToken);
    }


    private async Task<AuthResponse?>
        TryRefreshAsync(
            CancellationToken cancellationToken)
    {
        try
        {
            return await _authService.RefreshAsync(
                cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}