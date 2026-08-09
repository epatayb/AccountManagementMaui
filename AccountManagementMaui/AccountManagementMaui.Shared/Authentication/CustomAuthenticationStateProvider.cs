using System.Security.Claims;
using AccountManagementMaui.Shared.Models.AuthModels;
using AccountManagementMaui.Shared.Services.AuthServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AccountManagementMaui.Shared.Authentication;

public class CustomAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly IAuthService _authService;

    private static readonly AuthenticationState AnonymousState =
        new(
            new ClaimsPrincipal(
                new ClaimsIdentity()));

    private AuthenticationState _currentState =
        AnonymousState;

    private readonly TaskCompletionSource<AuthenticationState>
        _initialStateSource =
            new(
                TaskCreationOptions
                    .RunContinuationsAsynchronously);

    private readonly SemaphoreSlim _initializeLock =
        new(1, 1);

    private bool _isInitialized;


    public CustomAuthenticationStateProvider(
        IAuthService authService)
    {
        _authService = authService;
    }


    // =========================================================
    // CURRENT AUTH STATE
    // =========================================================

    public override Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        if (_isInitialized)
        {
            return Task.FromResult(
                _currentState);
        }

        return _initialStateSource.Task;
    }


    // =========================================================
    // INITIALIZE
    // =========================================================

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await _initializeLock.WaitAsync();

        try
        {
            if (_isInitialized)
            {
                return;
            }

            AuthResponse? session;

            try
            {
                session =
                    await _authService
                        .GetValidSessionAsync();
            }
            catch
            {
                session = null;
            }

            if (session is null)
            {
                SetAnonymous();

                return;
            }

            SetAuthenticated(
                session.User);
        }
        finally
        {
            _initializeLock.Release();
        }
    }


    // =========================================================
    // LOGIN NOTIFICATION
    // =========================================================

    public void NotifyUserAuthenticated(
        AuthResponse authResponse)
    {
        SetAuthenticated(
            authResponse.User);
    }


    // =========================================================
    // LOGOUT NOTIFICATION
    // =========================================================

    public void NotifyUserLoggedOut()
    {
        SetAnonymous();
    }


    // =========================================================
    // REFRESH AUTH STATE
    // =========================================================

    public async Task RefreshAuthenticationStateAsync()
    {
        try
        {
            var session =
                await _authService
                    .GetValidSessionAsync();

            if (session is null)
            {
                SetAnonymous();

                return;
            }

            SetAuthenticated(
                session.User);
        }
        catch
        {
            SetAnonymous();
        }
    }


    // =========================================================
    // INTERNAL STATE
    // =========================================================

    private void SetAuthenticated(
        AuthUserDto user)
    {
        var state =
            CreateAuthenticationState(
                user);

        SetState(state);
    }


    private void SetAnonymous()
    {
        SetState(
            AnonymousState);
    }


    private void SetState(
        AuthenticationState state)
    {
        _currentState = state;

        if (!_isInitialized)
        {
            _isInitialized = true;

            _initialStateSource
                .TrySetResult(
                    state);
        }

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                state));
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