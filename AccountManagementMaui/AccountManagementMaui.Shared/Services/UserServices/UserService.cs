using System.Net;
using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Models.UserModels;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.UserServices;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<UserListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = "api/users";

        if (!string.IsNullOrWhiteSpace(search))
        {
            requestUri +=
                $"?search={Uri.EscapeDataString(search.Trim())}";
        }

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var users = await response.Content
            .ReadFromJsonAsync<List<UserListDto>>(
                cancellationToken: cancellationToken);

        return users ?? [];
    }

    public async Task<UserDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/users/{id}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<UserDetailDto>(
                cancellationToken: cancellationToken);
    }

    public async Task<UserDetailDto> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/users",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var user = await response.Content
            .ReadFromJsonAsync<UserDetailDto>(
                cancellationToken: cancellationToken);

        return user ?? throw new ApiException(
            "API geçerli bir kullanıcı bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task<UserDetailDto> UpdateAsync(
        int id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            $"api/users/{id}",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var user = await response.Content
            .ReadFromJsonAsync<UserDetailDto>(
                cancellationToken: cancellationToken);

        return user ?? throw new ApiException(
            "API geçerli bir kullanıcı bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task DeleteAsync(
        int id,
        DeleteUserRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"api/users/{id}")
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(
            httpRequest,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var error = await TryReadErrorAsync(
            response,
            cancellationToken);

        throw new ApiException(
            error?.GetErrorMessage() ??
            $"API isteği başarısız oldu. Durum kodu: {(int)response.StatusCode}",
            (int)response.StatusCode);
    }

    private static async Task<ApiErrorResponse?> TryReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content
                .ReadFromJsonAsync<ApiErrorResponse>(
                    cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}