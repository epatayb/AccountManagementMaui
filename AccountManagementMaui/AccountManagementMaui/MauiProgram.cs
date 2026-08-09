using AccountManagementMaui.Authentication;
using AccountManagementMaui.Services;

using AccountManagementMaui.Shared.Authentication;
using AccountManagementMaui.Shared.Services;
using AccountManagementMaui.Shared.Services.AccountCardGroupServices;
using AccountManagementMaui.Shared.Services.AccountCardKindServices;
using AccountManagementMaui.Shared.Services.AccountCardServices;
using AccountManagementMaui.Shared.Services.AccountCardSubGroupServices;
using AccountManagementMaui.Shared.Services.AccountCardTypeServices;
using AccountManagementMaui.Shared.Services.AuthServices;
using AccountManagementMaui.Shared.Services.CityServices;
using AccountManagementMaui.Shared.Services.DistrictServices;
using AccountManagementMaui.Shared.Services.TaxOfficeServices;
using AccountManagementMaui.Shared.Services.UserServices;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace AccountManagementMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // =========================================================
        // MAUI
        // =========================================================

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont(
                    "OpenSans-Regular.ttf",
                    "OpenSansRegular");
            });


        // =========================================================
        // BLAZOR WEB VIEW
        // =========================================================

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif


        // =========================================================
        // API BASE URL
        // =========================================================

#if ANDROID
        const string apiBaseUrl =
            "http://10.0.2.2:5107/";
#else
        const string apiBaseUrl =
            "https://localhost:7192/";
#endif


        // =========================================================
        // FORM FACTOR
        // =========================================================

        builder.Services.AddSingleton<
            IFormFactor,
            FormFactor>();


        // =========================================================
        // AUTHORIZATION
        // =========================================================

        builder.Services.AddAuthorizationCore();

        builder.Services.AddCascadingAuthenticationState();


        // =========================================================
        // TOKEN STORAGE
        // =========================================================

        builder.Services.AddSingleton<
            IAuthTokenStorage,
            MauiAuthTokenStorage>();


        // =========================================================
        // AUTHENTICATION STATE PROVIDER
        // =========================================================

        builder.Services.AddScoped<
            CustomAuthenticationStateProvider>();

        builder.Services.AddScoped<
            AuthenticationStateProvider>(
            serviceProvider =>
                serviceProvider.GetRequiredService<
                    CustomAuthenticationStateProvider>());


        // =========================================================
        // AUTHENTICATED HTTP HANDLER
        // =========================================================

        builder.Services.AddTransient<
            AuthenticatedHttpMessageHandler>();


        // =========================================================
        // AUTH SERVICE
        // =========================================================

        // Login ve Refresh istekleri token gerektirmediği için
        // AuthenticatedHttpMessageHandler eklemiyoruz.

        builder.Services.AddHttpClient<
            IAuthService,
            AuthService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            });


        // =========================================================
        // CITY SERVICE
        // =========================================================

        builder.Services.AddHttpClient<
            ICityService,
            CityService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<
                AuthenticatedHttpMessageHandler>();


        // =========================================================
        // DISTRICT SERVICE
        // =========================================================

        builder.Services.AddHttpClient<
            IDistrictService,
            DistrictService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<
                AuthenticatedHttpMessageHandler>();


        // =========================================================
        // USER SERVICE
        // =========================================================

        builder.Services.AddHttpClient<
            IUserService,
            UserService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<
                AuthenticatedHttpMessageHandler>();


        // =========================================================
        // TAX OFFICE SERVICE
        // =========================================================

        builder.Services.AddHttpClient<
            ITaxOfficeService,
            TaxOfficeService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<
                AuthenticatedHttpMessageHandler>();


        // =========================================================
        // ACCOUNT CARD TYPE SERVICE
        // =========================================================

        builder.Services.AddHttpClient<
            IAccountCardTypeService,
            AccountCardTypeService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<
                AuthenticatedHttpMessageHandler>();


        // =========================================================
        // ACCOUNT CARD KIND SERVICE
        // =========================================================

        builder.Services.AddHttpClient<
            IAccountCardKindService,
            AccountCardKindService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<
                AuthenticatedHttpMessageHandler>();


        // =========================================================
        // ACCOUNT CARD GROUP SERVICE
        // =========================================================

        builder.Services.AddHttpClient<
            IAccountCardGroupService,
            AccountCardGroupService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<
                AuthenticatedHttpMessageHandler>();


        // =========================================================
        // ACCOUNT CARD SUB GROUP SERVICE
        // =========================================================

        builder.Services.AddHttpClient<
            IAccountCardSubGroupService,
            AccountCardSubGroupService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<
                AuthenticatedHttpMessageHandler>();


        // =========================================================
        // ACCOUNT CARD SERVICE
        // =========================================================

        builder.Services.AddHttpClient<
            IAccountCardService,
            AccountCardService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<
                AuthenticatedHttpMessageHandler>();


        // =========================================================
        // BUILD
        // =========================================================

        return builder.Build();
    }
}