using System.Text.Json;
using AccountManagementMaui.Shared.Models.AuthModels;
using AccountManagementMaui.Shared.Services.AuthServices;
using Microsoft.JSInterop;

namespace AccountManagementMaui.Web.Authentication;

public class WebAuthTokenStorage : IAuthTokenStorage
{
    private const string StorageKey =
        "lts_auth_session";

    private readonly IJSRuntime _jsRuntime;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);


    public WebAuthTokenStorage(
        IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }


    public async Task SaveAsync(
        AuthResponse authResponse)
    {
        var json =
            JsonSerializer.Serialize(
                authResponse,
                JsonOptions);

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            StorageKey,
            json);
    }


    public async Task<AuthResponse?> GetAsync()
    {
        var json =
            await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                StorageKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<AuthResponse>(
                json,
                JsonOptions);
        }
        catch (JsonException)
        {
            await ClearAsync();

            return null;
        }
    }


    public async Task ClearAsync()
    {
        await _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            StorageKey);
    }
}