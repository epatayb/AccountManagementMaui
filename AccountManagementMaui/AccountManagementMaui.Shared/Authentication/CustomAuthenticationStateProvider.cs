using System.Security.Claims;
using AccountManagementMaui.Shared.Models.AuthModels;
using AccountManagementMaui.Shared.Services.AuthServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AccountManagementMaui.Shared.Authentication;

public class CustomAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly IAuthService _authService;


    private static readonly AuthenticationState
        AnonymousState =
            new(
                new ClaimsPrincipal(
                    new ClaimsIdentity()));


    public CustomAuthenticationStateProvider(
        IAuthService authService)
    {
        _authService =
            authService;
    }


    // =========================================================
    // CURRENT AUTH STATE
    // =========================================================

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        try
        {
            var session =
                await _authService.GetSessionAsync();


            if (session is null)
            {
                return AnonymousState;
            }


            // Refresh token da bittiyse oturum tamamen sona ermiştir.
            if (session.RefreshTokenExpiresAtUtc <=
                DateTime.UtcNow)
            {
                await _authService.LogoutAsync();

                return AnonymousState;
            }


            // Access token süresi bittiyse yenile.
            if (session.AccessTokenExpiresAtUtc <=
                DateTime.UtcNow)
            {
                session =
                    await TryRefreshAsync();


                if (session is null)
                {
                    return AnonymousState;
                }
            }


            return CreateAuthenticationState(
                session.User);
        }
        catch
        {
            return AnonymousState;
        }
    }


    // =========================================================
    // LOGIN NOTIFICATION
    // =========================================================

    public void NotifyUserAuthenticated(
        AuthResponse authResponse)
    {
        var authenticationState =
            CreateAuthenticationState(
                authResponse.User);


        NotifyAuthenticationStateChanged(
            Task.FromResult(
                authenticationState));
    }


    // =========================================================
    // LOGOUT NOTIFICATION
    // =========================================================

    public void NotifyUserLoggedOut()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(
                AnonymousState));
    }


    // =========================================================
    // REFRESH AUTH STATE
    // =========================================================

    public async Task RefreshAuthenticationStateAsync()
    {
        var state =
            await GetAuthenticationStateAsync();


        NotifyAuthenticationStateChanged(
            Task.FromResult(state));
    }


    // =========================================================
    // REFRESH TOKEN
    // =========================================================

    private async Task<AuthResponse?>
        TryRefreshAsync()
    {
        try
        {
            return await _authService
                .RefreshAsync();
        }
        catch
        {
            return null;
        }
    }


    // =========================================================
    // CLAIMS
    // =========================================================

    private static AuthenticationState
        CreateAuthenticationState(
            AuthUserDto user)
    {
        var claims =
            new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new(
                    ClaimTypes.Name,
                    user.UserName),

                new(
                    ClaimTypes.Email,
                    user.Email),

                new(
                    ClaimTypes.GivenName,
                    user.FirstName),

                new(
                    ClaimTypes.Surname,
                    user.LastName),

                new(
                    "FullName",
                    user.FullName)
            };


        if (!string.IsNullOrWhiteSpace(
            user.PhoneNumber))
        {
            claims.Add(
                new Claim(
                    ClaimTypes.MobilePhone,
                    user.PhoneNumber));
        }


        foreach (var role in user.Roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }


        var identity =
            new ClaimsIdentity(
                claims,
                authenticationType:
                    "Bearer");


        var principal =
            new ClaimsPrincipal(
                identity);


        return new AuthenticationState(
            principal);
    }
}