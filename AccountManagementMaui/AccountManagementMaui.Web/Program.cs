using AccountManagementMaui.Shared.Services;
using AccountManagementMaui.Shared.Services.CityServices;
using AccountManagementMaui.Web.Components;
using AccountManagementMaui.Web.Services;
using AccountManagementMaui.Shared.Services.DistrictServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var apiBaseUrl =
    builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException(
        "ApiSettings:BaseUrl yapýlandýrmasý bulunamadý.");

builder.Services.AddHttpClient<ICityService, CityService>(
    client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

builder.Services.AddHttpClient<IDistrictService, DistrictService>(
    client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

builder.Services.AddSingleton<IFormFactor, FormFactor>();

var app = builder.Build();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(AccountManagementMaui.Shared._Imports).Assembly);

app.Run();