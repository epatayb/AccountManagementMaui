using AccountManagementMaui.Services;
using AccountManagementMaui.Shared.Services;
using AccountManagementMaui.Shared.Services.CityServices;
using Microsoft.Extensions.Logging;
using AccountManagementMaui.Shared.Services.DistrictServices;
using AccountManagementMaui.Shared.Services.UserServices;

namespace AccountManagementMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont(
                    "OpenSans-Regular.ttf",
                    "OpenSansRegular");
            });

        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

#if ANDROID
        const string apiBaseUrl =
            "http://10.0.2.2:5107/";
#else
        const string apiBaseUrl =
            "https://localhost:7192/";
#endif

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

        builder.Services.AddHttpClient<IUserService, UserService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return builder.Build();
    }
}