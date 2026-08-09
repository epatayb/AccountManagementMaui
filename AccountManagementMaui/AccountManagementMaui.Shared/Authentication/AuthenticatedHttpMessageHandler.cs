using System.Net.Http.Headers;
using AccountManagementMaui.Shared.Services.AuthServices;

namespace AccountManagementMaui.Shared.Authentication;

public class AuthenticatedHttpMessageHandler
    : DelegatingHandler
{
    private readonly IAuthService _authService;


    public AuthenticatedHttpMessageHandler(
        IAuthService authService)
    {
        _authService = authService;
    }


    protected override async Task<HttpResponseMessage>
        SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
    {
        var session =
            await _authService
                .GetValidSessionAsync(
                    cancellationToken);

        if (session is not null &&
            !string.IsNullOrWhiteSpace(
                session.AccessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    session.AccessToken);
        }

        return await base.SendAsync(
            request,
            cancellationToken);
    }
}