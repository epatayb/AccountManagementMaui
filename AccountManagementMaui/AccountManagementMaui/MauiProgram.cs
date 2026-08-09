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
using AccountManagementMaui.Shared.Services.VehicleKindServices;
using AccountManagementMaui.Shared.Services.VehicleLookupServices;
using AccountManagementMaui.Shared.Services.VehicleServices;
using AccountManagementMaui.Shared.Services.VehicleTypeServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace AccountManagementMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder =
            MauiApp.CreateBuilder();


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

        builder.Services
            .AddMauiBlazorWebView();

#if DEBUG
        builder.Services
            .AddBlazorWebViewDeveloperTools();

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

        builder.Services
            .AddCascadingAuthenticationState();


        // =========================================================
        // TOKEN STORAGE
        // =========================================================

        builder.Services.AddSingleton<
            IAuthTokenStorage,
            MauiAuthTokenStorage>();


        // =========================================================
        // AUTH HTTP CLIENT
        // =========================================================

        builder.Services.AddHttpClient(
            "AuthApi",
            client =>
            {
                client.BaseAddress =
                    new Uri(apiBaseUrl);

                client.Timeout =
                    TimeSpan.FromSeconds(30);
            });


        // =========================================================
        // AUTH SERVICE
        // =========================================================

        builder.Services.AddScoped<IAuthService>(
            serviceProvider =>
            {
                var factory =
                    serviceProvider
                        .GetRequiredService<
                            IHttpClientFactory>();

                var client =
                    factory.CreateClient(
                        "AuthApi");

                var storage =
                    serviceProvider
                        .GetRequiredService<
                            IAuthTokenStorage>();

                return new AuthService(
                    client,
                    storage);
            });


        // =========================================================
        // AUTHENTICATION STATE PROVIDER
        // =========================================================

        builder.Services.AddScoped<
            CustomAuthenticationStateProvider>();

        builder.Services.AddScoped<
            AuthenticationStateProvider>(
            serviceProvider =>
                serviceProvider
                    .GetRequiredService<
                        CustomAuthenticationStateProvider>());


        // =========================================================
        // AUTHENTICATED HANDLER
        // =========================================================

        builder.Services.AddScoped<
            AuthenticatedHttpMessageHandler>();


        // =========================================================
        // AUTHENTICATED API CLIENT
        // =========================================================

        builder.Services.AddScoped<HttpClient>(
            serviceProvider =>
            {
                var handler =
                    serviceProvider
                        .GetRequiredService<
                            AuthenticatedHttpMessageHandler>();

                handler.InnerHandler =
                    new HttpClientHandler();

                return new HttpClient(
                    handler,
                    disposeHandler: true)
                {
                    BaseAddress =
                        new Uri(apiBaseUrl),

                    Timeout =
                        TimeSpan.FromSeconds(30)
                };
            });


        // =========================================================
        // API SERVICES
        // =========================================================

        builder.Services.AddScoped<
            ICityService,
            CityService>();

        builder.Services.AddScoped<
            IDistrictService,
            DistrictService>();

        builder.Services.AddScoped<
            IUserService,
            UserService>();

        builder.Services.AddScoped<
            ITaxOfficeService,
            TaxOfficeService>();

        builder.Services.AddScoped<
            IAccountCardTypeService,
            AccountCardTypeService>();

        builder.Services.AddScoped<
            IAccountCardKindService,
            AccountCardKindService>();

        builder.Services.AddScoped<
            IAccountCardGroupService,
            AccountCardGroupService>();

        builder.Services.AddScoped<
            IAccountCardSubGroupService,
            AccountCardSubGroupService>();

        builder.Services.AddScoped<
            IAccountCardService,
            AccountCardService>();

        builder.Services.AddScoped<
            IVehicleKindService,
            VehicleKindService>();

        builder.Services.AddScoped<
            IVehicleTypeService,
            VehicleTypeService>();

        builder.Services.AddScoped<
            IVehicleService,
            VehicleService>();

        builder.Services.AddScoped<
            IVehicleLookupService,
            VehicleLookupService>();

        // =========================================================
        // BUILD
        // =========================================================

        return builder.Build();
    }
}