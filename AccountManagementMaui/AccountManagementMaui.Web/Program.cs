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
// AUTHENTICATION / AUTHORIZATION
// =========================================================

builder.Services.AddAuthorizationCore();

builder.Services.AddCascadingAuthenticationState();


// Token saklama
builder.Services.AddScoped<
    IAuthTokenStorage,
    WebAuthTokenStorage>();


// Authentication State Provider
builder.Services.AddScoped<
    CustomAuthenticationStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(
    serviceProvider =>
        serviceProvider.GetRequiredService<
            CustomAuthenticationStateProvider>());


// JWT token'ý request header'a ekleyen handler
builder.Services.AddTransient<
    AuthenticatedHttpMessageHandler>();


// =========================================================
// AUTH SERVICE
// =========================================================

// Login / refresh iþlemlerinde JWT handler kullanmýyoruz.
builder.Services.AddHttpClient<IAuthService, AuthService>(
    client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });


// =========================================================
// CITY SERVICE
// =========================================================

builder.Services.AddHttpClient<ICityService, CityService>(
    client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddHttpMessageHandler<
        AuthenticatedHttpMessageHandler>();


// =========================================================
// DISTRICT SERVICE
// =========================================================

builder.Services.AddHttpClient<IDistrictService, DistrictService>(
    client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddHttpMessageHandler<
        AuthenticatedHttpMessageHandler>();


// =========================================================
// USER SERVICE
// =========================================================

builder.Services.AddHttpClient<IUserService, UserService>(
    client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
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
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
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
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
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
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
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
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
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
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
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
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddHttpMessageHandler<
        AuthenticatedHttpMessageHandler>();


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

app.UseStaticFiles();

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