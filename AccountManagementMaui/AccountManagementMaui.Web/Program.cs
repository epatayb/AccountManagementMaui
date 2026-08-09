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
using AccountManagementMaui.Web.Authentication;
using AccountManagementMaui.Web.Components;
using AccountManagementMaui.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using AccountManagementMaui.Shared.Services.VehicleTypeServices;
using AccountManagementMaui.Shared.Services.VehicleKindServices;
using AccountManagementMaui.Shared.Services.VehicleServices;
using AccountManagementMaui.Shared.Services.VehicleLookupServices;

var builder = WebApplication.CreateBuilder(args);


// =========================================================
// RAZOR / BLAZOR
// =========================================================

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();


// =========================================================
// API BASE URL
// =========================================================

var apiBaseUrl =
    builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException(
        "ApiSettings:BaseUrl yapýlandýrmasý bulunamadý.");


// =========================================================
// AUTHORIZATION
// =========================================================

builder.Services.AddAuthorizationCore();

builder.Services.AddCascadingAuthenticationState();


// =========================================================
// TOKEN STORAGE
// =========================================================

builder.Services.AddScoped<
    IAuthTokenStorage,
    WebAuthTokenStorage>();


// =========================================================
// AUTH HTTP CLIENT
// =========================================================

// Login / Register / Refresh / Logout için.
// Bu client Bearer handler kullanmaz.

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
// AUTHENTICATED HTTP HANDLER
// =========================================================

builder.Services.AddScoped<
    AuthenticatedHttpMessageHandler>();


// =========================================================
// AUTHENTICATED API HTTP CLIENT
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
    IVehicleTypeService,
    VehicleTypeService>();

builder.Services.AddScoped<
    IVehicleKindService,
    VehicleKindService>();

builder.Services.AddScoped<
    IVehicleService,
    VehicleService>();

builder.Services.AddScoped<
    IVehicleLookupService,
    VehicleLookupService>();

// =========================================================
// FORM FACTOR
// =========================================================

builder.Services.AddSingleton<
    IFormFactor,
    FormFactor>();


// =========================================================
// BUILD
// =========================================================

var app = builder.Build();


// =========================================================
// HTTP PIPELINE
// =========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();

app.UseAntiforgery();


// =========================================================
// RAZOR COMPONENTS
// =========================================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(
            AccountManagementMaui.Shared._Imports
        ).Assembly);

app.Run();