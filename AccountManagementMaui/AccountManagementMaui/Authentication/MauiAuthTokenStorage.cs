using System.Text.Json;
using AccountManagementMaui.Shared.Models.AuthModels;
using AccountManagementMaui.Shared.Services.AuthServices;
using Microsoft.Maui.Storage;

namespace AccountManagementMaui.Authentication;

public class MauiAuthTokenStorage : IAuthTokenStorage
{
    private const string StorageKey =
        "lts_auth_session";

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);


    public async Task SaveAsync(
        AuthResponse authResponse)
    {
        var json =
            JsonSerializer.Serialize(
                authResponse,
                JsonOptions);

        await SecureStorage.Default.SetAsync(
            StorageKey,
            json);
    }


    public async Task<AuthResponse?> GetAsync()
    {
        try
        {
            var json =
                await SecureStorage.Default.GetAsync(
                    StorageKey);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<AuthResponse>(
                json,
                JsonOptions);
        }
        catch
        {
            ClearStorage();

            return null;
        }
    }


    public Task ClearAsync()
    {
        ClearStorage();

        return Task.CompletedTask;
    }


    private static void ClearStorage()
    {
        SecureStorage.Default.Remove(
            StorageKey);
    }
}